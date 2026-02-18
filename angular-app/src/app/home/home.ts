import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { VegCategoryService } from '../vegcategory.service';
import { Vegproduct, VegProduct } from '../vegproduct';
import { VegCategory } from '../vegcategory';

/**
 * HomeComponent - VeggyWorldShop eCommerce Landing Page
 * 
 * This is the main public-facing landing page for the eCommerce site.
 * Features:
 * - Hero section with compelling visuals and CTA
 * - Featured categories showcase
 * - Featured/new products display
 * - Responsive design optimized for conversion
 * - Integration with real product and category data
 */
@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    MatButtonModule,
    MatIconModule,
    MatCardModule
  ],
  templateUrl: './home.html',
  styleUrl: './home.css'
})
export class HomeComponent implements OnInit {
  private categoryService = inject(VegCategoryService);
  private productService = inject(Vegproduct);

  // Signal-based state management
  categories = signal<VegCategory[]>([]);
  featuredProducts = signal<VegProduct[]>([]);
  isLoading = signal(true);
  error = signal<string | null>(null);

  ngOnInit(): void {
    this.loadData();
  }

  /**
   * Load categories and products from API
   */
  private loadData(): void {
    this.isLoading.set(true);
    
    // Load categories
    this.categoryService.getVegcategories().subscribe({
      next: (data) => {
        this.categories.set(data);
      },
      error: (err) => {
        console.error('Error loading categories:', err);
        this.error.set('Failed to load categories');
      }
    });

    // Load products (featured products will be first 6)
    this.productService.getVegproducts().subscribe({
      next: (data) => {
        // Get first 6 products as featured, or products with stock > 0
        const featured = data
          .filter(p => (p.stockQuantity ?? 0) > 0)
          .slice(0, 6);
        this.featuredProducts.set(featured);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('Error loading products:', err);
        this.error.set('Failed to load products');
        this.isLoading.set(false);
      }
    });
  }

  /**
   * Format price for display
   */
  formatPrice(price: number): string {
    return new Intl.NumberFormat('es-CO', {
      style: 'currency',
      currency: 'COP',
      minimumFractionDigits: 0
    }).format(price);
  }

  /**
   * Get stock status badge class
   */
  getStockStatus(quantity: number | undefined): string {
    if (!quantity || quantity === 0) return 'out-of-stock';
    if (quantity < 10) return 'low-stock';
    return 'in-stock';
  }

  /**
   * Get stock status text
   */
  getStockText(quantity: number | undefined): string {
    if (!quantity || quantity === 0) return 'Out of Stock';
    if (quantity < 10) return `Only ${quantity} left`;
    return 'In Stock';
  }

  /**
   * Scroll to section smoothly
   */
  scrollToSection(sectionId: string): void {
    const element = document.getElementById(sectionId);
    if (element) {
      element.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
  }

  /**
   * Get Material icon for category based on name
   */
  getCategoryIcon(categoryName: string): string {
    const name = categoryName.toLowerCase();
    
    if (name.includes('vegetable') || name.includes('veggie')) return 'eco';
    if (name.includes('fruit')) return 'apple';
    if (name.includes('organic')) return 'nature';
    if (name.includes('fresh')) return 'grass';
    if (name.includes('leaf') || name.includes('salad') || name.includes('lettuce')) return 'park';
    if (name.includes('root')) return 'groundwater_detector';
    if (name.includes('herb') || name.includes('spice')) return 'spa';
    if (name.includes('beans') || name.includes('legume')) return 'grain';
    
    // Default icon
    return 'storefront';
  }
}

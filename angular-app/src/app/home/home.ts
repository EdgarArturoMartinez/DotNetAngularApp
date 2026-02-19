import { Component, inject, OnInit, signal, ViewChild, ElementRef, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { VegCategoryService } from '../vegcategory.service';
import { Vegproduct, VegProduct } from '../vegproduct';
import { VegCategory } from '../vegcategory';
import { ProductImageService } from '../shared/services/product-image.service';
import { environment } from '../../environments/environment';
import { forkJoin, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

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
    FormsModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatInputModule,
    MatFormFieldModule
  ],
  templateUrl: './home.html',
  styleUrl: './home.css'
})
export class HomeComponent implements OnInit {
  private categoryService = inject(VegCategoryService);
  private productService = inject(Vegproduct);
  private imageService = inject(ProductImageService);

  // Signal-based state management
  categories = signal<VegCategory[]>([]);
  featuredProducts = signal<VegProduct[]>([]);
  allProducts = signal<VegProduct[]>([]);
  productImages = signal<Map<number, string>>(new Map());
  isLoading = signal(true);
  error = signal<string | null>(null);
  
  // Search functionality
  searchText = signal<string>('');
  isSearchFocused = signal<boolean>(false);
  
  // Computed filtered products
  filteredProducts = computed(() => {
    const search = this.searchText().toLowerCase().trim();
    const products = this.allProducts();
    
    if (!search) {
      return products;
    }
    
    return products.filter(product => 
      product.name.toLowerCase().includes(search) ||
      product.description?.toLowerCase().includes(search)
    );
  });

  // Placeholder image for products without a main image (using data URL for emoji-based hero design)
  readonly PLACEHOLDER_IMAGE = 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iNDAwIiBoZWlnaHQ9IjI0MCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj48ZGVmcz48bGluZWFyR3JhZGllbnQgaWQ9ImciIHgxPSIwJSIgeTE9IjAlIiB4Mj0iMTAwJSIgeTI9IjEwMCUiPjxzdG9wIG9mZnNldD0iMCUiIHN0eWxlPSJzdG9wLWNvbG9yOiM2NjdlZWE7c3RvcC1vcGFjaXR5OjEiLz48c3RvcCBvZmZzZXQ9IjUwJSIgc3R5bGU9InN0b3AtY29sb3I6Izc2NGJhMjtzdG9wLW9wYWNpdHk6MSIvPjxzdG9wIG9mZnNldD0iMTAwJSIgc3R5bGU9InN0b3AtY29sb3I6I2YwOTNmYjtzdG9wLW9wYWNpdHk6MSIvPjwvbGluZWFyR3JhZGllbnQ+PC9kZWZzPjxyZWN0IHdpZHRoPSI0MDAiIGhlaWdodD0iMjQwIiBmaWxsPSJ1cmwoI2cpIi8+PHRleHQgeD0iNTAlIiB5PSI0NSUiIGZvbnQtc2l6ZT0iNjQiIHRleHQtYW5jaG9yPSJtaWRkbGUiIGRvbWluYW50LWJhc2VsaW5lPSJtaWRkbGUiPvCfpafwn6WWIPCfpZo8L3RleHQ+PHRleHQgeD0iNTAlIiB5PSI3MCUiIGZvbnQtc2l6ZT0iMTYiIGZvbnQtZmFtaWx5PSJBcmlhbCIgZmlsbD0iI2ZmZmZmZiIgdGV4dC1hbmNob3I9Im1pZGRsZSIgZG9taW5hbnQtYmFzZWxpbmU9Im1pZGRsZSIgb3BhY2l0eT0iMC45Ij5GcmVzaCBWZWdldGFibGVzPC90ZXh0Pjwvc3ZnPg==';

  @ViewChild('categoriesCarousel') categoriesCarousel!: ElementRef;
  @ViewChild('featuresCarousel') featuresCarousel!: ElementRef;

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
        // Store all products
        this.allProducts.set(data);
        
        // Get first 6 products as featured, or products with stock > 0
        const featured = data
          .filter(p => (p.stockQuantity ?? 0) > 0)
          .slice(0, 6);
        this.featuredProducts.set(featured);
        
        // Load main images for all products
        this.loadProductImages(data);
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
   * Scroll categories carousel horizontally
   */
  scrollCategories(direction: 'left' | 'right'): void {
    const carousel = this.categoriesCarousel.nativeElement;
    const scrollAmount = 320; // Card width + gap
    const scrollDistance = direction === 'left' ? -scrollAmount : scrollAmount;
    
    carousel.scrollBy({
      left: scrollDistance,
      behavior: 'smooth'
    });
  }

  /**
   * Scroll features carousel horizontally
   */
  scrollFeatures(direction: 'left' | 'right'): void {
    const carousel = this.featuresCarousel.nativeElement;
    const scrollAmount = 320; // Card width + gap
    const scrollDistance = direction === 'left' ? -scrollAmount : scrollAmount;
    
    carousel.scrollBy({
      left: scrollDistance,
      behavior: 'smooth'
    });
  }

  /**
   * Handle search input focus
   */
  onSearchFocus(): void {
    this.isSearchFocused.set(true);
    // Scroll to products section smoothly
    setTimeout(() => {
      this.scrollToSection('products');
    }, 100);
  }

  /**
   * Handle search input blur
   */
  onSearchBlur(): void {
    // Delay to allow click events on clear button and products
    setTimeout(() => {
      // Only hide if there's no search text
      if (!this.searchText()) {
        this.isSearchFocused.set(false);
      }
    }, 200);
  }

  /**
   * Handle search text change
   */
  onSearchChange(value: string): void {
    this.searchText.set(value);
    // Scroll to products when user starts typing
    if (value.trim()) {
      setTimeout(() => {
        this.scrollToSection('products');
      }, 50);
    }
  }

  /**
   * Clear search
   */
  clearSearch(): void {
    this.searchText.set('');
    this.isSearchFocused.set(false);
  }

  /**
   * Load main images for products
   */
  private loadProductImages(products: VegProduct[]): void {
    const imageRequests = products.map(product => 
      this.imageService.getMainImage(product.id).pipe(
        map(image => ({ productId: product.id, imageUrl: `${environment.apiURL}/${image.imageUrl}` })),
        catchError(() => {
          // 404 is expected when no images are uploaded - silently use placeholder
          return of({ productId: product.id, imageUrl: this.PLACEHOLDER_IMAGE });
        })
      )
    );

    forkJoin(imageRequests).subscribe({
      next: (results) => {
        const imageMap = new Map<number, string>();
        results.forEach(result => {
          imageMap.set(result.productId, result.imageUrl);
        });
        this.productImages.set(imageMap);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('Error loading product images:', err);
        this.isLoading.set(false);
      }
    });
  }

  /**
   * Get product image URL with fallback to placeholder
   */
  getProductImageUrl(productId: number): string {
    return this.productImages().get(productId) || this.PLACEHOLDER_IMAGE;
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

  /**
   * Get product display name with net weight and unit
   * Format: "Product Name - 500 Gms"
   */
  getProductDisplayName(product: VegProduct): string {
    let displayName = product.name;
    
    if (product.netWeight && product.vegTypeWeight?.abbreviationWeight) {
      displayName += ` - ${product.netWeight} ${product.vegTypeWeight.abbreviationWeight}`;
    } else if (product.netWeight) {
      displayName += ` - ${product.netWeight}`;
    }
    
    return displayName;
  }
}

import { Component, OnInit, inject, ChangeDetectorRef, ChangeDetectionStrategy } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { RouterLink, Router } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { CommonModule } from '@angular/common';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { Vegproduct } from '../vegproduct';

@Component({
  selector: 'app-index-products',
  imports: [MatButtonModule, RouterLink, MatIconModule, CommonModule, MatProgressSpinnerModule, MatTooltipModule, MatSnackBarModule],
  templateUrl: './index-products.html',
  styleUrl: './index-products.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class IndexProducts implements OnInit {
  vegproductService = inject(Vegproduct);
  cdr = inject(ChangeDetectorRef);
  router = inject(Router);
  snackBar = inject(MatSnackBar);
  products: any[] = [];
  isLoading = false;
  error = '';

  ngOnInit() {
    this.loadProducts();
  }

  loadProducts() {
    this.isLoading = true;
    this.error = '';
    this.products = [];
    
    this.vegproductService.getVegproducts().subscribe({
      next: (data: any) => {
        // Ensure each product has a valid ID property
        this.products = Array.isArray(data) ? data.map(product => {
          // Handle both 'id' and 'Id' or 'idVegproduct' property names from API
          if (!product.id && (product.Id || product.idVegproduct)) {
            product.id = product.Id || product.idVegproduct;
          }
          return product;
        }) : [];
        console.log('Products loaded:', this.products);
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: (error: any) => {
        this.error = 'Failed to load products: ' + (error.message || error.statusText || 'Unknown');
        this.isLoading = false;
        this.cdr.markForCheck();
      }
    });
  }

  editProduct(product: any) {
    // Navigate to edit page with product ID
    // Handle both 'id' and potential API response variations
    const productId = product.id || product.idVegproduct || product.Id;
    
    if (!productId) {
      this.snackBar.open('Error: Product ID not found. Please refresh and try again.', 'Close', {
        duration: 5000,
        horizontalPosition: 'end',
        verticalPosition: 'bottom',
        panelClass: ['error-snackbar']
      });
      return;
    }
    
    this.router.navigate(['/products/edit', productId]);
  }

  deleteProduct(product: any) {
    // Confirm before deleting
    if (confirm(`Are you sure you want to delete "${product.name}"?`)) {
      this.vegproductService.deleteVegproduct(product.id).subscribe({
        next: (response) => {
          this.snackBar.open(`✓ Product "${product.name}" deleted successfully!`, 'Close', {
            duration: 3000,
            horizontalPosition: 'end',
            verticalPosition: 'bottom',
            panelClass: ['success-snackbar']
          });
          // Refresh the list
          this.loadProducts();
        },
        error: (error) => {
          let errorMessage = error.error?.message || error.statusText || 'Unknown error';
          this.snackBar.open(`✗ Error deleting product: ${errorMessage}`, 'Close', {
            duration: 5000,
            horizontalPosition: 'end',
            verticalPosition: 'bottom',
            panelClass: ['error-snackbar']
          });
        }
      });
    }
  }
}

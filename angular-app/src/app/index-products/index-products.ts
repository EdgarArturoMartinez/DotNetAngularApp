import { Component, OnInit, inject, ChangeDetectorRef, ChangeDetectionStrategy } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { RouterLink, Router } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { CommonModule } from '@angular/common';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Vegproduct } from '../vegproduct';
import { DialogService } from '../shared/services/dialog.service';
import { NotificationService } from '../shared/services/notification.service';

@Component({
  selector: 'app-index-products',
  imports: [MatButtonModule, RouterLink, MatIconModule, CommonModule, MatProgressSpinnerModule, MatTooltipModule],
  templateUrl: './index-products.html',
  styleUrl: './index-products.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class IndexProducts implements OnInit {
  vegproductService = inject(Vegproduct);
  cdr = inject(ChangeDetectorRef);
  router = inject(Router);
  dialogService = inject(DialogService);
  notificationService = inject(NotificationService);
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
        console.log('First product stockQuantity:', this.products[0]?.stockQuantity);
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
      this.notificationService.error('Product ID not found. Please refresh and try again.');
      return;
    }
    
    this.router.navigate(['/products/edit', productId]);
  }

  deleteProduct(product: any) {
    this.dialogService.confirmDelete('Product', product.name).subscribe(confirmed => {
      if (confirmed) {
        this.vegproductService.deleteVegproduct(product.id).subscribe({
          next: () => {
            this.notificationService.deleted('Product', product.name);
            this.loadProducts();
          },
          error: (error) => {
            const errorMessage = error.error?.message || error.statusText || 'Unknown error';
            this.notificationService.saveError('delete', errorMessage);
          }
        });
      }
    });
  }
}

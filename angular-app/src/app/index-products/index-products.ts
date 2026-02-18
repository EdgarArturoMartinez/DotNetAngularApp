import { Component, OnInit, inject, ChangeDetectorRef, ChangeDetectionStrategy } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { RouterLink, Router } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { CommonModule } from '@angular/common';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ProductService } from '../features/products/services/product.service';
import { VegProduct } from '../shared/models/entities';
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
  productService = inject(ProductService);
  cdr = inject(ChangeDetectorRef);
  router = inject(Router);
  dialogService = inject(DialogService);
  notificationService = inject(NotificationService);
  products: VegProduct[] = [];
  isLoading = false;
  error = '';

  ngOnInit() {
    this.loadProducts();
  }

  loadProducts() {
    this.isLoading = true;
    this.error = '';
    this.products = [];
    
    this.productService.getAll().subscribe({
      next: (data) => {
        this.products = data;
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: (error) => {
        this.error = 'Failed to load products: ' + (error.message || error.statusText || 'Unknown');
        this.isLoading = false;
        this.cdr.markForCheck();
      }
    });
  }

  editProduct(product: VegProduct) {
    if (!product.id) {
      this.notificationService.error('Product ID not found. Please refresh and try again.');
      return;
    }
    
    this.router.navigate(['/products/edit', product.id]);
  }

  deleteProduct(product: VegProduct) {
    this.dialogService.confirmDelete('Product', product.name).subscribe(confirmed => {
      if (confirmed) {
        this.productService.delete(product.id!).subscribe({
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

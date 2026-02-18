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
import { GenericDataTableComponent, ColumnDefinition, TableAction } from '../shared/components/generic-data-table/generic-data-table.component';

@Component({
  selector: 'app-index-products',
  imports: [MatButtonModule, RouterLink, MatIconModule, CommonModule, MatProgressSpinnerModule, MatTooltipModule, GenericDataTableComponent],
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

  // Table configuration
  columns: ColumnDefinition[] = [
    { key: 'id', label: 'ID', type: 'number', width: '80px' },
    { key: 'name', label: 'Product Name', type: 'text' },
    { key: 'price', label: 'Price', type: 'currency', width: '150px' },
    { key: 'vegCategory.categoryName', label: 'Category', type: 'text' },
    { key: 'stockQuantity', label: 'Stock', type: 'number', width: '100px' },
    { key: 'description', label: 'Description', type: 'text' }
  ];

  hiddenColumns: string[] = ['id'];

  actions: TableAction[] = [
    { label: 'Edit', icon: 'edit', action: 'edit', tooltip: 'Edit this product' },
    { label: 'Delete', icon: 'delete', action: 'delete', color: 'warn', tooltip: 'Delete this product' }
  ];

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

  onEdit(product: VegProduct) {
    if (!product.id) {
      this.notificationService.error('Product ID not found. Please refresh and try again.');
      return;
    }
    this.router.navigate(['/products/edit', product.id]);
  }

  onDelete(product: VegProduct) {
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

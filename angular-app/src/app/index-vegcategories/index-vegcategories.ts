import { Component, OnInit, inject, ChangeDetectorRef, ChangeDetectionStrategy } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { RouterLink, Router } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { CommonModule } from '@angular/common';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { CategoryService } from '../features/categories/services/category.service';
import { VegCategory } from '../shared/models/entities';
import { DialogService } from '../shared/services/dialog.service';
import { NotificationService } from '../shared/services/notification.service';
import { GenericDataTableComponent, ColumnDefinition, TableAction } from '../shared/components/generic-data-table/generic-data-table.component';

@Component({
  selector: 'app-index-vegcategories',
  standalone: true,
  imports: [MatButtonModule, RouterLink, MatIconModule, CommonModule, MatProgressSpinnerModule, MatTooltipModule, GenericDataTableComponent],
  templateUrl: './index-vegcategories.html',
  styleUrl: './index-vegcategories.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class IndexVegcategories implements OnInit {
  categoryService = inject(CategoryService);
  cdr = inject(ChangeDetectorRef);
  router = inject(Router);
  dialogService = inject(DialogService);
  notificationService = inject(NotificationService);
  categories: VegCategory[] = [];
  isLoading = false;
  error = '';

  // Table configuration
  columns: ColumnDefinition[] = [
    { key: 'idCategory', label: 'ID', type: 'number', width: '80px' },
    { key: 'categoryName', label: 'Category Name', type: 'text' },
    { key: 'description', label: 'Description', type: 'text' },
    { key: 'productCount', label: 'Products', type: 'number', width: '100px' },
    { key: 'createdAt', label: 'Created', type: 'date', width: '150px' }
  ];

  hiddenColumns: string[] = ['idCategory'];

  actions: TableAction[] = [
    { label: 'Edit', icon: 'edit', action: 'edit', tooltip: 'Edit this category' },
    { label: 'Delete', icon: 'delete', action: 'delete', color: 'warn', tooltip: 'Delete this category' }
  ];

  ngOnInit() {
    this.loadCategories();
  }

  loadCategories() {
    this.isLoading = true;
    this.error = '';
    this.categories = [];
    
    console.log('Loading categories from API...');
    this.categoryService.getAll().subscribe({
      next: (data) => {
        console.log('Categories loaded successfully:', data);
        this.categories = data;
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: (error) => {
        console.error('Error loading categories:', error);
        if (error.status === 0) {
          this.error = 'Cannot connect to backend. Check CORS settings on the .NET API.';
        } else {
          this.error = 'Failed to load categories: ' + (error.message || error.statusText || 'Unknown');
        }
        this.isLoading = false;
        this.cdr.markForCheck();
      }
    });
  }

  onEdit(category: VegCategory) {
    if (!category.idCategory) {
      this.notificationService.error('Category ID not found. Please refresh and try again.');
      return;
    }
    this.router.navigate(['/categories/edit', category.idCategory]);
  }

  onDelete(category: VegCategory) {
    this.dialogService.confirmDelete('Category', category.categoryName).subscribe(confirmed => {
      if (confirmed) {
        this.categoryService.delete(category.idCategory!).subscribe({
          next: () => {
            this.notificationService.deleted('Category', category.categoryName);
            this.loadCategories();
          },
          error: (error) => {
            const errorMessage = error.error?.message || error.statusText || 'Unknown error';
            this.notificationService.saveError('delete', errorMessage);
          }
        });
      }
    });
  }

  editCategory(category: VegCategory) {
    if (!category.idCategory) {
      this.notificationService.error('Category ID not found. Please refresh and try again.');
      return;
    }
    
    this.router.navigate(['/categories/edit', category.idCategory]);
  }

  deleteCategory(category: VegCategory) {
    this.dialogService.confirmDelete('Category', category.categoryName).subscribe(confirmed => {
      if (confirmed) {
        this.categoryService.delete(category.idCategory!).subscribe({
          next: () => {
            this.notificationService.deleted('Category', category.categoryName);
            this.loadCategories();
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

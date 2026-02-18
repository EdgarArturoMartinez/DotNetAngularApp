import { Component, OnInit, inject, ChangeDetectorRef, ChangeDetectionStrategy } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { RouterLink, Router } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { CommonModule } from '@angular/common';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { VegCategoryService } from '../vegcategory.service';
import { VegCategory } from '../vegcategory';
import { DialogService } from '../shared/services/dialog.service';
import { NotificationService } from '../shared/services/notification.service';

@Component({
  selector: 'app-index-vegcategories',
  standalone: true,
  imports: [MatButtonModule, RouterLink, MatIconModule, CommonModule, MatProgressSpinnerModule, MatTooltipModule],
  templateUrl: './index-vegcategories.html',
  styleUrl: './index-vegcategories.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class IndexVegcategories implements OnInit {
  vegCategoryService = inject(VegCategoryService);
  cdr = inject(ChangeDetectorRef);
  router = inject(Router);
  dialogService = inject(DialogService);
  notificationService = inject(NotificationService);
  categories: VegCategory[] = [];
  isLoading = false;
  error = '';

  ngOnInit() {
    this.loadCategories();
  }

  loadCategories() {
    this.isLoading = true;
    this.error = '';
    this.categories = [];
    
    console.log('Loading categories from API...');
    this.vegCategoryService.getVegcategories().subscribe({
      next: (data: VegCategory[]) => {
        console.log('Categories loaded successfully:', data);
        // Ensure each category has a valid idCategory property
        this.categories = Array.isArray(data) ? data.map(category => {
          // Handle both 'idCategory' and 'Id' or 'id' property names from API
          if (!category.idCategory && ((category as any).Id || (category as any).id)) {
            category.idCategory = (category as any).Id || (category as any).id;
          }
          return category;
        }) : [];
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: (error: any) => {
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

  editCategory(category: VegCategory) {
    // Handle both 'idCategory' and potential API response variations
    const categoryId = category.idCategory || (category as any).id || (category as any).Id;
    
    if (!categoryId) {
      this.notificationService.error('Category ID not found. Please refresh and try again.');
      return;
    }
    
    this.router.navigate(['/categories/edit', categoryId]);
  }

  deleteCategory(category: VegCategory) {
    this.dialogService.confirmDelete('Category', category.categoryName).subscribe(confirmed => {
      if (confirmed) {
        this.vegCategoryService.deleteVegcategory(category.idCategory).subscribe({
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

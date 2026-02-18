import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { Router, RouterLink, ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { VegCategoryService } from '../vegcategory.service';
import { VegCategory } from '../vegcategory';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { NotificationService } from '../shared/services/notification.service';

@Component({
  selector: 'app-edit-vegcategory',
  standalone: true,
  imports: [ReactiveFormsModule, MatFormFieldModule, MatInputModule, MatButtonModule, RouterLink, CommonModule, MatIconModule, MatProgressSpinnerModule],
  templateUrl: './edit-vegcategory.html',
  styleUrl: './edit-vegcategory.css',
})
export class EditVegcategory implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private vegCategoryService = inject(VegCategoryService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private notificationService = inject(NotificationService);
  private cdr = inject(ChangeDetectorRef);

  categoryId: number = 0;
  isLoading = true;
  category: VegCategory | null = null;

  vegCategoryForm = this.formBuilder.group({
    categoryName: ['', Validators.required],
    description: ['']
  });

  ngOnInit() {
    console.log('EditVegcategory ngOnInit called');
    const id = this.route.snapshot.paramMap.get('id');
    console.log('Category ID from route:', id);
    if (id && id !== 'undefined' && id.trim() !== '') {
      this.categoryId = parseInt(id);
      console.log('Parsed categoryId:', this.categoryId);
      if (!isNaN(this.categoryId)) {
        this.loadCategory();
      } else {
        console.log('Invalid category ID format:', id);
        this.notificationService.error('Invalid category ID');
        this.router.navigate(['/categories']);
      }
    } else {
      this.notificationService.error('Invalid category ID');
      this.router.navigate(['/categories']);
    }
  }

  loadCategory() {
    console.log('loadCategory called for ID:', this.categoryId);
    this.isLoading = true;
    console.log('Calling API:', `https://localhost:7020/api/vegcategories/${this.categoryId}`);
    
    this.vegCategoryService.getVegcategoryById(this.categoryId).subscribe({
      next: (category) => {
        try {
          console.log('SUCCESS! Category loaded:', category);
          console.log('Setting this.category...');
          this.category = category;
          console.log('this.category set successfully');
          
          console.log('About to patch form with:', {
            categoryName: category.categoryName,
            description: category.description || ''
          });
          
          this.vegCategoryForm.patchValue({
            categoryName: category.categoryName,
            description: category.description || ''
          });
          console.log('Form patched successfully');
          
          console.log('Setting isLoading to false...');
          this.isLoading = false;
          console.log('isLoading is now:', this.isLoading);
          console.log('Triggering change detection...');
          this.cdr.detectChanges();
          console.log('Change detection triggered!');
        } catch (error) {
          console.error('ERROR in success handler:', error);
          this.isLoading = false;
        }
      },
      error: (error) => {
        console.error('ERROR loading category:', error);
        console.error('Error status:', error.status);
        console.error('Error message:', error.message);
        console.error('Full error object:', JSON.stringify(error, null, 2));
        
        let errorMsg = 'Error loading category';
        if (error.status === 404) {
          errorMsg = 'Category not found';
        } else if (error.status === 0) {
          errorMsg = 'Cannot connect to backend. Check if API is running and CORS is configured.';
        } else {
          errorMsg = `Error ${error.status}: ${error.message || 'Unknown error'}`;
        }
        
        this.notificationService.error(errorMsg, 6000);
        this.isLoading = false;
        console.log('Error handled, isLoading set to false, navigating back...');
        setTimeout(() => {
          this.router.navigate(['/categories']);
        }, 2000);
      }
    });
  }

  saveChanges() {
    if (this.vegCategoryForm.valid && this.categoryId) {
      const formValue = this.vegCategoryForm.value;
      const categoryData: any = {
        idCategory: this.categoryId,
        categoryName: formValue.categoryName,
        description: formValue.description || ''
      };
      
      this.vegCategoryService.updateVegcategory(this.categoryId, categoryData).subscribe({
        next: () => {
          const categoryName = formValue.categoryName ?? undefined;
          this.notificationService.updated('Category', categoryName);
          
          setTimeout(() => {
            this.router.navigate(['/categories']);
          }, 1000);
        },
        error: (error) => {
          let errorMessage = 'Unknown error';
          if (error.error) {
            if (typeof error.error === 'string') {
              errorMessage = error.error;
            } else if (error.error.message) {
              errorMessage = error.error.message;
            } else if (error.error.title) {
              errorMessage = error.error.title;
            } else {
              errorMessage = JSON.stringify(error.error);
            }
          }
          
          this.notificationService.saveError('update', errorMessage);
        }
      });
    }
  }
}

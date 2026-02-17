import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { Router, RouterLink } from '@angular/router';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { CommonModule } from '@angular/common';
import { VegCategoryService } from '../vegcategory.service';
import { VegCategory } from '../vegcategory';

@Component({
  selector: 'app-create-vegcategory',
  standalone: true,
  imports: [ReactiveFormsModule, MatFormFieldModule, MatInputModule, MatButtonModule, RouterLink, MatSnackBarModule, CommonModule, MatIconModule],
  templateUrl: './create-vegcategory.html',
  styleUrl: './create-vegcategory.css',
})
export class CreateVegcategory {
  private readonly formBuilder = inject(FormBuilder);
  private vegCategoryService = inject(VegCategoryService);
  private router = inject(Router);
  private snackBar = inject(MatSnackBar);

  vegCategoryForm = this.formBuilder.group({
    categoryName: ['', Validators.required],
    description: ['']
  });

  saveChanges() {
    if (this.vegCategoryForm.valid) {
      const formValue = this.vegCategoryForm.value;
      const categoryData: any = {
        categoryName: formValue.categoryName,
        description: formValue.description || ''
      };
      
      this.vegCategoryService.createVegcategory(categoryData).subscribe({
        next: (response) => {
          this.vegCategoryForm.reset();
          
          this.snackBar.open(`✓ Category "${formValue.categoryName}" created successfully!`, 'Close', {
            duration: 4000,
            horizontalPosition: 'end',
            verticalPosition: 'bottom',
            panelClass: ['success-snackbar']
          });
          
          setTimeout(() => {
            this.router.navigate(['/categories']);
          }, 1000);
        },
        error: (error) => {
          console.error('Full error object:', error);
          let errorMessage = 'Unknown error';
          
          if (error.status === 0) {
            errorMessage = 'Cannot connect to backend API. Make sure the .NET server is running on https://localhost:7020';
          } else if (error.error) {
            if (typeof error.error === 'string') {
              errorMessage = error.error;
            } else if (error.error.message) {
              errorMessage = error.error.message;
            } else if (error.error.title) {
              errorMessage = error.error.title;
            } else {
              errorMessage = JSON.stringify(error.error);
            }
          } else if (error.message) {
            errorMessage = error.message;
          }
          
          this.snackBar.open(`✗ Error: ${errorMessage}`, 'Close', {
            duration: 8000,
            horizontalPosition: 'end',
            verticalPosition: 'bottom',
            panelClass: ['error-snackbar']
          });
        }
      });
    } else {
      this.snackBar.open('Please fill in all required fields', 'Close', {
        duration: 3000,
        horizontalPosition: 'end',
        verticalPosition: 'bottom',
        panelClass: ['warn-snackbar']
      });
    }
  }
}

import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatHint } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { Router, RouterLink } from '@angular/router';
import { Vegproduct } from '../vegproduct';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { CommonModule } from '@angular/common';
import { VegCategoryService } from '../vegcategory.service';
import { VegCategory } from '../vegcategory';

@Component({
  selector: 'app-create-vegproduct',
  imports: [ReactiveFormsModule, MatFormFieldModule, MatInputModule, MatSelectModule, MatButtonModule, RouterLink, MatSnackBarModule, CommonModule, MatHint, MatIconModule],
  templateUrl: './create-vegproduct.html',
  styleUrl: './create-vegproduct.css',
})
export class CreateVegproduct implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  vegProduct = inject(Vegproduct);
  vegCategoryService = inject(VegCategoryService);
  router = inject(Router);
  snackBar = inject(MatSnackBar);

  categories: VegCategory[] = [];

  vegProductForm = this.formBuilder.group({
    name: ['', Validators.required],
    price: ['', Validators.required],
    description: [''],
    stockQuantity: [0, Validators.required],
    idCategory: [null as number | null]
  });

  ngOnInit() {
    this.loadCategories();
  }

  loadCategories() {
    this.vegCategoryService.getVegcategories().subscribe({
      next: (data) => {
        // Normalize category IDs in case API returns different property names
        this.categories = data.map(cat => {
          if (!cat.idCategory && ((cat as any).Id || (cat as any).id)) {
            cat.idCategory = (cat as any).Id || (cat as any).id;
          }
          return cat;
        });
        console.log('Categories loaded for dropdown:', this.categories);
      },
      error: (error) => {
        console.error('Error loading categories:', error);
      }
    });
  }

  formatPrice() {
    const priceField = this.vegProductForm.get('price');
    if (priceField && priceField.value) {
      const numValue = parseFloat(priceField.value);
      if (!isNaN(numValue)) {
        priceField.setValue(numValue.toFixed(2));
      }
    }
  }

  saveChanges() {
    if (this.vegProductForm.valid) {
      const formValue = this.vegProductForm.value;
      const vegProductData: any = {
        name: formValue.name,
        price: parseFloat(formValue.price || '0'),
        stockQuantity: formValue.stockQuantity || 0,
        idCategory: formValue.idCategory || null
      };
      
      this.vegProduct.createVegproduct(vegProductData).subscribe({
        next: (response) => {
          this.vegProductForm.reset();
          
          // Show success message
          this.snackBar.open(`✓ Product "${formValue.name}" created successfully!`, 'Close', {
            duration: 4000,
            horizontalPosition: 'end',
            verticalPosition: 'bottom',
            panelClass: ['success-snackbar']
          });
          
          // Navigate after 1 second
          setTimeout(() => {
            this.router.navigate(['/products']);
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
          
          this.snackBar.open(`✗ Error: ${errorMessage}`, 'Close', {
            duration: 5000,
            horizontalPosition: 'end',
            verticalPosition: 'bottom',
            panelClass: ['error-snackbar']
          });
        }
      });
    } else {
      this.snackBar.open('Please fill in all fields correctly', 'Close', {
        duration: 3000,
        horizontalPosition: 'end',
        verticalPosition: 'bottom',
        panelClass: ['warn-snackbar']
      });
    }
  }
}

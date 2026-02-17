import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatHint } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Vegproduct } from '../vegproduct';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { CommonModule } from '@angular/common';
import { VegCategoryService } from '../vegcategory.service';
import { VegCategory } from '../vegcategory';

@Component({
  selector: 'app-edit-vegproduct',
  imports: [ReactiveFormsModule, MatFormFieldModule, MatInputModule, MatSelectModule, MatButtonModule, RouterLink, MatSnackBarModule, CommonModule, MatHint, MatIconModule, MatProgressSpinnerModule],
  templateUrl: './edit-vegproduct.html',
  styleUrl: './edit-vegproduct.css',
})
export class EditVegproduct implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  vegProduct = inject(Vegproduct);
  vegCategoryService = inject(VegCategoryService);
  router = inject(Router);
  snackBar = inject(MatSnackBar);
  activatedRoute = inject(ActivatedRoute);
  cdr = inject(ChangeDetectorRef);

  productId: number | null = null;
  isLoading = true;
  categories: VegCategory[] = [];

  vegProductForm = this.formBuilder.group({
    name: ['', Validators.required],
    price: ['', Validators.required],
    idCategory: [null as number | null]
  });

  ngOnInit() {
    // Load categories first
    this.loadCategories();
    
    // Get the product ID from the route parameters
    this.activatedRoute.paramMap.subscribe(params => {
      const id = params.get('id');
      console.log('Route param ID:', id);
      
      if (id && id !== 'undefined' && id.trim() !== '') {
        this.productId = parseInt(id, 10);
        console.log('Parsed product ID:', this.productId);
        if (!isNaN(this.productId)) {
          this.loadProduct();
        } else {
          console.log('Invalid product ID format:', id);
          this.snackBar.open('Invalid product ID', 'Close', {
            duration: 3000,
            horizontalPosition: 'end',
            verticalPosition: 'bottom',
            panelClass: ['error-snackbar']
          });
          this.router.navigate(['/products']);
        }
      } else {
        console.log('No ID found in route');
        this.snackBar.open('Invalid product ID', 'Close', {
          duration: 3000,
          horizontalPosition: 'end',
          verticalPosition: 'bottom',
          panelClass: ['error-snackbar']
        });
        this.router.navigate(['/products']);
      }
    });
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

  loadProduct() {
    console.log('loadProduct called, productId:', this.productId);
    if (!this.productId) {
      console.log('No productId, returning');
      return;
    }

    this.isLoading = true;
    
    this.vegProduct.getVegproductById(this.productId).subscribe({
      next: (product) => {
        console.log('SUCCESS! Product loaded:', product);
        
        this.vegProductForm.patchValue({
          name: product.name,
          price: product.price.toFixed(2),
          idCategory: product.idCategory || null
        });
        
        console.log('Form populated with:', this.vegProductForm.value);
        
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error('ERROR loading product:', error);
        let errorMessage = 'Failed to load product';
        
        if (error.status === 404) {
          errorMessage = 'Product not found';
        } else if (error.status === 0) {
          errorMessage = 'Cannot connect to backend';
        } else if (error.error && error.error.message) {
          errorMessage = error.error.message;
        }

        this.snackBar.open(`✗ Error: ${errorMessage}`, 'Close', {
          duration: 5000,
          horizontalPosition: 'end',
          verticalPosition: 'bottom',
          panelClass: ['error-snackbar']
        });
        
        this.isLoading = false;
        this.cdr.detectChanges();
        
        setTimeout(() => {
          this.router.navigate(['/products']);
        }, 2000);
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
    console.log('saveChanges called');
    console.log('Form valid:', this.vegProductForm.valid);
    console.log('Form value:', this.vegProductForm.value);
    console.log('Product ID:', this.productId);
    
    if (this.vegProductForm.valid && this.productId) {
      const formValue = this.vegProductForm.value;
      
      // Ensure we have valid values
      if (!formValue.name || !formValue.price) {
        console.error('Name or price missing:', { name: formValue.name, price: formValue.price });
        this.snackBar.open('Please fill in all required fields', 'Close', {
          duration: 3000,
          horizontalPosition: 'end',
          verticalPosition: 'bottom',
          panelClass: ['warn-snackbar']
        });
        return;
      }
      
      const vegProductData: any = {
        id: this.productId,
        name: String(formValue.name).trim(),
        price: parseFloat(String(formValue.price)),
        description: '',
        idCategory: formValue.idCategory || null
      };
      
      console.log('Submitting product data:', vegProductData);
      
      this.vegProduct.updateVegproduct(this.productId, vegProductData).subscribe({
        next: () => {
          this.snackBar.open(`✓ Product "${formValue.name}" updated successfully!`, 'Close', {
            duration: 4000,
            horizontalPosition: 'end',
            verticalPosition: 'bottom',
            panelClass: ['success-snackbar']
          });
          
          setTimeout(() => {
            this.router.navigate(['/products']);
          }, 1000);
        },
        error: (error) => {
          console.error('Update error:', error);
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
      console.log('Form invalid or no product ID');
      this.snackBar.open('Please fill in all required fields correctly', 'Close', {
        duration: 3000,
        horizontalPosition: 'end',
        verticalPosition: 'bottom',
        panelClass: ['warn-snackbar']
      });
    }
  }
}

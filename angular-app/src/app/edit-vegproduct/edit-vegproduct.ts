import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatHint } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Vegproduct } from '../vegproduct';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-edit-vegproduct',
  imports: [ReactiveFormsModule, MatFormFieldModule, MatInputModule, MatButtonModule, RouterLink, MatSnackBarModule, CommonModule, MatHint, MatIconModule, MatProgressSpinnerModule],
  templateUrl: './edit-vegproduct.html',
  styleUrl: './edit-vegproduct.css',
})
export class EditVegproduct implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  vegProduct = inject(Vegproduct);
  router = inject(Router);
  snackBar = inject(MatSnackBar);
  activatedRoute = inject(ActivatedRoute);
  cdr = inject(ChangeDetectorRef);

  productId: number | null = null;
  isLoading = true;

  vegProductForm = this.formBuilder.group({
    name: [''],
    price: ['']
  });

  ngOnInit() {
    // Get the product ID from the route parameters
    this.activatedRoute.paramMap.subscribe(params => {
      const id = params.get('id');
      console.log('Route param ID:', id);
      if (id) {
        this.productId = parseInt(id, 10);
        console.log('Parsed product ID:', this.productId);
        this.loadProduct();
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

  loadProduct() {
    console.log('loadProduct called, productId:', this.productId);
    if (!this.productId) {
      console.log('No productId, returning');
      return;
    }

    // Get all products and find the one with matching ID
    this.vegProduct.getVegproducts().subscribe({
      next: (products) => {
        console.log('Received products:', products);
        const product = products.find(p => p.id === this.productId);
        console.log('Found product:', product);
        if (product) {
          // Populate the form with product data
          this.vegProductForm.patchValue({
            name: product.name,
            price: product.price.toFixed(2)
          });
          console.log('Form populated');
        } else {
          console.log('Product not found with ID:', this.productId);
          this.snackBar.open('Product not found', 'Close', {
            duration: 3000,
            horizontalPosition: 'end',
            verticalPosition: 'bottom',
            panelClass: ['error-snackbar']
          });
          this.router.navigate(['/products']);
        }
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.log('Error loading products:', error);
        let errorMessage = 'Failed to load product';
        if (error.error && error.error.message) {
          errorMessage = error.error.message;
        }

        this.snackBar.open(`✗ Error: ${errorMessage}`, 'Close', {
          duration: 5000,
          horizontalPosition: 'end',
          verticalPosition: 'bottom',
          panelClass: ['error-snackbar']
        });
        this.router.navigate(['/products']);
        this.isLoading = false;
        this.cdr.detectChanges();
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
    if (this.vegProductForm.valid && this.productId) {
      const formValue = this.vegProductForm.value;
      const vegProductData: any = {
        name: formValue.name,
        price: parseFloat(formValue.price || '0')
      };
      
      this.vegProduct.updateVegproduct(this.productId, vegProductData).subscribe({
        next: (response) => {
          // Show success message
          this.snackBar.open(`✓ Product "${formValue.name}" updated successfully!`, 'Close', {
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

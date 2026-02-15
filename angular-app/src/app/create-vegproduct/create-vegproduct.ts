import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatHint } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { Router, RouterLink } from '@angular/router';
import { Vegproduct } from '../vegproduct';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-create-vegproduct',
  imports: [ReactiveFormsModule, MatFormFieldModule, MatInputModule, MatButtonModule, RouterLink, MatSnackBarModule, CommonModule, MatHint, MatIconModule],
  templateUrl: './create-vegproduct.html',
  styleUrl: './create-vegproduct.css',
})
export class CreateVegproduct {
  private readonly formBuilder = inject(FormBuilder);
  vegProduct = inject(Vegproduct);
  router = inject(Router);
  snackBar = inject(MatSnackBar);

  vegProductForm = this.formBuilder.group({
    name: [''],
    price: ['']
  });

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
        price: parseFloat(formValue.price || '0')
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

import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatHint } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { Router, RouterLink } from '@angular/router';
import { ProductService } from '../features/products/services/product.service';
import { CategoryService } from '../features/categories/services/category.service';
import { CommonModule } from '@angular/common';
import { VegCategory, VegProductCreateUpdateDto } from '../shared/models/entities';
import { NotificationService } from '../shared/services/notification.service';
import { ProductImageUploadComponent } from '../shared/components/product-image-upload/product-image-upload.component';
import { ProductImage } from '../shared/models/product-image';

@Component({
  selector: 'app-create-vegproduct',
  imports: [ReactiveFormsModule, MatFormFieldModule, MatInputModule, MatSelectModule, MatButtonModule, RouterLink, CommonModule, MatHint, MatIconModule, ProductImageUploadComponent],
  templateUrl: './create-vegproduct.html',
  styleUrl: './create-vegproduct.css',
})
export class CreateVegproduct implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  productService = inject(ProductService);
  categoryService = inject(CategoryService);
  router = inject(Router);
  notificationService = inject(NotificationService);

  categories: VegCategory[] = [];
  
  // Use a dummy product ID (0) for new products - image component handles this
  tempProductId = signal<number>(0);
  selectedImages = signal<File[]>([]);

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
    this.categoryService.getAll().subscribe({
      next: (data) => {
        this.categories = data;
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
      const productData: VegProductCreateUpdateDto = {
        name: formValue.name!,
        price: parseFloat(formValue.price || '0'),
        description: formValue.description || undefined,
        stockQuantity: formValue.stockQuantity || 0,
        idCategory: formValue.idCategory || null
      };
      
      this.productService.create(productData).subscribe({
        next: (response) => {
          const productName = formValue.name ?? undefined;
          this.notificationService.created('Product', productName);
          
          // Store the created product ID for image upload
          if (response && response.id) {
            this.tempProductId.set(response.id);
          }
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
          
          this.notificationService.saveError('create', errorMessage);
        }
      });
    } else {
      this.notificationService.validationError();
    }
  }

  /**
   * Handle when images have been updated
   * Navigate to products list after image upload is complete
   */
  onImagesUpdated(images: ProductImage[]) {
    // Optional: You can do something with the images here if needed
    // For now, just navigate after a short delay to allow user to see success message
    setTimeout(() => {
      this.router.navigate(['/products']);
    }, 1500);
  }
}

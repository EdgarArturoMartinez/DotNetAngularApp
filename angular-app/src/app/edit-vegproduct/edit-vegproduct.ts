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
import { ProductService } from '../features/products/services/product.service';
import { CategoryService } from '../features/categories/services/category.service';
import { CommonModule } from '@angular/common';
import { VegCategory, VegProduct, VegProductCreateUpdateDto } from '../shared/models/entities';
import { NotificationService } from '../shared/services/notification.service';
import { ProductImageUploadComponent } from '../shared/components/product-image-upload/product-image-upload.component';
import { ProductImage } from '../shared/models/product-image';

@Component({
  selector: 'app-edit-vegproduct',
  imports: [ReactiveFormsModule, MatFormFieldModule, MatInputModule, MatSelectModule, MatButtonModule, RouterLink, CommonModule, MatHint, MatIconModule, MatProgressSpinnerModule, ProductImageUploadComponent],
  templateUrl: './edit-vegproduct.html',
  styleUrl: './edit-vegproduct.css',
})
export class EditVegproduct implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  productService = inject(ProductService);
  categoryService = inject(CategoryService);
  router = inject(Router);
  notificationService = inject(NotificationService);
  activatedRoute = inject(ActivatedRoute);
  cdr = inject(ChangeDetectorRef);

  productId: number | null = null;
  isLoading = true;
  categories: VegCategory[] = [];

  vegProductForm = this.formBuilder.group({
    name: ['', Validators.required],
    price: ['', Validators.required],
    description: [''],
    stockQuantity: [0, Validators.required],
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
          this.notificationService.error('Invalid product ID');
          this.router.navigate(['/products']);
        }
      } else {
        console.log('No ID found in route');
        this.notificationService.error('Invalid product ID');
        this.router.navigate(['/products']);
      }
    });
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

  loadProduct() {
    console.log('loadProduct called, productId:', this.productId);
    if (!this.productId) {
      console.log('No productId, returning');
      return;
    }

    this.isLoading = true;
    
    this.productService.getById(this.productId).subscribe({
      next: (product) => {
        console.log('SUCCESS! Product loaded:', product);
        console.log('Product stockQuantity from API:', product.stockQuantity);
        
        this.vegProductForm.patchValue({
          name: product.name,
          price: product.price.toFixed(2),
          description: product.description || '',
          stockQuantity: product.stockQuantity || 0,
          idCategory: product.idCategory || null
        });
        
        console.log('Form populated with:', this.vegProductForm.value);
        console.log('Form stockQuantity value:', this.vegProductForm.get('stockQuantity')?.value);
        
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

        this.notificationService.error(errorMessage);
        
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
        this.notificationService.validationError('Please fill in all required fields');
        return;
      }
      
      const productData: VegProductCreateUpdateDto = {
        name: String(formValue.name).trim(),
        price: parseFloat(String(formValue.price)),
        description: formValue.description || undefined,
        stockQuantity: formValue.stockQuantity || 0,
        idCategory: formValue.idCategory || null
      };
      
      console.log('Submitting product data:', productData);
      
      this.productService.update(this.productId, productData).subscribe({
        next: () => {
          const productName = formValue.name ?? undefined;
          this.notificationService.updated('Product', productName);
          
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
          
          this.notificationService.saveError('update', errorMessage);
        }
      });
    } else {
      console.log('Form invalid or no product ID');
      this.notificationService.validationError();
    }
  }

  /**
   * Handle when images have been updated
   */
  onImagesUpdated(images: ProductImage[]) {
    // Optional: You can do something with the images here if needed
    // For now, just acknowledge that images were updated
    console.log('Product images updated:', images);
  }
}

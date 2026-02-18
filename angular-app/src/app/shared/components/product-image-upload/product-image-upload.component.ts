import { Component, Input, Output, EventEmitter, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { ProductImage, ProductImageCreateUpdate, ProductImageType, ImageValidationResult } from '../../models/product-image';
import { ProductImageService } from '../../services/product-image.service';

@Component({
  selector: 'app-product-image-upload',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatSelectModule,
    MatProgressBarModule,
    MatSnackBarModule
  ],
  templateUrl: './product-image-upload.component.html',
  styleUrl: './product-image-upload.component.css'
})
export class ProductImageUploadComponent {
  @Input() productId!: number;
  @Output() imagesUpdated = new EventEmitter<ProductImage[]>();

  private imageService = inject(ProductImageService);
  private snackBar = inject(MatSnackBar);

  // State signals
  productImages = signal<ProductImage[]>([]);
  isLoading = signal(false);
  uploadingImageType = signal<ProductImageType | null>(null);

  // Main Image
  mainImageUrl = signal('');
  mainImageError = signal('');
  mainImagePreview = signal('');

  // Mobile Image
  mobileImageUrl = signal('');
  mobileImageError = signal('');
  mobileImagePreview = signal('');

  // Gallery Images
  galleryImages = signal<{ url: string; preview: string; error: string }[]>([]);
  galleryImageInput = signal('');

  ProductImageType = ProductImageType;

  ngOnInit() {
    if (this.productId) {
      this.loadProductImages();
    }
  }

  /**
   * Load all images for the product
   */
  loadProductImages() {
    this.isLoading.set(true);
    this.imageService.getProductImages(this.productId).subscribe({
      next: (images: ProductImage[]) => {
        this.productImages.set(images);
        this.populateFormFields(images);
        this.isLoading.set(false);
      },
      error: (err: any) => {
        console.error('Error loading images:', err);
        this.snackBar.open('Error loading product images', 'Close', { duration: 3000, panelClass: 'error-snackbar' });
        this.isLoading.set(false);
      }
    });
  }

  /**
   * Populate form fields with existing images
   */
  private populateFormFields(images: ProductImage[]) {
    const mainImage = images.find(img => img.imageType === ProductImageType.Main);
    const mobileImage = images.find(img => img.imageType === ProductImageType.Mobile);
    const galleryImages = images.filter(img => img.imageType === ProductImageType.Gallery);

    if (mainImage) {
      this.mainImageUrl.set(mainImage.imageUrl);
      this.mainImagePreview.set(mainImage.imageUrl);
    }

    if (mobileImage) {
      this.mobileImageUrl.set(mobileImage.imageUrl);
      this.mobileImagePreview.set(mobileImage.imageUrl);
    }

    if (galleryImages.length > 0) {
      this.galleryImages.set(
        galleryImages.map(img => ({
          url: img.imageUrl,
          preview: img.imageUrl,
          error: ''
        }))
      );
    }
  }

  /**
   * Handle main image URL input
   */
  onMainImageUrlChange(imageUrl: string) {
    this.mainImageError.set('');
    this.mainImageUrl.set(imageUrl);
    
    if (imageUrl) {
      this.mainImagePreview.set(imageUrl);
      this.validateMainImage(imageUrl);
    }
  }

  /**
   * Validate main image dimensions and aspect ratio
   */
  private validateMainImage(imageUrl: string) {
    const imageData: ProductImageCreateUpdate = {
      imageUrl,
      imageType: ProductImageType.Main,
      displayOrder: 0,
      width: 1000, // Recommended
      height: 800  // Recommended
    };

    this.imageService.validateImage(this.productId, imageData).subscribe({
      next: (result: ImageValidationResult) => {
        if (!result.valid) {
          this.mainImageError.set(result.message || 'Image validation failed');
        }
      },
      error: (err: any) => {
        console.error('Validation error:', err);
      }
    });
  }

  /**
   * Save main image
   */
  saveMainImage() {
    if (!this.mainImageUrl().trim()) {
      this.mainImageError.set('Please enter an image URL');
      return;
    }

    this.uploadingImageType.set(ProductImageType.Main);
    const imageData: ProductImageCreateUpdate = {
      imageUrl: this.mainImageUrl(),
      imageType: ProductImageType.Main,
      displayOrder: 0
    };

    this.imageService.createImage(this.productId, imageData).subscribe({
      next: (createdImage: ProductImage) => {
        this.snackBar.open('Main image saved successfully!', 'Close', { duration: 3000, panelClass: 'success-snackbar' });
        this.loadProductImages();
        this.uploadingImageType.set(null);
      },
      error: (err: any) => {
        console.error('Error saving main image:', err);
        this.mainImageError.set(err.error?.message || 'Error saving image');
        this.snackBar.open('Error saving main image', 'Close', { duration: 3000, panelClass: 'error-snackbar' });
        this.uploadingImageType.set(null);
      }
    });
  }

  /**
   * Replace/remove main image
   */
  removeMainImage() {
    const images = this.productImages();
    const mainImage = images.find(img => img.imageType === ProductImageType.Main);
    
    if (mainImage) {
      this.imageService.deleteImage(this.productId, mainImage.id).subscribe({
        next: () => {
          this.mainImageUrl.set('');
          this.mainImagePreview.set('');
          this.mainImageError.set('');
          this.snackBar.open('Main image removed', 'Close', { duration: 3000 });
          this.loadProductImages();
        },
        error: (err) => {
          console.error('Error removing image:', err);
          this.snackBar.open('Error removing image', 'Close', { duration: 3000, panelClass: 'error-snackbar' });
        }
      });
    }
  }

  /**
   * Handle mobile image URL input
   */
  onMobileImageUrlChange(imageUrl: string) {
    this.mobileImageError.set('');
    this.mobileImageUrl.set(imageUrl);
    
    if (imageUrl) {
      this.mobileImagePreview.set(imageUrl);
      this.validateMobileImage(imageUrl);
    }
  }

  /**
   * Validate mobile image dimensions and aspect ratio
   */
  private validateMobileImage(imageUrl: string) {
    const imageData: ProductImageCreateUpdate = {
      imageUrl,
      imageType: ProductImageType.Mobile,
      displayOrder: 1,
      width: 600, // Recommended
      height: 600 // Recommended
    };

    this.imageService.validateImage(this.productId, imageData).subscribe({
      next: (result: ImageValidationResult) => {
        if (!result.valid) {
          this.mobileImageError.set(result.message || 'Image validation failed');
        }
      },
      error: (err: any) => {
        console.error('Validation error:', err);
      }
    });
  }

  /**
   * Save mobile image
   */
  saveMobileImage() {
    if (!this.mobileImageUrl().trim()) {
      this.mobileImageError.set('Please enter an image URL');
      return;
    }

    this.uploadingImageType.set(ProductImageType.Mobile);
    const imageData: ProductImageCreateUpdate = {
      imageUrl: this.mobileImageUrl(),
      imageType: ProductImageType.Mobile,
      displayOrder: 1
    };

    this.imageService.createImage(this.productId, imageData).subscribe({
      next: (createdImage: ProductImage) => {
        this.snackBar.open('Mobile image saved successfully!', 'Close', { duration: 3000, panelClass: 'success-snackbar' });
        this.loadProductImages();
        this.uploadingImageType.set(null);
      },
      error: (err: any) => {
        console.error('Error saving mobile image:', err);
        this.mobileImageError.set(err.error?.message || 'Error saving image');
        this.snackBar.open('Error saving mobile image', 'Close', { duration: 3000, panelClass: 'error-snackbar' });
        this.uploadingImageType.set(null);
      }
    });
  }

  /**
   * Remove mobile image
   */
  removeMobileImage() {
    const images = this.productImages();
    const mobileImage = images.find(img => img.imageType === ProductImageType.Mobile);
    
    if (mobileImage) {
      this.imageService.deleteImage(this.productId, mobileImage.id).subscribe({
        next: () => {
          this.mobileImageUrl.set('');
          this.mobileImagePreview.set('');
          this.mobileImageError.set('');
          this.snackBar.open('Mobile image removed', 'Close', { duration: 3000 });
          this.loadProductImages();
        },
        error: (err) => {
          console.error('Error removing image:', err);
          this.snackBar.open('Error removing image', 'Close', { duration: 3000, panelClass: 'error-snackbar' });
        }
      });
    }
  }

  /**
   * Add gallery image
   */
  addGalleryImage() {
    if (!this.galleryImageInput().trim()) {
      this.snackBar.open('Please enter an image URL', 'Close', { duration: 3000, panelClass: 'warn-snackbar' });
      return;
    }

    const currentGallery = this.galleryImages();
    if (currentGallery.length >= 8) {
      this.snackBar.open('Maximum 8 gallery images allowed', 'Close', { duration: 3000, panelClass: 'warn-snackbar' });
      return;
    }

    this.uploadingImageType.set(ProductImageType.Gallery);
    const imageUrl = this.galleryImageInput();
    const displayOrder = currentGallery.length + 2;

    const imageData: ProductImageCreateUpdate = {
      imageUrl,
      imageType: ProductImageType.Gallery,
      displayOrder
    };

    this.imageService.createImage(this.productId, imageData).subscribe({
      next: (createdImage) => {
        this.snackBar.open('Gallery image added successfully!', 'Close', { duration: 3000, panelClass: 'success-snackbar' });
        this.galleryImageInput.set('');
        this.loadProductImages();
        this.uploadingImageType.set(null);
      },
      error: (err) => {
        console.error('Error adding gallery image:', err);
        this.snackBar.open('Error adding gallery image', 'Close', { duration: 3000, panelClass: 'error-snackbar' });
        this.uploadingImageType.set(null);
      }
    });
  }

  /**
   * Remove gallery image
   */
  removeGalleryImage(index: number) {
    const images = this.productImages();
    const galleryImages = images.filter(img => img.imageType === ProductImageType.Gallery);
    const imageToRemove = galleryImages[index];

    if (imageToRemove) {
      this.imageService.deleteImage(this.productId, imageToRemove.id).subscribe({
        next: () => {
          this.snackBar.open('Gallery image removed', 'Close', { duration: 3000 });
          this.loadProductImages();
        },
        error: (err) => {
          console.error('Error removing image:', err);
          this.snackBar.open('Error removing image', 'Close', { duration: 3000, panelClass: 'error-snackbar' });
        }
      });
    }
  }
}

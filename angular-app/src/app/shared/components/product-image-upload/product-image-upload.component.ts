import { Component, Input, Output, EventEmitter, inject, signal, ViewChild } from '@angular/core';
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

  // Main Image - File based
  mainImageFile = signal<File | null>(null);
  mainImageFileName = signal('');
  mainImageError = signal('');
  mainImagePreview = signal('');

  // Mobile Image - File based
  mobileImageFile = signal<File | null>(null);
  mobileImageFileName = signal('');
  mobileImageError = signal('');
  mobileImagePreview = signal('');

  // Gallery Images - File based
  galleryImages = signal<{ file?: File; url: string; preview: string; error: string }[]>([]);

  // Template references to file inputs
  @ViewChild('mainImageInput') mainImageInput: any;
  @ViewChild('mobileImageInput') mobileImageInput: any;
  @ViewChild('galleryImageInput') galleryImageInput: any;

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
      this.mainImagePreview.set(mainImage.imageUrl);
    }

    if (mobileImage) {
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
   * Handle main image file selection
   */
  onMainImageSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    
    if (!file) return;

    this.mainImageError.set('');
    this.mainImageFileName.set(file.name);
    this.mainImageFile.set(file);
    
    this.createImagePreview(file, (preview) => {
      this.mainImagePreview.set(preview);
      this.validateImageFile(file, ProductImageType.Main);
    });
  }

  /**
   * Handle mobile image file selection
   */
  onMobileImageSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    
    if (!file) return;

    this.mobileImageError.set('');
    this.mobileImageFileName.set(file.name);
    this.mobileImageFile.set(file);
    
    this.createImagePreview(file, (preview) => {
      this.mobileImagePreview.set(preview);
      this.validateImageFile(file, ProductImageType.Mobile);
    });
  }

  /**
   * Handle gallery image file selection
   */
  onGalleryImageSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    
    if (!file) return;

    const currentGallery = this.galleryImages();
    if (currentGallery.length >= 8) {
      this.snackBar.open('Maximum 8 gallery images allowed', 'Close', { 
        duration: 3000, 
        panelClass: 'warn-snackbar' 
      });
      return;
    }

    this.createImagePreview(file, (preview) => {
      this.galleryImages.update(arr => [...arr, {
        file,
        url: '',
        preview,
        error: ''
      }]);
    });
    
    // Reset input
    input.value = '';
  }

  /**
   * Create image preview from File using FileReader API
   */
  private createImagePreview(file: File, callback: (preview: string) => void): void {
    const reader = new FileReader();
    reader.onload = (e) => {
      const preview = e.target?.result as string;
      callback(preview);
    };
    reader.onerror = () => {
      this.snackBar.open('Error reading file', 'Close', { 
        duration: 3000, 
        panelClass: 'error-snackbar' 
      });
    };
    reader.readAsDataURL(file);
  }

  /**
   * Validate image file type and size
   */
  private validateImageFile(file: File, imageType: ProductImageType): void {
    const allowedExtensions = ['image/jpeg', 'image/png', 'image/gif', 'image/webp'];
    const mainImageMaxSize = 5 * 1024 * 1024; // 5MB
    const otherImageMaxSize = 4 * 1024 * 1024; // 4MB
    const maxSize = imageType === ProductImageType.Main ? mainImageMaxSize : otherImageMaxSize;

    if (!allowedExtensions.includes(file.type)) {
      const errorMsg = `Invalid file type. Allowed: ${allowedExtensions.join(', ')}`;
      if (imageType === ProductImageType.Main) {
        this.mainImageError.set(errorMsg);
      } else if (imageType === ProductImageType.Mobile) {
        this.mobileImageError.set(errorMsg);
      }
      return;
    }

    if (file.size > maxSize) {
      const maxSizeMB = maxSize / (1024 * 1024);
      const errorMsg = `File size exceeds ${maxSizeMB}MB limit`;
      if (imageType === ProductImageType.Main) {
        this.mainImageError.set(errorMsg);
      } else if (imageType === ProductImageType.Mobile) {
        this.mobileImageError.set(errorMsg);
      }
      return;
    }
  }

  /**
   * Save main image file
   */
  saveMainImage() {
    const file = this.mainImageFile();
    
    if (!file) {
      this.mainImageError.set('Please select an image file');
      return;
    }

    this.uploadingImageType.set(ProductImageType.Main);
    
    this.imageService.uploadImageFile(
      this.productId,
      file,
      ProductImageType.Main,
      0
    ).subscribe({
      next: (createdImage: ProductImage) => {
        this.snackBar.open('Main image uploaded successfully!', 'Close', { 
          duration: 3000, 
          panelClass: 'success-snackbar' 
        });
        this.mainImageFile.set(null);
        this.mainImageFileName.set('');
        this.mainImageError.set('');
        this.loadProductImages();
        this.uploadingImageType.set(null);
      },
      error: (err: any) => {
        console.error('Error uploading main image:', err);
        this.mainImageError.set(err.error?.message || 'Error uploading image');
        this.snackBar.open('Error uploading main image', 'Close', { 
          duration: 3000, 
          panelClass: 'error-snackbar' 
        });
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
          this.mainImageFile.set(null);
          this.mainImageFileName.set('');
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
   * Save mobile image file
   */
  saveMobileImage() {
    const file = this.mobileImageFile();
    
    if (!file) {
      this.mobileImageError.set('Please select an image file');
      return;
    }

    this.uploadingImageType.set(ProductImageType.Mobile);
    
    this.imageService.uploadImageFile(
      this.productId,
      file,
      ProductImageType.Mobile,
      1
    ).subscribe({
      next: (createdImage: ProductImage) => {
        this.snackBar.open('Mobile image uploaded successfully!', 'Close', { 
          duration: 3000, 
          panelClass: 'success-snackbar' 
        });
        this.mobileImageFile.set(null);
        this.mobileImageFileName.set('');
        this.mobileImageError.set('');
        this.loadProductImages();
        this.uploadingImageType.set(null);
      },
      error: (err: any) => {
        console.error('Error uploading mobile image:', err);
        this.mobileImageError.set(err.error?.message || 'Error uploading image');
        this.snackBar.open('Error uploading mobile image', 'Close', { 
          duration: 3000, 
          panelClass: 'error-snackbar' 
        });
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
          this.mobileImageFile.set(null);
          this.mobileImageFileName.set('');
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
   * Add gallery image file
   */
  addGalleryImage(index: number) {
    const galleryImages = this.galleryImages();
    if (index < 0 || index >= galleryImages.length) return;

    const imageData = galleryImages[index];
    const file = imageData.file;
    
    if (!file) {
      this.snackBar.open('Please select an image file', 'Close', { 
        duration: 3000, 
        panelClass: 'warn-snackbar' 
      });
      return;
    }

    this.uploadingImageType.set(ProductImageType.Gallery);
    const displayOrder = index + 2; // Main=0, Mobile=1, Gallery=2+

    this.imageService.uploadImageFile(
      this.productId,
      file,
      ProductImageType.Gallery,
      displayOrder
    ).subscribe({
      next: (createdImage: ProductImage) => {
        this.snackBar.open('Gallery image added successfully!', 'Close', { 
          duration: 3000, 
          panelClass: 'success-snackbar' 
        });
        this.loadProductImages();
        this.uploadingImageType.set(null);
      },
      error: (err: any) => {
        console.error('Error adding gallery image:', err);
        this.snackBar.open('Error adding gallery image', 'Close', { 
          duration: 3000, 
          panelClass: 'error-snackbar' 
        });
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

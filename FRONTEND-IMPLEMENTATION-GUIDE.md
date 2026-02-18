# Product Image Upload - Frontend Implementation Guide

## ✅ Completed Backend Implementation

The file upload architecture is now fully implemented on the backend:

### File Upload Service (`FileUploadService.cs`)
- Naming strategy: `product_{productId}_{imageType}_{timestamp}_{random}.{ext}`
- File storage: `wwwroot/images/` folder
- Validation: File type, size limits (5MB main, 4MB mobile/gallery)
- Automatic cleanup when images deleted

### API Endpoints
```
POST /api/products/{productId}/images/upload
Content-Type: multipart/form-data
Parameters:
  - file: File (binary)
  - imageType: Integer (0=Main, 1=Mobile, 2=Gallery)
  - displayOrder: Integer
```

### Frontend Service Update
- ✅ `ProductImageService.uploadImageFile()` method added
- ✅ Fixed API URL to use correct port (7020)
- Ready to handle file uploads

---

## 📋 Remaining Frontend Work

### 1. Update ` create-vegproduct.html` Form Structure
**Current Issue:** Image manager shows AFTER product creation
**Required Change:** Move to BEFORE save button

**Structure:**
```html
<form>
  <!-- Product Basic Info -->
  <div class="form-field">
    <mat-form-field>Product Name</mat-form-field>
  </div>
  <!-- ... other fields ... -->

  <!-- IMAGE UPLOAD SECTION (NEW POSITION) -->
  <div class="image-section-divider"></div>
  <div class="form-section-header">
    <h3>Product Images</h3>
    <span>(Optional but recommended)</span>
  </div>
  <app-product-image-upload 
    [productId]="tempProductId()" 
    (imagesUpdated)="onImagesUpdated($event)">
  </app-product-image-upload>
 
  <!-- FORM ACTIONS (AFTER IMAGES) -->
  <div class="form-actions">
    <button type="submit">Create Product & Save</button>
    <button routerLink="/products">Cancel</button>
  </div>
</form>
```

### 2. Update `ProductImageUploadComponent` to Use File Inputs

**Replace URL input fields with file inputs:**

#### For Main Image Section:
```html
<div class="file-input-group">
  <input 
    #mainImageInput
    type="file" 
    accept="image/*"
    (change)="onMainImageSelected($event)"
    [disabled]="uploadingImageType() !== null">
  
  <button 
    type="button"
    color="primary"
    (click)="mainImageInput.click()"
    [disabled]="uploadingImageType() !== null">
    <mat-icon>upload_file</mat-icon>
    Choose Image
  </button>
  
  <span *ngIf="mainImageFileName()">
    {{ mainImageFileName() }}
  </span>
</div>
```

#### For Mobile Image Section:
Same pattern with `mobileImageInput` template reference variable

#### For Gallery Images Section:
```html
<div class="add-gallery-section">
  <input 
    #galleryImageInput
    type="file" 
    accept="image/*"
    (change)="onGalleryImageSelected($event)"
    [disabled]="galleryImages().length >= 8 || uploadingImageType() !== null">
  
  <button 
    type="button"
    (click)="galleryImageInput.click()"
    [disabled]="galleryImages().length >= 8 || uploadingImageType() !== null">
    <mat-icon>add_photo_alternate</mat-icon>
    Select Gallery Image
  </button>
</div>
```

### 3. Update Component TypeScript Logic

**Add new signals for file handling:**
```typescript
// Signals for file names
mainImageFileName = signal('');
mobileImageFileName = signal('');

// Signal for selected gallery files
galleryFileInputs = signal<{ file: File, preview: string }[]>([]);

// Signal for isInForm input (to detect if component is inside form)
@Input() isInForm = signal(false);
```

**Add file selection methods:**
```typescript
onMainImageSelected(event: Event): void {
  const input = event.target as HTMLInputElement;
  const file = input.files?.[0];
  if (file) {
    this.mainImageFileName.set(file.name);
    this.createImagePreview(file, (preview) => {
      this.mainImagePreview.set(preview);
      this.validateImageFile(file);
    });
  }
}

onMobileImageSelected(event: Event): void {
  const input = event.target as HTMLInputElement;
  const file = input.files?.[0];
  if (file) {
    this.mobileImageFileName.set(file.name);
    this.createImagePreview(file, (preview) => {
      this.mobileImagePreview.set(preview);
      this.validateImageFile(file);
    });
  }
}

onGalleryImageSelected(event: Event): void {
  const input = event.target as HTMLInputElement;
  const file = input.files?.[0];
  if (file && this.galleryImages().length < 8) {
    this.createImagePreview(file, (preview) => {
      this.galleryImages.update(arr => [...arr, { 
        url: '', 
        preview, 
        file,
        error: '' 
      }]);
    });
  }
}

private createImagePreview(file: File, callback: (preview: string) => void): void {
  const reader = new FileReader();
  reader.onload = (e) => {
    callback(e.target?.result as string);
  };
  reader.readAsDataURL(file);
}

private validateImageFile(file: File): void {
  // Client-side validation
  const maxSize = 5 * 1024 * 1024; // 5MB
  if (file.size > maxSize) {
    this.mainImageError.set('File size exceeds 5MB');
    return;
  }
  
  if (!file.type.startsWith('image/')) {
    this.mainImageError.set('Please select an image file');
    return;
  }
  
  // Server will do aspect ratio validation
}
```

**Update save methods to upload files:**
```typescript
saveMainImage(): void {
  // Get file from input (need template reference)
  const input = this.mainImageInput?.nativeElement;
  const file = input?.files?.[0];
  
  if (!file) {
    this.mainImageError.set('Please select a file');
    return;
  }

  this.uploadingImageType.set(ProductImageType.Main);
  
  this.imageService.uploadImageFile(
    this.productId,
    file,
    ProductImageType.Main,
    0
  ).subscribe({
    next: (createdImage) => {
      this.snackBar.open('Main image uploaded!', 'Close', { 
        duration: 3000, 
        panelClass: 'success-snackbar' 
      });
      this.loadProductImages();
      this.uploadingImageType.set(null);
    },
    error: (err: any) => {
      this.mainImageError.set(err.error?.message || 'Upload failed');
      this.snackBar.open('Error uploading image', 'Close', { 
        duration: 3000, 
        panelClass: 'error-snackbar' 
      });
      this.uploadingImageType.set(null);
    }
  });
}
```

### 4. Update Component Styling

**Add file input styling to CSS:**
```css
.file-input {
  display: none;
}

.file-input-group {
  display: flex;
  gap: 12px;
  align-items: center;
  margin-bottom: 12px;
}

.file-select-btn {
  min-width: 150px;
  height: 44px;
  white-space: nowrap;
}

.filename {
  font-size: 13px;
  color: #7f8c8d;
  font-style: italic;
  padding: 8px;
  background-color: #f5f5f5;
  border-radius: 4px;
  max-width: 300px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

@media (max-width: 768px) {
  .file-input-group {
    flex-direction: column;
  }
  
  .file-select-btn {
    width: 100%;
  }
  
  .filename {
    width: 100%;
  }
}
```

---

## 🔄 Upload Flow Visualization

### Create New Product
```
1. User fills: Name, Price, Description, Category, Stock
   ↓
2. User optionally selects image files
   ↓
3. Component creates previews (FileReader API)
   ↓
4. User clicks "Create Product & Save"
   ↓
5. [CURRENT FLOW BREAKS HERE - needs refactoring]
   
SHOULD BE:
5. Click "Create Product & Save"
   → Backend creates product
   → Get new product ID
   → Upload image files to /upload endpoint
   → Show success "Product created with images!"
```

### Edit Existing Product
```
1. Load product form (existing flow works fine)
   ↓
2. Images load and display (from relative paths like "images/product_8_main_...")
   ↓
3. User can add/remove/replace images
   ↓
4. Save product updates and image changes
```

---

## 🎯 Implementation Priority

### Phase 1: File Input Conversion (2-3 hours)
- Update component HTML to use file inputs
- Add file selection handlers
- Implement FileReader preview

### Phase 2: Save Methods Refactoring (1-2 hours)
- Update saveMainImage(), saveMobileImage(), addGalleryImage()
- Call `uploadImageFile()` instead of data URL methods
- Handle file upload response

### Phase 3: Form Structure (1 hour)
- Move image section before buttons
- Update CSS for new layout
- Test responsive design

### Phase 4: Testing & Refinement (1-2 hours)
- Test file uploads end-to-end
- Verify image display from relative paths
- Mobile responsiveness testing

---

## 📝 CSS Already Great!

The styling you mentioned is perfect:
- Section headers with badges ✅
- Gradient backgrounds ✅
- Responsive layouts ✅
- Error message displays ✅
- Gallery grid layout ✅

Just needs file input styling added!

---

## ✨ Key Benefits After Completion

1. **No URL Dependencies** - Files locally stored
2. **Copyright Safe** - Your own images
3. **Portable** - Easy to backup or migrate
4. **Smart Naming** - Identify files by product/type
5. **One-Step Upload** - Files uploaded automatically
6. **Professional** - Auto-sized previews and validation

---

## Next Steps

Would you like me to:
1. Completely refactor the HTML & TypeScript files for file uploads?
2. Create a new, simplified component specifically for file handling?
3. Provide specific code snippets for each method?

The backend is 100% ready - just needs frontend UI update!

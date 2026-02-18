# Product Images Implementation Guide

This guide walks through adding product images to your eCommerce application, including image storage, database configuration, API endpoints, and frontend display.

## Overview

We'll implement the following:
1. **Database**: Add `ImageUrl` property to VegProducts entity
2. **Backend**: Create image upload API endpoint with file storage
3. **Frontend**: Display product images in listings, details, and cards
4. **Storage**: Configure wwwroot for static image serving

---

## SCENARIO 1: Add ImageUrl Field to VegProducts

### Backend Implementation

#### Step 1: Update VegProducts Entity
**File**: `DotNetCoreWebApi/DotNetCoreWebApi/Application/Entities/VegProducts.cs`

Add the ImageUrl property:

```csharp
namespace DotNetCoreWebApi.Application.Entities;

public class VegProducts
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }

    // Add this line - stores relative path to image (e.g., "/images/products/carrot.jpg")
    public string? ImageUrl { get; set; }

    public int? IdCategory { get; set; }
    public virtual VegCategory? VegCategory { get; set; }
}
```

#### Step 2: Update VegProductDto (Response)
**File**: `DotNetCoreWebApi/DotNetCoreWebApi/DTOs/VegProductDto.cs`

```csharp
namespace DotNetCoreWebApi.DTOs;

public class VegProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public int StockQuantity { get; set; }

    // Add this line
    public string? ImageUrl { get; set; }

    public int? IdCategory { get; set; }
    public VegCategoryBasicDto? VegCategory { get; set; }
}

public class VegCategoryBasicDto
{
    public int IdCategory { get; set; }
    public string CategoryName { get; set; } = null!;
    public string? Description { get; set; }
}
```

#### Step 3: Update VegProductCreateUpdateDto
**File**: `DotNetCoreWebApi/DotNetCoreWebApi/DTOs/VegProductCreateUpdateDto.cs`

```csharp
namespace DotNetCoreWebApi.DTOs;

public class VegProductCreateUpdateDto
{
    [Required(ErrorMessage = "Product name is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters")]
    public string Name { get; set; } = null!;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative")]
    public int StockQuantity { get; set; }

    // Add this line - optional, stores relative path
    [StringLength(255, ErrorMessage = "ImageUrl cannot exceed 255 characters")]
    public string? ImageUrl { get; set; }

    public int? IdCategory { get; set; }
}
```

#### Step 4: Update ProductService
**File**: `DotNetCoreWebApi/DotNetCoreWebApi/Application/Services/ProductService.cs`

Add ImageUrl mapping in 5 methods:

```csharp
using DotNetCoreWebApi.Application.Entities;
using DotNetCoreWebApi.DTOs;
using DotNetCoreWebApi.Infrastructure.Repositories;

namespace DotNetCoreWebApi.Application.Services;

public interface IProductService
{
    Task<IEnumerable<VegProductDto>> GetAllProductsAsync();
    Task<VegProductDto?> GetProductByIdAsync(int id);
    Task<VegProductDto> CreateProductAsync(VegProductCreateUpdateDto createDto);
    Task<VegProductDto> UpdateProductAsync(int id, VegProductCreateUpdateDto updateDto);
    Task<IEnumerable<VegProductDto>> GetProductsByCategoryAsync(int categoryId);
}

public class ProductService : IProductService
{
    private readonly IRepository<VegProducts> _repository;

    public ProductService(IRepository<VegProducts> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<VegProductDto>> GetAllProductsAsync()
    {
        var products = await _repository.GetAllAsync();
        return products.Select(p => new VegProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            Description = p.Description,
            StockQuantity = p.StockQuantity,
            ImageUrl = p.ImageUrl,  // Add this line
            IdCategory = p.IdCategory,
            VegCategory = p.VegCategory != null ? new VegCategoryBasicDto
            {
                IdCategory = p.VegCategory.IdCategory,
                CategoryName = p.VegCategory.CategoryName,
                Description = p.VegCategory.Description
            } : null
        });
    }

    public async Task<VegProductDto?> GetProductByIdAsync(int id)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product == null) return null;

        return new VegProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Description = product.Description,
            StockQuantity = product.StockQuantity,
            ImageUrl = product.ImageUrl,  // Add this line
            IdCategory = product.IdCategory,
            VegCategory = product.VegCategory != null ? new VegCategoryBasicDto
            {
                IdCategory = product.VegCategory.IdCategory,
                CategoryName = product.VegCategory.CategoryName,
                Description = product.VegCategory.Description
            } : null
        };
    }

    public async Task<VegProductDto> CreateProductAsync(VegProductCreateUpdateDto createDto)
    {
        var product = new VegProducts
        {
            Name = createDto.Name,
            Description = createDto.Description,
            Price = createDto.Price,
            StockQuantity = createDto.StockQuantity,
            ImageUrl = createDto.ImageUrl,  // Add this line
            IdCategory = createDto.IdCategory
        };

        await _repository.AddAsync(product);
        await _repository.SaveChangesAsync();

        return new VegProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Description = product.Description,
            StockQuantity = product.StockQuantity,
            ImageUrl = product.ImageUrl,  // Add this line
            IdCategory = product.IdCategory
        };
    }

    public async Task<VegProductDto> UpdateProductAsync(int id, VegProductCreateUpdateDto updateDto)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product == null) throw new KeyNotFoundException($"Product {id} not found");

        product.Name = updateDto.Name;
        product.Description = updateDto.Description;
        product.Price = updateDto.Price;
        product.StockQuantity = updateDto.StockQuantity;
        product.ImageUrl = updateDto.ImageUrl;  // Add this line
        product.IdCategory = updateDto.IdCategory;

        await _repository.UpdateAsync(product);
        await _repository.SaveChangesAsync();

        return new VegProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Description = product.Description,
            StockQuantity = product.StockQuantity,
            ImageUrl = product.ImageUrl,  // Add this line
            IdCategory = product.IdCategory
        };
    }

    public async Task<IEnumerable<VegProductDto>> GetProductsByCategoryAsync(int categoryId)
    {
        var products = await _repository.GetAllAsync();
        var categoryProducts = products.Where(p => p.IdCategory == categoryId);

        return categoryProducts.Select(p => new VegProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            Description = p.Description,
            StockQuantity = p.StockQuantity,
            ImageUrl = p.ImageUrl,  // Add this line
            IdCategory = p.IdCategory,
            VegCategory = p.VegCategory != null ? new VegCategoryBasicDto
            {
                IdCategory = p.VegCategory.IdCategory,
                CategoryName = p.VegCategory.CategoryName,
                Description = p.VegCategory.Description
            } : null
        });
    }
}
```

#### Step 5: Create Database Migration
Navigate to the API project and create a migration:

```powershell
cd c:\Arthur\Development\2026\DotNetAngularApp\DotNetCoreWebApi\DotNetCoreWebApi

# Create migration
dotnet ef migrations add AddImageUrlToVegProducts

# Apply migration to database
dotnet ef database update
```

Expected output:
```
Build started...
Build succeeded.
To undo this action, use 'ef migrations remove'
Done. To create a complete seed file see the output -v option.
```

---

### Frontend Implementation

#### Step 6: Update vegproduct.ts Interface
**File**: `angular-app/src/app/vegproduct.ts`

```typescript
export interface vegproduct {
  id: number;
  name: string;
  price: number;
  description?: string;
  stockQuantity: number;
  imageUrl?: string;  // Add this line
  idCategory?: number;
}
```

#### Step 7: Update vegproduct.models.ts
**File**: `angular-app/src/app/vegproduct.models.ts`

```typescript
export interface VegProductCreation {
  name: string;
  description?: string;
  price: number;
  stockQuantity: number;
  imageUrl?: string;  // Add this line
  idCategory?: number;
}
```

#### Step 8: Update shared entities.ts
**File**: `angular-app/src/app/shared/models/entities.ts`

```typescript
export interface VegProduct {
  id: number;
  name: string;
  price: number;
  description?: string;
  stockQuantity: number;
  imageUrl?: string;  // Add this line
  idCategory?: number;
  vegCategory?: VegCategory;
}

export interface VegProductCreateUpdateDto {
  name: string;
  description?: string;
  price: number;
  stockQuantity: number;
  imageUrl?: string;  // Add this line
  idCategory?: number;
}
```

#### Step 9: Update Index Products Column Configuration
**File**: `angular-app/src/app/index-products/index-products.ts`

```typescript
export class IndexProductsComponent implements OnInit {
  products = signal<VegProduct[]>([]);
  isLoading = signal(false);
  errorMessage = signal('');

  columns: ColumnDefinition[] = [
    { key: 'name', label: 'Product Name', sortable: true, width: '150px' },
    { key: 'price', label: 'Price', type: 'currency', sortable: true, width: '100px' },
    { key: 'description', label: 'Description', width: '200px' },
    { key: 'stockQuantity', label: 'Stock', type: 'number', sortable: true, width: '80px' },
    { key: 'vegCategory.categoryName', label: 'Category', width: '120px' },
    // Add this line - shows thumbnail in table
    {
      key: 'imageUrl',
      label: 'Image',
      type: 'custom',
      width: '80px',
      customTemplate: (item: VegProduct) => {
        if (!item.imageUrl) return 'No image';
        return `<img src="${item.imageUrl}" alt="${item.name}" style="max-width: 60px; max-height: 60px; border-radius: 4px; object-fit: cover;">`;
      }
    }
  ];

  actions: TableAction[] = [
    { label: 'Edit', icon: 'edit', action: 'edit', color: '#4caf50' },
    { label: 'Delete', icon: 'delete', action: 'delete', color: '#f44336' }
  ];

  displayedColumns = ['name', 'price', 'description', 'stockQuantity', 'vegCategory.categoryName', 'imageUrl'];

  // ... rest of component
}
```

#### Step 10: Update Create Product Form
**File**: `angular-app/src/app/create-vegproduct/create-vegproduct.ts`

```typescript
import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { Router } from '@angular/router';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { Vegproduct } from '../vegproduct';
import { VegCategoryService } from '../vegcategory.service';
import { Vegproduct as VegproductService } from '../vegproduct.service';

@Component({
  selector: 'app-create-vegproduct',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatInputModule,
    MatFormFieldModule,
    MatIconModule,
    MatSelectModule,
    MatSnackBarModule
  ],
  templateUrl: './create-vegproduct.html',
  styleUrl: './create-vegproduct.css'
})
export class CreateVegproductComponent implements OnInit {
  private fb = inject(FormBuilder);
  private vegproductService = inject(VegproductService);
  private vegcategoryService = inject(VegCategoryService);
  private router = inject(Router);
  private snackBar = inject(MatSnackBar);

  productForm!: FormGroup;
  categories = signal<VegCategory[]>([]);
  isSubmitting = signal(false);

  ngOnInit() {
    this.productForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]],
      description: [''],
      price: ['', [Validators.required, Validators.min(0.01)]],
      stockQuantity: ['', [Validators.required, Validators.min(0)]],
      imageUrl: ['', [Validators.maxLength(255)]],  // Add this line
      idCategory: [null]
    });

    this.vegcategoryService.GetAllCategories().subscribe({
      next: (res) => {
        this.categories.set(res);
      },
      error: (err) => {
        console.error('Error loading categories:', err);
        this.snackBar.open('Error loading categories', 'Close', { duration: 3000, panelClass: 'error-snackbar' });
      }
    });
  }

  onSubmit() {
    if (this.productForm.invalid) return;

    this.isSubmitting.set(true);
    this.vegproductService.CreateProduct(this.productForm.value).subscribe({
      next: (product: Vegproduct) => {
        this.snackBar.open('Product created successfully!', 'Close', { duration: 3000, panelClass: 'success-snackbar' });
        this.router.navigate(['/admin/products']);
      },
      error: (error) => {
        console.error('Error creating product:', error);
        this.snackBar.open('Error creating product', 'Close', { duration: 3000, panelClass: 'error-snackbar' });
        this.isSubmitting.set(false);
      }
    });
  }

  onCancel() {
    this.router.navigate(['/admin/products']);
  }
}
```

**File**: `angular-app/src/app/create-vegproduct/create-vegproduct.html`

Add image URL field to the form:

```html
<div class="form-wrapper">
  <div class="form-header">
    <h1 class="form-title">Create New Product</h1>
    <p class="form-subtitle">Add a new vegetable or product to your catalog</p>
  </div>

  <div class="form-container">
    <form [formGroup]="productForm" (ngSubmit)="onSubmit()" class="product-form">
      <!-- Name Field -->
      <mat-form-field class="form-field" appearance="outline">
        <mat-label>Product Name *</mat-label>
        <input matInput formControlName="name" placeholder="e.g., Organic Carrot">
        <mat-icon matPrefix>label</mat-icon>
        <mat-hint>3-100 characters</mat-hint>
      </mat-form-field>

      <!-- Price Field -->
      <mat-form-field class="form-field" appearance="outline">
        <mat-label>Price (COP) *</mat-label>
        <input matInput type="number" formControlName="price" placeholder="0.00">
        <mat-icon matPrefix>attach_money</mat-icon>
      </mat-form-field>

      <!-- Stock Quantity Field -->
      <mat-form-field class="form-field" appearance="outline">
        <mat-label>Stock Quantity *</mat-label>
        <input matInput type="number" formControlName="stockQuantity" placeholder="0">
        <mat-icon matPrefix>inventory_2</mat-icon>
      </mat-form-field>

      <!-- Category Select -->
      <mat-form-field class="form-field" appearance="outline">
        <mat-label>Category</mat-label>
        <mat-select formControlName="idCategory">
          <mat-option [value]="null">No Category</mat-option>
          <mat-option *ngFor="let category of categories()" [value]="category.idCategory">
            {{ category.categoryName }}
          </mat-option>
        </mat-select>
        <mat-icon matPrefix>category</mat-icon>
      </mat-form-field>

      <!-- Description Field -->
      <mat-form-field class="form-field" appearance="outline">
        <mat-label>Description</mat-label>
        <textarea matInput formControlName="description" placeholder="Product description..." rows="4"></textarea>
        <mat-icon matPrefix>description</mat-icon>
        <mat-hint>Max 500 characters</mat-hint>
      </mat-form-field>

      <!-- Image URL Field - NEW -->
      <mat-form-field class="form-field" appearance="outline">
        <mat-label>Image URL</mat-label>
        <input matInput formControlName="imageUrl" placeholder="e.g., /images/products/carrot.jpg">
        <mat-icon matPrefix>image</mat-icon>
        <mat-hint>Optional. Relative path to product image</mat-hint>
      </mat-form-field>

      <!-- Form Actions -->
      <div class="form-actions">
        <button mat-raised-button type="button" class="cancel-btn" (click)="onCancel()">
          <mat-icon>arrow_back</mat-icon>
          Cancel
        </button>
        <button mat-raised-button color="primary" type="submit" class="submit-btn" [disabled]="productForm.invalid || isSubmitting()">
          <mat-icon>add</mat-icon>
          {{ isSubmitting() ? 'Creating...' : 'Create Product' }}
        </button>
      </div>
    </form>
  </div>
</div>
```

#### Step 11: Update Edit Product Form
**File**: `angular-app/src/app/edit-vegproduct/edit-vegproduct.ts`

Add imageUrl to FormGroup and in patchValue:

```typescript
ngOnInit() {
  this.route.params.subscribe((params) => {
    const id = params['id'];
    this.loadProduct(id);
  });
}

loadProduct(id: number) {
  this.isLoading.set(true);
  this.vegproductService.GetProductById(id).subscribe({
    next: (product: Vegproduct) => {
      this.productForm.patchValue({
        name: product.name,
        description: product.description,
        price: product.price,
        stockQuantity: product.stockQuantity,
        imageUrl: product.imageUrl,  // Add this line
        idCategory: product.idCategory
      });
      this.isLoading.set(false);
    },
    error: (error) => {
      console.error('Error loading product:', error);
      this.snackBar.open('Error loading product', 'Close', { duration: 3000, panelClass: 'error-snackbar' });
      this.isLoading.set(false);
    }
  });
}
```

Add FormControl in form initialization:

```typescript
this.productForm = this.fb.group({
  name: ['', [Validators.required, Validators.minLength(3)]],
  description: [''],
  price: ['', [Validators.required, Validators.min(0.01)]],
  stockQuantity: ['', [Validators.required, Validators.min(0)]],
  imageUrl: ['', [Validators.maxLength(255)]],  // Add this line
  idCategory: [null]
});
```

**File**: `angular-app/src/app/edit-vegproduct/edit-vegproduct.html`

Add the same Image URL field as in Step 10 (create form).

---

## SCENARIO 2: Image Upload API Endpoint (Optional - For Future)

This creates an endpoint to upload images instead of just storing paths:

### Backend - Add Image Upload Service

**File**: `DotNetCoreWebApi/DotNetCoreWebApi/Application/Services/ImageService.cs`

```csharp
namespace DotNetCoreWebApi.Application.Services;

public interface IImageService
{
    Task<string> UploadProductImageAsync(IFormFile file, string productId);
    Task DeleteProductImageAsync(string imagePath);
}

public class ImageService : IImageService
{
    private readonly IWebHostEnvironment _environment;
    private readonly string _uploadFolder = "images/products";

    public ImageService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string> UploadProductImageAsync(IFormFile file, string productId)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is empty");

        // Validate file type
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(fileExtension))
            throw new ArgumentException("Invalid file type. Only images allowed.");

        // Create directory if it doesn't exist
        var uploadPath = Path.Combine(_environment.WebRootPath, _uploadFolder);
        if (!Directory.Exists(uploadPath))
            Directory.CreateDirectory(uploadPath);

        // Generate unique filename
        var fileName = $"{productId}_{Guid.NewGuid()}{fileExtension}";
        var filePath = Path.Combine(uploadPath, fileName);

        // Save file
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Return relative path for database storage
        return $"/{_uploadFolder}/{fileName}";
    }

    public Task DeleteProductImageAsync(string imagePath)
    {
        if (string.IsNullOrEmpty(imagePath))
            return Task.CompletedTask;

        var filePathToDelete = Path.Combine(_environment.WebRootPath, imagePath.TrimStart('/'));
        
        if (File.Exists(filePathToDelete))
            File.Delete(filePathToDelete);

        return Task.CompletedTask;
    }
}
```

---

## Testing Checklist

### Backend Testing (Postman/curl)

- [ ] **GET** `/api/vegproducts` - Verify ImageUrl field in response
- [ ] **GET** `/api/vegproducts/{id}` - Verify ImageUrl field populated
- [ ] **POST** `/api/vegproducts` - Create product with ImageUrl
  ```json
  {
    "name": "Organic Carrot",
    "price": 5000,
    "stockQuantity": 100,
    "imageUrl": "/images/products/carrot.jpg",
    "idCategory": 1
  }
  ```
- [ ] **PUT** `/api/vegproducts/{id}` - Update product ImageUrl
- [ ] Database has ImageUrl column in VegProducts table

### Frontend Testing

- [ ] Create Product form displays Image URL field
- [ ] Edit Product form displays and loads Image URL
- [ ] Products list table shows image thumbnails
- [ ] Home/eCommerce page displays product images
- [ ] Card view shows product images
- [ ] Images render without errors
- [ ] No validation errors in console (F12)

### Image Display Testing

- [ ] Images load on desktop view (1024px+)
- [ ] Images display correctly on tablet (768px+)
- [ ] Images display correctly on mobile (480px+)
- [ ] Broken image placeholder appears if URL invalid

---

## Database Column Details

| Column | Type | Length | Nullable | Default | Purpose |
|--------|------|--------|----------|---------|---------|
| ImageUrl | nvarchar | 255 | Yes | NULL | Stores relative path to product image |

Example values:
- `/images/products/carrot.jpg`
- `/images/products/tomato-fresh.png`
- `/images/products/lettuce-organic-2024.jpg`

---

## File Storage Structure

```
DotNetCoreWebApi/bin/Debug/net8.0/
wwwroot/
  images/
    products/
      1_a1b2c3d4.jpg
      2_e5f6g7h8.jpg
      3_i9j0k1l2.png
```

---

## Next Steps

1. **Follow SCENARIO 1** to add the ImageUrl field (5-10 minutes)
2. **Update all 5 files** (Entity, DTOs, Service) with ImageUrl mapping
3. **Create and apply migration** to add database column
4. **Update Frontend** entities and forms to display/input imageUrl
5. **Test** complete flow with sample images
6. **For Future**: Implement SCENARIO 2 (image upload endpoint) when file uploads needed

---

## Common Issues & Solutions

### Issue: Migration fails
**Solution**: 
```powershell
# Verify you're in correct directory
cd DotNetCoreWebApi/DotNetCoreWebApi

# Check migrations
dotnet ef migrations list

# If stuck, remove last migration
dotnet ef migrations remove
```

### Issue: Image doesn't display in Angular
**Possible Causes**:
- ImageUrl path is absolute instead of relative (should start with `/`)
- Image file doesn't exist at wwwroot location
- CORS issue if images served from different domain

**Solution**:
- Verify path format: `/images/products/filename.jpg` ✅
- Check file exists in wwwroot folder
- Test with relative path in HTML: `<img src="/images/test.jpg">`

### Issue: Form validation error on imageUrl
**Solution**: Increase maxLength in validators if URLs are long:
```typescript
imageUrl: ['', [Validators.maxLength(500)]]  // Increased from 255
```

---

## Summary

| Item | Time | Difficulty |
|------|------|------------|
| Entity + DTOs update | 2 min | Easy |
| Service method updates | 3 min | Easy |
| Migration creation | 1 min | Easy |
| Frontend interfaces | 2 min | Easy |
| Form fields update | 3 min | Easy |
| **Total** | **~11 min** | **Easy** |

Ready to implement? Start with Step 1 of SCENARIO 1!

# Veggie World E-Commerce - Complete Architecture Documentation

**Last Updated:** February 18, 2026

## 📋 Table of Contents
1. [Overview](#overview)
2. [Backend Architecture](#backend-architecture)
3. [Frontend Architecture](#frontend-architecture)
4. [Adding New Entities](#adding-new-entities)
5. [Testing Guidelines](#testing-guidelines)
6. [Deployment](#deployment)

---

## Overview

This is a full-stack e-commerce application built with:
- **Backend:** .NET 8 Web API with clean architecture
- **Frontend:** Angular 19 with standalone components
- **Database:** SQL Server with Entity Framework Core
- **Architecture:** Clean Architecture with Repository and Service patterns

---

## Backend Architecture

### Project Structure

```
DotNetCoreWebApi/
├── Application/
│   ├── Entities/              # Domain entities
│   ├── Interfaces/            # Service and repository contracts
│   ├── Services/              # Business logic implementation
│   └── DBContext/             # Database context
├── Infrastructure/
│   └── Repositories/          # Repository implementations
├── Controllers/               # API controllers (thin layer)
├── DTOs/                      # Data Transfer Objects
└── Migrations/                # EF Core migrations
```

### Layer Responsibilities

#### 1. Entities (Domain Layer)
- Pure domain models
- Contains only properties and basic validation
- No dependencies on other layers

```csharp
public class VegProducts
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public int StockQuantity { get; set; }
    public int? IdCategory { get; set; }
    public virtual VegCategory? VegCategory { get; set; }
}
```

#### 2. Repositories (Data Access Layer)
- Handles all database operations
- Implements generic repository pattern
- Provides specific methods for complex queries

```csharp
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}
```

#### 3. Services (Business Logic Layer)
- Contains all business logic
- Orchestrates between repositories and controllers
- Handles DTO mapping
- Implements validation rules

```csharp
public interface IVegProductService
{
    Task<IEnumerable<VegProductDto>> GetAllProductsAsync();
    Task<VegProductDto?> GetProductByIdAsync(int id);
    Task<VegProductDto> CreateProductAsync(VegProductCreateUpdateDto dto);
    Task UpdateProductAsync(int id, VegProductCreateUpdateDto dto);
    Task DeleteProductAsync(int id);
}
```

#### 4. Controllers (Presentation Layer)
- Thin layer that handles HTTP concerns
- Delegates to services
- Returns appropriate HTTP status codes

```csharp
[ApiController]
[Route("api/[controller]")]
public class VegProductsController : ControllerBase
{
    private readonly IVegProductService _productService;
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VegProductDto>>> GetAllProducts()
    {
        var products = await _productService.GetAllProductsAsync();
        return Ok(products);
    }
}
```

#### 5. DTOs (Data Transfer Objects)
- Define API contracts
- Prevent over-posting attacks
- Hide internal structure

```csharp
public class VegProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public int StockQuantity { get; set; }
    public int? IdCategory { get; set; }
    public VegCategoryBasicDto? VegCategory { get; set; }
}

public class VegProductCreateUpdateDto
{
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public int StockQuantity { get; set; }
    public int? IdCategory { get; set; }
}
```

### Dependency Injection Setup

In `Program.cs`:

```csharp
// Register Repositories
builder.Services.AddScoped<IVegCategoryRepository, VegCategoryRepository>();
builder.Services.AddScoped<IVegProductRepository, VegProductRepository>();

// Register Services
builder.Services.AddScoped<IVegCategoryService, VegCategoryService>();
builder.Services.AddScoped<IVegProductService, VegProductService>();
```

---

## Frontend Architecture

### Project Structure

```
src/app/
├── features/                  # Feature modules
│   ├── categories/
│   │   └── services/
│   │       └── category.service.ts
│   └── products/
│       └── services/
│           └── product.service.ts
├── shared/                   # Shared utilities
│   ├── models/
│   │   └── entities.ts       # All TypeScript interfaces
│   ├── services/
│   │   ├── notification.service.ts
│   │   └── dialog.service.ts
│   └── components/           # Reusable components
├── index-vegcategories/      # Category list component
├── create-vegcategory/       # Category create component
├── edit-vegcategory/         # Category edit component
├── index-products/           # Product list component  
├── create-vegproduct/        # Product create component
├── edit-vegproduct/          # Product edit component
└── app.routes.ts            # Route definitions
```

### Component Responsibilities

Each feature has:
- **Index Component:** Display list with CRUD actions
- **Create Component:** Form for creating new entities
- **Edit Component:** Form for editing existing entities

### Service Layer

Services handle all HTTP communication:

```typescript
@Injectable({ providedIn: 'root' })
export class ProductService {
  private apiUrl = `${environment.apiURL}/api/vegproducts`;
  
  constructor(private http: HttpClient) { }
  
  getAll(): Observable<VegProduct[]> {
    return this.http.get<VegProduct[]>(this.apiUrl);
  }
  
  create(data: VegProductCreateUpdate): Observable<VegProduct> {
    return this.http.post<VegProduct>(this.apiUrl, data);
  }
}
```

### Models (TypeScript Interfaces)

All models centralized in `shared/models/entities.ts`:

```typescript
export interface VegProduct {
  id: number;
  name: string;
  price: number;
  description?: string;
  stockQuantity: number;
  idCategory?: number;
  vegCategory?: VegCategoryBasic;
}

export interface VegProductCreateUpdate {
  name: string;
  price: number;
  description?: string;
  stockQuantity: number;
  idCategory?: number | null;
}
```

---

## Adding New Entities

This section provides step-by-step guides for two common scenarios:
1. **Scenario 1:** Adding a new field to an existing entity (VegProducts)
2. **Scenario 2:** Creating a new entity with a foreign key relationship to VegProducts

---

### SCENARIO 1: Adding a New Field to Existing Entity (VegProducts)

This scenario walks you through adding a new field (e.g., `ImageUrl`) to the VegProducts entity.

#### Backend Steps

##### Step 1.1: Update Entity Class

**File:** `DotNetCoreWebApi/DotNetCoreWebApi/Application/Entities/VegProducts.cs`

Add the new property to the entity class:

```csharp
public class VegProducts
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public int StockQuantity { get; set; }
    public int? IdCategory { get; set; }
    public virtual VegCategory? VegCategory { get; set; }
    
    // NEW FIELD - Add this line
    public string? ImageUrl { get; set; }
}
```

##### Step 1.2: Update DTOs

**File:** `DotNetCoreWebApi/DotNetCoreWebApi/DTOs/VegProductDto.cs`

Add the new field to the response DTO:

```csharp
public class VegProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public int StockQuantity { get; set; }
    public int? IdCategory { get; set; }
    public VegCategoryBasicDto? VegCategory { get; set; }
    
    // NEW FIELD - Add this line
    public string? ImageUrl { get; set; }
}
```

**File:** `DotNetCoreWebApi/DotNetCoreWebApi/DTOs/VegProductCreateUpdateDto.cs`

Add the new field to the create/update DTO:

```csharp
public class VegProductCreateUpdateDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;
    
    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }
    
    public string? Description { get; set; }
    
    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; }
    
    public int? IdCategory { get; set; }
    
    // NEW FIELD - Add this line with validation
    [Url]
    [StringLength(500)]
    public string? ImageUrl { get; set; }
}
```

##### Step 1.3: Update Service Layer

**File:** `DotNetCoreWebApi/DotNetCoreWebApi/Application/Services/VegProductService.cs`

Update the DTO mapping in `GetAllProductsAsync()`:

```csharp
public async Task<IEnumerable<VegProductDto>> GetAllProductsAsync()
{
    var products = await _productRepository.GetProductsWithCategoryAsync();
    
    return products.Select(p => new VegProductDto
    {
        Id = p.Id,
        Name = p.Name,
        Price = p.Price,
        Description = p.Description,
        StockQuantity = p.StockQuantity,
        IdCategory = p.IdCategory,
        ImageUrl = p.ImageUrl, // NEW FIELD - Add this line
        VegCategory = p.VegCategory == null ? null : new VegCategoryBasicDto
        {
            IdCategory = p.VegCategory.IdCategory,
            CategoryName = p.VegCategory.CategoryName,
            Description = p.VegCategory.Description
        }
    });
}
```

Update the DTO mapping in `GetProductByIdAsync()`:

```csharp
public async Task<VegProductDto?> GetProductByIdAsync(int id)
{
    var product = await _productRepository.GetProductWithCategoryAsync(id);
    
    if (product == null)
        return null;

    return new VegProductDto
    {
        Id = product.Id,
        Name = product.Name,
        Price = product.Price,
        Description = product.Description,
        StockQuantity = product.StockQuantity,
        IdCategory = product.IdCategory,
        ImageUrl = product.ImageUrl, // NEW FIELD - Add this line
        VegCategory = product.VegCategory == null ? null : new VegCategoryBasicDto
        {
            IdCategory = product.VegCategory.IdCategory,
            CategoryName = product.VegCategory.CategoryName,
            Description = product.VegCategory.Description
        }
    };
}
```

Update the mapping in `CreateProductAsync()`:

```csharp
public async Task<VegProductDto> CreateProductAsync(VegProductCreateUpdateDto dto)
{
    var product = new VegProducts
    {
        Name = dto.Name,
        Price = dto.Price,
        Description = dto.Description,
        StockQuantity = dto.StockQuantity,
        IdCategory = dto.IdCategory,
        ImageUrl = dto.ImageUrl // NEW FIELD - Add this line
    };

    var createdProduct = await _productRepository.AddAsync(product);
    
    return new VegProductDto
    {
        Id = createdProduct.Id,
        Name = createdProduct.Name,
        Price = createdProduct.Price,
        Description = createdProduct.Description,
        StockQuantity = createdProduct.StockQuantity,
        IdCategory = createdProduct.IdCategory,
        ImageUrl = createdProduct.ImageUrl // NEW FIELD - Add this line
    };
}
```

Update the mapping in `UpdateProductAsync()`:

```csharp
public async Task UpdateProductAsync(int id, VegProductCreateUpdateDto dto)
{
    var product = await _productRepository.GetByIdAsync(id);
    
    if (product == null)
        throw new KeyNotFoundException($"Product with ID {id} not found");

    product.Name = dto.Name;
    product.Price = dto.Price;
    product.Description = dto.Description;
    product.StockQuantity = dto.StockQuantity;
    product.IdCategory = dto.IdCategory;
    product.ImageUrl = dto.ImageUrl; // NEW FIELD - Add this line

    await _productRepository.UpdateAsync(product);
}
```

Update the mapping in `GetProductsByCategoryAsync()`:

```csharp
public async Task<IEnumerable<VegProductDto>> GetProductsByCategoryAsync(int categoryId)
{
    var products = await _productRepository.GetProductsByCategoryAsync(categoryId);
    
    return products.Select(p => new VegProductDto
    {
        Id = p.Id,
        Name = p.Name,
        Price = p.Price,
        Description = p.Description,
        StockQuantity = p.StockQuantity,
        IdCategory = p.IdCategory,
        ImageUrl = p.ImageUrl, // NEW FIELD - Add this line
        VegCategory = p.VegCategory == null ? null : new VegCategoryBasicDto
        {
            IdCategory = p.VegCategory.IdCategory,
            CategoryName = p.VegCategory.CategoryName,
            Description = p.VegCategory.Description
        }
    });
}
```

**Note:** You do NOT need to modify the Repository or Controller layers - they work with the updated entity automatically.

##### Step 1.4: Create Database Migration

```powershell
cd DotNetCoreWebApi/DotNetCoreWebApi
dotnet ef migrations add AddImageUrlToVegProducts
dotnet ef database update
```

This will add the `ImageUrl` column to the `VegProducts` table in the database.

#### Frontend Steps

##### Step 1.5: Update TypeScript Interface

**File:** `angular-app/src/app/vegproduct.ts`

Add the new field to the interface:

```typescript
export interface VegProduct {
  id: number;
  name: string;
  price: number;
  description?: string;
  stockQuantity?: number;
  idCategory?: number;
  vegCategory?: VegCategory;
  imageUrl?: string; // NEW FIELD - Add this line
}
```

**File:** `angular-app/src/app/vegproduct.models.ts`

Add the new field to the creation DTO:

```typescript
export interface VegProductCreation {
  name: string;
  price: number;
  description?: string;
  stockQuantity?: number;
  idCategory?: number;
  imageUrl?: string; // NEW FIELD - Add this line
}
```

**File:** `angular-app/src/app/shared/models/entities.ts`

Update both interfaces:

```typescript
export interface VegProduct extends IEntity {
  name: string;
  price: number;
  description?: string;
  stockQuantity?: number;
  idCategory?: number;
  vegCategory?: VegCategoryBasic;
  imageUrl?: string; // NEW FIELD - Add this line
}

export interface VegProductCreateUpdateDto {
  name: string;
  price: number;
  description?: string;
  stockQuantity?: number;
  idCategory?: number;
  imageUrl?: string; // NEW FIELD - Add this line
}
```

##### Step 1.6: Update Index Component (Display in Table)

**File:** `angular-app/src/app/index-products/index-products.ts`

Add the new column to the columns array:

```typescript
columns: ColumnConfig[] = [
  { key: 'id', label: 'ID', type: 'number' },
  { key: 'name', label: 'Product Name', type: 'text', sortable: true, filterable: true },
  { key: 'price', label: 'Price', type: 'currency', sortable: true },
  { key: 'description', label: 'Description', type: 'text' },
  { key: 'stockQuantity', label: 'Stock', type: 'number', sortable: true },
  { key: 'vegCategory.categoryName', label: 'Category', type: 'text', sortable: true, filterable: true },
  { key: 'imageUrl', label: 'Image URL', type: 'text' } // NEW COLUMN - Add this line
];
```

**Note:** The generic-data-table component will automatically display the new field. If you want to hide it, add it to `hiddenColumns`:

```typescript
hiddenColumns = ['id', 'imageUrl']; // Hide image URL if you prefer
```

##### Step 1.7: Update Create Component Form

**File:** `angular-app/src/app/create-vegproduct/create-vegproduct.ts`

Add the new form control to the FormGroup:

```typescript
productForm = new FormGroup({
  name: new FormControl('', [Validators.required, Validators.maxLength(100)]),
  description: new FormControl(''),
  price: new FormControl<number | null>(null, [Validators.required, Validators.min(0)]),
  stockQuantity: new FormControl<number | null>(null, [Validators.required, Validators.min(0)]),
  idCategory: new FormControl<number | null>(null),
  imageUrl: new FormControl('', [Validators.maxLength(500)]) // NEW FIELD - Add this line
});
```

**File:** `angular-app/src/app/create-vegproduct/create-vegproduct.html`

Add the new form field after the existing fields:

```html
<!-- Existing fields... -->

<!-- NEW FIELD - Add this block -->
<mat-form-field appearance="outline" class="full-width">
  <mat-label>Image URL</mat-label>
  <input matInput formControlName="imageUrl" placeholder="https://example.com/image.jpg">
  <mat-icon matPrefix>image</mat-icon>
  <mat-hint>Optional: URL to product image</mat-hint>
  <mat-error *ngIf="productForm.get('imageUrl')?.hasError('maxlength')">
    Image URL cannot exceed 500 characters
  </mat-error>
</mat-form-field>
```

##### Step 1.8: Update Edit Component Form

**File:** `angular-app/src/app/edit-vegproduct/edit-vegproduct.ts`

Add the new form control:

```typescript
productForm = new FormGroup({
  name: new FormControl('', [Validators.required, Validators.maxLength(100)]),
  description: new FormControl(''),
  price: new FormControl<number | null>(null, [Validators.required, Validators.min(0)]),
  stockQuantity: new FormControl<number | null>(null, [Validators.required, Validators.min(0)]),
  idCategory: new FormControl<number | null>(null),
  imageUrl: new FormControl('', [Validators.maxLength(500)]) // NEW FIELD - Add this line
});
```

Update the form patching in `ngOnInit()`:

```typescript
this.productForm.patchValue({
  name: product.name,
  description: product.description || '',
  price: product.price,
  stockQuantity: product.stockQuantity || 0,
  idCategory: product.idCategory || null,
  imageUrl: product.imageUrl || '' // NEW FIELD - Add this line
});
```

**File:** `angular-app/src/app/edit-vegproduct/edit-vegproduct.html`

Add the same form field as in create-vegproduct.html (copy the mat-form-field block from Step 1.7).

##### Step 1.9: Test the New Field

1. **Start Backend:**
   ```powershell
   cd DotNetCoreWebApi/DotNetCoreWebApi
   dotnet run
   ```

2. **Start Frontend:**
   ```powershell
   cd angular-app
   npm start
   ```

3. **Test Flow:**
   - Navigate to `http://localhost:4201/admin/products/create`
   - Fill in all fields including the new ImageUrl field
   - Click "Create Product"
   - Go to `http://localhost:4201/admin/products` and verify the new column appears
   - Edit an existing product and verify the ImageUrl field loads and saves correctly

---

### SCENARIO 2: Creating New Entity with Foreign Key to VegProducts

This scenario walks you through creating a new entity called `ProductReview` that has a foreign key relationship to `VegProducts`.

#### Backend Steps

##### Step 2.1: Create Entity Class

**File:** `DotNetCoreWebApi/DotNetCoreWebApi/Application/Entities/ProductReview.cs` (NEW FILE)

Create a new entity with a foreign key to VegProducts:

```csharp
namespace DotNetCoreWebApi.Application.Entities;

/// <summary>
/// ProductReview Entity
/// Represents a customer review for a product
/// </summary>
public class ProductReview
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public required string ReviewerName { get; set; }
    
    [Range(1, 5)]
    public int Rating { get; set; }
    
    [StringLength(1000)]
    public string? Comment { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Foreign Key to VegProducts
    public int ProductId { get; set; }
    
    // Navigation Property
    public virtual VegProducts? Product { get; set; }
}
```

##### Step 2.2: Update VegProducts Entity (Optional - for bidirectional relationship)

**File:** `DotNetCoreWebApi/DotNetCoreWebApi/Application/Entities/VegProducts.cs`

Add navigation property for reviews (optional, but recommended):

```csharp
public class VegProducts
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public int StockQuantity { get; set; }
    public int? IdCategory { get; set; }
    public virtual VegCategory? VegCategory { get; set; }
    
    // NEW - Add navigation property for reviews
    public virtual ICollection<ProductReview> Reviews { get; set; } = new List<ProductReview>();
}
```

##### Step 2.3: Update DbContext

**File:** `DotNetCoreWebApi/DotNetCoreWebApi/Application/DBContext/ApplicationDBContext.cs`

Add DbSet and configure relationship:

```csharp
public class ApplicationDBContext : DbContext
{
    public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options) { }

    public DbSet<VegProducts> VegProducts { get; set; }
    public DbSet<VegCategory> VegCategories { get; set; }
    public DbSet<ProductReview> ProductReviews { get; set; } // NEW - Add this line

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Existing configurations...
        
        // NEW - Configure ProductReview entity and foreign key relationship
        modelBuilder.Entity<ProductReview>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.ReviewerName)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(e => e.Rating)
                .IsRequired();
            
            entity.Property(e => e.Comment)
                .HasMaxLength(1000);
            
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
            
            // Configure foreign key relationship
            entity.HasOne(r => r.Product)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // Delete reviews when product is deleted
        });
    }
}
```

##### Step 2.4: Create DTOs

**File:** `DotNetCoreWebApi/DotNetCoreWebApi/DTOs/ProductReviewDto.cs` (NEW FILE)

```csharp
namespace DotNetCoreWebApi.DTOs;

/// <summary>
/// DTO for ProductReview responses
/// </summary>
public class ProductReviewDto
{
    public int Id { get; set; }
    public string ReviewerName { get; set; } = null!;
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
    public int ProductId { get; set; }
    
    // Include basic product info
    public VegProductBasicDto? Product { get; set; }
}

/// <summary>
/// Basic product info for nested responses
/// </summary>
public class VegProductBasicDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
}

/// <summary>
/// DTO for creating/updating ProductReview
/// </summary>
public class ProductReviewCreateUpdateDto
{
    [Required]
    [StringLength(100)]
    public string ReviewerName { get; set; } = null!;
    
    [Required]
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
    public int Rating { get; set; }
    
    [StringLength(1000)]
    public string? Comment { get; set; }
    
    [Required]
    public int ProductId { get; set; }
}
```

##### Step 2.5: Create Repository Interface

**File:** `DotNetCoreWebApi/DotNetCoreWebApi/Application/Interfaces/IProductReviewRepository.cs` (NEW FILE)

```csharp
using DotNetCoreWebApi.Application.Entities;

namespace DotNetCoreWebApi.Application.Interfaces;

/// <summary>
/// Repository interface for ProductReview entity
/// </summary>
public interface IProductReviewRepository : IRepository<ProductReview>
{
    /// <summary>
    /// Get all reviews with product information
    /// </summary>
    Task<IEnumerable<ProductReview>> GetReviewsWithProductAsync();
    
    /// <summary>
    /// Get a specific review with product information
    /// </summary>
    Task<ProductReview?> GetReviewWithProductAsync(int id);
    
    /// <summary>
    /// Get all reviews for a specific product
    /// </summary>
    Task<IEnumerable<ProductReview>> GetReviewsByProductIdAsync(int productId);
}
```

##### Step 2.6: Create Repository Implementation

**File:** `DotNetCoreWebApi/DotNetCoreWebApi/Infrastructure/Repositories/ProductReviewRepository.cs` (NEW FILE)

```csharp
using DotNetCoreWebApi.Application.DBContext;
using DotNetCoreWebApi.Application.Entities;
using DotNetCoreWebApi.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DotNetCoreWebApi.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for ProductReview entity
/// </summary>
public class ProductReviewRepository : Repository<ProductReview>, IProductReviewRepository
{
    public ProductReviewRepository(ApplicationDBContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ProductReview>> GetReviewsWithProductAsync()
    {
        return await _dbSet
            .Include(r => r.Product)
            .OrderByDescending(r => r.CreatedAt)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<ProductReview?> GetReviewWithProductAsync(int id)
    {
        return await _dbSet
            .Include(r => r.Product)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<ProductReview>> GetReviewsByProductIdAsync(int productId)
    {
        return await _dbSet
            .Include(r => r.Product)
            .Where(r => r.ProductId == productId)
            .OrderByDescending(r => r.CreatedAt)
            .AsNoTracking()
            .ToListAsync();
    }
}
```

##### Step 2.7: Create Service Interface

**File:** `DotNetCoreWebApi/DotNetCoreWebApi/Application/Interfaces/IProductReviewService.cs` (NEW FILE)

```csharp
using DotNetCoreWebApi.DTOs;

namespace DotNetCoreWebApi.Application.Interfaces;

/// <summary>
/// Service interface for ProductReview business logic
/// </summary>
public interface IProductReviewService
{
    Task<IEnumerable<ProductReviewDto>> GetAllReviewsAsync();
    Task<ProductReviewDto?> GetReviewByIdAsync(int id);
    Task<IEnumerable<ProductReviewDto>> GetReviewsByProductIdAsync(int productId);
    Task<ProductReviewDto> CreateReviewAsync(ProductReviewCreateUpdateDto dto);
    Task UpdateReviewAsync(int id, ProductReviewCreateUpdateDto dto);
    Task DeleteReviewAsync(int id);
    Task<bool> ReviewExistsAsync(int id);
}
```

##### Step 2.8: Create Service Implementation

**File:** `DotNetCoreWebApi/DotNetCoreWebApi/Application/Services/ProductReviewService.cs` (NEW FILE)

```csharp
using DotNetCoreWebApi.Application.Entities;
using DotNetCoreWebApi.Application.Interfaces;
using DotNetCoreWebApi.DTOs;

namespace DotNetCoreWebApi.Application.Services;

/// <summary>
/// Service implementation for ProductReview business logic
/// </summary>
public class ProductReviewService : IProductReviewService
{
    private readonly IProductReviewRepository _reviewRepository;
    private readonly IVegProductRepository _productRepository;

    public ProductReviewService(
        IProductReviewRepository reviewRepository,
        IVegProductRepository productRepository)
    {
        _reviewRepository = reviewRepository;
        _productRepository = productRepository;
    }

    public async Task<IEnumerable<ProductReviewDto>> GetAllReviewsAsync()
    {
        var reviews = await _reviewRepository.GetReviewsWithProductAsync();
        
        return reviews.Select(r => new ProductReviewDto
        {
            Id = r.Id,
            ReviewerName = r.ReviewerName,
            Rating = r.Rating,
            Comment = r.Comment,
            CreatedAt = r.CreatedAt,
            ProductId = r.ProductId,
            Product = r.Product == null ? null : new VegProductBasicDto
            {
                Id = r.Product.Id,
                Name = r.Product.Name,
                Price = r.Product.Price
            }
        });
    }

    public async Task<ProductReviewDto?> GetReviewByIdAsync(int id)
    {
        var review = await _reviewRepository.GetReviewWithProductAsync(id);
        
        if (review == null)
            return null;

        return new ProductReviewDto
        {
            Id = review.Id,
            ReviewerName = review.ReviewerName,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt,
            ProductId = review.ProductId,
            Product = review.Product == null ? null : new VegProductBasicDto
            {
                Id = review.Product.Id,
                Name = review.Product.Name,
                Price = review.Product.Price
            }
        };
    }

    public async Task<IEnumerable<ProductReviewDto>> GetReviewsByProductIdAsync(int productId)
    {
        var reviews = await _reviewRepository.GetReviewsByProductIdAsync(productId);
        
        return reviews.Select(r => new ProductReviewDto
        {
            Id = r.Id,
            ReviewerName = r.ReviewerName,
            Rating = r.Rating,
            Comment = r.Comment,
            CreatedAt = r.CreatedAt,
            ProductId = r.ProductId,
            Product = r.Product == null ? null : new VegProductBasicDto
            {
                Id = r.Product.Id,
                Name = r.Product.Name,
                Price = r.Product.Price
            }
        });
    }

    public async Task<ProductReviewDto> CreateReviewAsync(ProductReviewCreateUpdateDto dto)
    {
        // Validate that product exists
        var productExists = await _productRepository.ExistsAsync(dto.ProductId);
        if (!productExists)
            throw new KeyNotFoundException($"Product with ID {dto.ProductId} not found");

        var review = new ProductReview
        {
            ReviewerName = dto.ReviewerName,
            Rating = dto.Rating,
            Comment = dto.Comment,
            ProductId = dto.ProductId,
            CreatedAt = DateTime.UtcNow
        };

        var createdReview = await _reviewRepository.AddAsync(review);
        
        return new ProductReviewDto
        {
            Id = createdReview.Id,
            ReviewerName = createdReview.ReviewerName,
            Rating = createdReview.Rating,
            Comment = createdReview.Comment,
            CreatedAt = createdReview.CreatedAt,
            ProductId = createdReview.ProductId
        };
    }

    public async Task UpdateReviewAsync(int id, ProductReviewCreateUpdateDto dto)
    {
        var review = await _reviewRepository.GetByIdAsync(id);
        
        if (review == null)
            throw new KeyNotFoundException($"Review with ID {id} not found");

        // Validate that product exists if ProductId is being changed
        if (review.ProductId != dto.ProductId)
        {
            var productExists = await _productRepository.ExistsAsync(dto.ProductId);
            if (!productExists)
                throw new KeyNotFoundException($"Product with ID {dto.ProductId} not found");
        }

        review.ReviewerName = dto.ReviewerName;
        review.Rating = dto.Rating;
        review.Comment = dto.Comment;
        review.ProductId = dto.ProductId;

        await _reviewRepository.UpdateAsync(review);
    }

    public async Task DeleteReviewAsync(int id)
    {
        var review = await _reviewRepository.GetByIdAsync(id);
        
        if (review == null)
            throw new KeyNotFoundException($"Review with ID {id} not found");

        await _reviewRepository.DeleteAsync(review);
    }

    public async Task<bool> ReviewExistsAsync(int id)
    {
        return await _reviewRepository.ExistsAsync(id);
    }
}
```

##### Step 2.9: Create Controller

**File:** `DotNetCoreWebApi/DotNetCoreWebApi/Controllers/ProductReviewsController.cs` (NEW FILE)

```csharp
using DotNetCoreWebApi.Application.Interfaces;
using DotNetCoreWebApi.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DotNetCoreWebApi.Controllers;

/// <summary>
/// Controller for ProductReview operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductReviewsController : ControllerBase
{
    private readonly IProductReviewService _reviewService;

    public ProductReviewsController(IProductReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    /// <summary>
    /// Get all product reviews
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductReviewDto>>> GetAllReviews()
    {
        var reviews = await _reviewService.GetAllReviewsAsync();
        return Ok(reviews);
    }

    /// <summary>
    /// Get a specific review by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductReviewDto>> GetReviewById(int id)
    {
        var review = await _reviewService.GetReviewByIdAsync(id);

        if (review == null)
            return NotFound(new { message = $"Review with ID {id} not found" });

        return Ok(review);
    }

    /// <summary>
    /// Get all reviews for a specific product
    /// </summary>
    [HttpGet("product/{productId}")]
    public async Task<ActionResult<IEnumerable<ProductReviewDto>>> GetReviewsByProduct(int productId)
    {
        var reviews = await _reviewService.GetReviewsByProductIdAsync(productId);
        return Ok(reviews);
    }

    /// <summary>
    /// Create a new review
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ProductReviewDto>> CreateReview([FromBody] ProductReviewCreateUpdateDto reviewDto)
    {
        try
        {
            var review = await _reviewService.CreateReviewAsync(reviewDto);
            return CreatedAtAction(nameof(GetReviewById), new { id = review.Id }, review);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing review
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateReview(int id, [FromBody] ProductReviewCreateUpdateDto reviewDto)
    {
        try
        {
            await _reviewService.UpdateReviewAsync(id, reviewDto);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Delete a review
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReview(int id)
    {
        try
        {
            await _reviewService.DeleteReviewAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
```

##### Step 2.10: Register Services in Program.cs

**File:** `DotNetCoreWebApi/DotNetCoreWebApi/Program.cs`

Add the new service registrations:

```csharp
// Existing registrations...
builder.Services.AddScoped<IVegProductRepository, VegProductRepository>();
builder.Services.AddScoped<IVegProductService, VegProductService>();
builder.Services.AddScoped<IVegCategoryRepository, VegCategoryRepository>();
builder.Services.AddScoped<IVegCategoryService, VegCategoryService>();

// NEW - Add these lines for ProductReview
builder.Services.AddScoped<IProductReviewRepository, ProductReviewRepository>();
builder.Services.AddScoped<IProductReviewService, ProductReviewService>();
```

##### Step 2.11: Create Database Migration

```powershell
cd DotNetCoreWebApi/DotNetCoreWebApi
dotnet ef migrations add AddProductReviewEntity
dotnet ef database update
```

This will create the `ProductReviews` table with a foreign key to `VegProducts`.

##### Step 2.12: Test Backend API

Start the backend:

```powershell
cd DotNetCoreWebApi/DotNetCoreWebApi
dotnet run
```

Test the endpoints using a tool like Postman or curl:

```powershell
# Get all reviews
GET https://localhost:7020/api/productreviews

# Create a review (replace {productId} with actual product ID)
POST https://localhost:7020/api/productreviews
Content-Type: application/json

{
  "reviewerName": "John Doe",
  "rating": 5,
  "comment": "Excellent product!",
  "productId": 1
}

# Get reviews for a specific product
GET https://localhost:7020/api/productreviews/product/1
```

#### Frontend Steps

##### Step 2.13: Add TypeScript Interfaces

**File:** `angular-app/src/app/shared/models/entities.ts`

Add the new interfaces:

```typescript
// Existing interfaces...

/**
 * ProductReview Entity
 * Represents a customer review for a product
 */
export interface ProductReview extends IEntity {
  reviewerName: string;
  rating: number;
  comment?: string;
  createdAt: Date | string;
  productId: number;
  product?: VegProductBasic;
}

/**
 * Basic VegProduct for nested references in reviews
 */
export interface VegProductBasic {
  id: number;
  name: string;
  price: number;
}

/**
 * DTO for creating/updating reviews
 */
export interface ProductReviewCreateUpdateDto {
  reviewerName: string;
  rating: number;
  comment?: string;
  productId: number;
}
```

##### Step 2.14: Create Service

**File:** `angular-app/src/app/features/reviews/services/product-review.service.ts` (NEW FILE)

Create the directory first: `mkdir -p angular-app/src/app/features/reviews/services`

```typescript
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ProductReview, ProductReviewCreateUpdateDto } from '../../../shared/models/entities';
import { environment } from '../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ProductReviewService {
  private apiUrl = `${environment.apiURL}/api/productreviews`;
  
  constructor(private http: HttpClient) { }
  
  /**
   * Get all product reviews
   */
  getAll(): Observable<ProductReview[]> {
    return this.http.get<ProductReview[]>(this.apiUrl);
  }
  
  /**
   * Get review by ID
   */
  getById(id: number): Observable<ProductReview> {
    return this.http.get<ProductReview>(`${this.apiUrl}/${id}`);
  }
  
  /**
   * Get all reviews for a specific product
   */
  getByProductId(productId: number): Observable<ProductReview[]> {
    return this.http.get<ProductReview[]>(`${this.apiUrl}/product/${productId}`);
  }
  
  /**
   * Create a new review
   */
  create(data: ProductReviewCreateUpdateDto): Observable<ProductReview> {
    return this.http.post<ProductReview>(this.apiUrl, data);
  }
  
  /**
   * Update an existing review
   */
  update(id: number, data: ProductReviewCreateUpdateDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, data);
  }
  
  /**
   * Delete a review
   */
  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
```

##### Step 2.15: Create Index Component (List Reviews)

**File:** `angular-app/src/app/index-reviews/index-reviews.ts` (NEW FILE)

Create the directory first: `mkdir angular-app/src/app/index-reviews`

```typescript
import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { GenericDataTableComponent, ColumnConfig, ActionConfig } from '../shared/components/generic-data-table/generic-data-table.component';
import { ProductReviewService } from '../features/reviews/services/product-review.service';
import { ProductReview } from '../shared/models/entities';

@Component({
  selector: 'app-index-reviews',
  standalone: true,
  imports: [
    CommonModule,
    GenericDataTableComponent,
    MatSnackBarModule,
    MatDialogModule
  ],
  templateUrl: './index-reviews.html',
  styleUrl: './index-reviews.css'
})
export class IndexReviews implements OnInit {
  private router = inject(Router);
  private snackBar = inject(MatSnackBar);
  private dialog = inject(MatDialog);
  private reviewService = inject(ProductReviewService);

  // Signal for reviews data
  reviews = signal<ProductReview[]>([]);

  // Table configuration
  columns: ColumnConfig[] = [
    { key: 'id', label: 'ID', type: 'number' },
    { key: 'reviewerName', label: 'Reviewer', type: 'text', sortable: true, filterable: true },
    { key: 'rating', label: 'Rating', type: 'number', sortable: true },
    { key: 'comment', label: 'Comment', type: 'text' },
    { key: 'product.name', label: 'Product', type: 'text', sortable: true, filterable: true },
    { key: 'createdAt', label: 'Date', type: 'date', sortable: true }
  ];

  actions: ActionConfig[] = [
    { icon: 'edit', label: 'Edit', color: 'primary' },
    { icon: 'delete', label: 'Delete', color: 'warn' }
  ];

  hiddenColumns = ['id'];

  ngOnInit(): void {
    this.loadReviews();
  }

  /**
   * Load all reviews from API
   */
  loadReviews(): void {
    this.reviewService.getAll().subscribe({
      next: (data) => {
        this.reviews.set(data);
      },
      error: (error) => {
        console.error('Error loading reviews:', error);
        this.snackBar.open('Error loading reviews', 'Close', { duration: 3000 });
      }
    });
  }

  /**
   * Handle edit action
   */
  onEdit(review: ProductReview): void {
    this.router.navigate(['/admin/reviews/edit', review.id]);
  }

  /**
   * Handle delete action
   */
  onDelete(review: ProductReview): void {
    const confirmed = confirm(`Are you sure you want to delete the review by ${review.reviewerName}?`);
    
    if (confirmed) {
      this.reviewService.delete(review.id).subscribe({
        next: () => {
          this.snackBar.open('Review deleted successfully', 'Close', { duration: 3000 });
          this.loadReviews(); // Reload the list
        },
        error: (error) => {
          console.error('Error deleting review:', error);
          this.snackBar.open('Error deleting review', 'Close', { duration: 3000 });
        }
      });
    }
  }
}
```

**File:** `angular-app/src/app/index-reviews/index-reviews.html` (NEW FILE)

```html
<div class="page-container">
  <div class="page-header">
    <h1 class="page-title">Product Reviews</h1>
    <button mat-raised-button color="primary" routerLink="/admin/reviews/create" class="create-button">
      <mat-icon>add</mat-icon>
      Add Review
    </button>
  </div>

  <app-generic-data-table
    [items]="reviews()"
    [columns]="columns"
    [actions]="actions"
    [hiddenColumns]="hiddenColumns"
    [cardEmoji]="'⭐'"
    searchPlaceholder="Search reviews..."
    (edit)="onEdit($event)"
    (delete)="onDelete($event)"
    (reload)="loadReviews()"
  ></app-generic-data-table>
</div>
```

**File:** `angular-app/src/app/index-reviews/index-reviews.css` (NEW FILE)

```css
.page-container {
  padding: 24px;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
}

.page-title {
  margin: 0;
  font-size: 28px;
  font-weight: 600;
  color: #1e293b;
}

.create-button {
  height: 44px;
}
```

##### Step 2.16: Create Create Component

**File:** `angular-app/src/app/create-review/create-review.ts` (NEW FILE)

Create the directory first: `mkdir angular-app/src/app/create-review`

```typescript
import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatCardModule } from '@angular/material/card';
import { ProductReviewService } from '../features/reviews/services/product-review.service';
import { Vegproduct, VegProduct } from '../vegproduct';
import { ProductReviewCreateUpdateDto } from '../shared/models/entities';

@Component({
  selector: 'app-create-review',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule,
    MatIconModule,
    MatSnackBarModule,
    MatCardModule
  ],
  templateUrl: './create-review.html',
  styleUrl: './create-review.css'
})
export class CreateReview implements OnInit {
  private router = inject(Router);
  private snackBar = inject(MatSnackBar);
  private reviewService = inject(ProductReviewService);
  private productService = inject(Vegproduct);

  // Available products for dropdown
  products = signal<VegProduct[]>([]);
  
  // Rating options
  ratingOptions = [1, 2, 3, 4, 5];

  // Review form
  reviewForm = new FormGroup({
    reviewerName: new FormControl('', [Validators.required, Validators.maxLength(100)]),
    rating: new FormControl<number | null>(null, [Validators.required, Validators.min(1), Validators.max(5)]),
    comment: new FormControl('', [Validators.maxLength(1000)]),
    productId: new FormControl<number | null>(null, [Validators.required])
  });

  ngOnInit(): void {
    this.loadProducts();
  }

  /**
   * Load products for dropdown
   */
  loadProducts(): void {
    this.productService.getVegproducts().subscribe({
      next: (data) => {
        this.products.set(data);
      },
      error: (error) => {
        console.error('Error loading products:', error);
        this.snackBar.open('Error loading products', 'Close', { duration: 3000 });
      }
    });
  }

  /**
   * Submit form to create review
   */
  onSubmit(): void {
    if (this.reviewForm.valid) {
      const formValue = this.reviewForm.value;
      
      const reviewData: ProductReviewCreateUpdateDto = {
        reviewerName: formValue.reviewerName!,
        rating: formValue.rating!,
        comment: formValue.comment || undefined,
        productId: formValue.productId!
      };

      this.reviewService.create(reviewData).subscribe({
        next: () => {
          this.snackBar.open('Review created successfully!', 'Close', { duration: 3000 });
          this.router.navigate(['/admin/reviews']);
        },
        error: (error) => {
          console.error('Error creating review:', error);
          this.snackBar.open('Error creating review. Please try again.', 'Close', { duration: 3000 });
        }
      });
    }
  }

  /**
   * Cancel and navigate back
   */
  onCancel(): void {
    this.router.navigate(['/admin/reviews']);
  }
}
```

**File:** `angular-app/src/app/create-review/create-review.html` (NEW FILE)

```html
<div class="page-container">
  <mat-card class="form-card">
    <mat-card-header class="card-header">
      <mat-card-title class="card-title">
        <mat-icon class="title-icon">add_comment</mat-icon>
        <span>Add New Review</span>
      </mat-card-title>
    </mat-card-header>

    <mat-card-content>
      <form [formGroup]="reviewForm" (ngSubmit)="onSubmit()" class="review-form">
        
        <!-- Reviewer Name -->
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Reviewer Name</mat-label>
          <input matInput formControlName="reviewerName" placeholder="Enter reviewer name" required>
          <mat-icon matPrefix>person</mat-icon>
          <mat-error *ngIf="reviewForm.get('reviewerName')?.hasError('required')">
            Reviewer name is required
          </mat-error>
          <mat-error *ngIf="reviewForm.get('reviewerName')?.hasError('maxlength')">
            Reviewer name cannot exceed 100 characters
          </mat-error>
        </mat-form-field>

        <!-- Product Selection -->
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Product</mat-label>
          <mat-select formControlName="productId" required>
            <mat-option [value]="null">Select a product</mat-option>
            <mat-option *ngFor="let product of products()" [value]="product.id">
              {{ product.name }} - {{ product.price | currency: 'COP' }}
            </mat-option>
          </mat-select>
          <mat-icon matPrefix>shopping_bag</mat-icon>
          <mat-error *ngIf="reviewForm.get('productId')?.hasError('required')">
            Product selection is required
          </mat-error>
        </mat-form-field>

        <!-- Rating -->
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Rating</mat-label>
          <mat-select formControlName="rating" required>
            <mat-option [value]="null">Select rating</mat-option>
            <mat-option *ngFor="let rating of ratingOptions" [value]="rating">
              {{ rating }} ⭐ {{ rating === 1 ? 'star' : 'stars' }}
            </mat-option>
          </mat-select>
          <mat-icon matPrefix>star</mat-icon>
          <mat-error *ngIf="reviewForm.get('rating')?.hasError('required')">
            Rating is required
          </mat-error>
        </mat-form-field>

        <!-- Comment -->
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Comment</mat-label>
          <textarea 
            matInput 
            formControlName="comment" 
            placeholder="Write your review here..."
            rows="5"
          ></textarea>
          <mat-icon matPrefix>comment</mat-icon>
          <mat-hint align="end">
            {{ reviewForm.get('comment')?.value?.length || 0 }} / 1000
          </mat-hint>
          <mat-error *ngIf="reviewForm.get('comment')?.hasError('maxlength')">
            Comment cannot exceed 1000 characters
          </mat-error>
        </mat-form-field>

        <!-- Action Buttons -->
        <div class="button-group">
          <button mat-raised-button type="button" (click)="onCancel()" class="cancel-button">
            <mat-icon>cancel</mat-icon>
            Cancel
          </button>
          <button mat-raised-button color="primary" type="submit" [disabled]="!reviewForm.valid" class="submit-button">
            <mat-icon>save</mat-icon>
            Create Review
          </button>
        </div>
      </form>
    </mat-card-content>
  </mat-card>
</div>
```

**File:** `angular-app/src/app/create-review/create-review.css` (NEW FILE)

```css
.page-container {
  max-width: 800px;
  margin: 24px auto;
  padding: 24px;
}

.form-card {
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  border-radius: 12px;
}

.card-header {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 24px;
  border-radius: 12px 12px 0 0;
  margin: -16px -16px 24px -16px;
}

.card-title {
  display: flex;
  align-items: center;
  gap: 12px;
  font-size: 24px;
  font-weight: 600;
  margin: 0;
}

.title-icon {
  font-size: 32px;
  width: 32px;
  height: 32px;
}

.review-form {
  display: flex;
  flex-direction: column;
  gap: 20px;
  padding: 24px;
}

.full-width {
  width: 100%;
}

.button-group {
  display: flex;
  gap: 16px;
  justify-content: flex-end;
  margin-top: 24px;
}

.cancel-button,
.submit-button {
  min-width: 140px;
  height: 44px;
  font-weight: 600;
}

@media (max-width: 768px) {
  .page-container {
    padding: 16px;
  }

  .button-group {
    flex-direction: column;
  }

  .cancel-button,
  .submit-button {
    width: 100%;
  }
}
```

##### Step 2.17: Create Edit Component

**File:** `angular-app/src/app/edit-review/edit-review.ts` (NEW FILE)

Create the directory first: `mkdir angular-app/src/app/edit-review`

```typescript
import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatCardModule } from '@angular/material/card';
import { ProductReviewService } from '../features/reviews/services/product-review.service';
import { Vegproduct, VegProduct } from '../vegproduct';
import { ProductReviewCreateUpdateDto } from '../shared/models/entities';

@Component({
  selector: 'app-edit-review',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule,
    MatIconModule,
    MatSnackBarModule,
    MatCardModule
  ],
  templateUrl: './edit-review.html',
  styleUrl: './edit-review.css'
})
export class EditReview implements OnInit {
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private snackBar = inject(MatSnackBar);
  private reviewService = inject(ProductReviewService);
  private productService = inject(Vegproduct);

  // Review ID from route
  reviewId: number = 0;
  
  // Available products for dropdown
  products = signal<VegProduct[]>([]);
  
  // Rating options
  ratingOptions = [1, 2, 3, 4, 5];

  // Review form
  reviewForm = new FormGroup({
    reviewerName: new FormControl('', [Validators.required, Validators.maxLength(100)]),
    rating: new FormControl<number | null>(null, [Validators.required, Validators.min(1), Validators.max(5)]),
    comment: new FormControl('', [Validators.maxLength(1000)]),
    productId: new FormControl<number | null>(null, [Validators.required])
  });

  ngOnInit(): void {
    // Get review ID from route
    this.reviewId = Number(this.route.snapshot.paramMap.get('id'));
    
    // Load products and review data
    this.loadProducts();
    this.loadReview();
  }

  /**
   * Load products for dropdown
   */
  loadProducts(): void {
    this.productService.getVegproducts().subscribe({
      next: (data) => {
        this.products.set(data);
      },
      error: (error) => {
        console.error('Error loading products:', error);
        this.snackBar.open('Error loading products', 'Close', { duration: 3000 });
      }
    });
  }

  /**
   * Load review data for editing
   */
  loadReview(): void {
    this.reviewService.getById(this.reviewId).subscribe({
      next: (review) => {
        this.reviewForm.patchValue({
          reviewerName: review.reviewerName,
          rating: review.rating,
          comment: review.comment || '',
          productId: review.productId
        });
      },
      error: (error) => {
        console.error('Error loading review:', error);
        this.snackBar.open('Error loading review', 'Close', { duration: 3000 });
        this.router.navigate(['/admin/reviews']);
      }
    });
  }

  /**
   * Submit form to update review
   */
  onSubmit(): void {
    if (this.reviewForm.valid) {
      const formValue = this.reviewForm.value;
      
      const reviewData: ProductReviewCreateUpdateDto = {
        reviewerName: formValue.reviewerName!,
        rating: formValue.rating!,
        comment: formValue.comment || undefined,
        productId: formValue.productId!
      };

      this.reviewService.update(this.reviewId, reviewData).subscribe({
        next: () => {
          this.snackBar.open('Review updated successfully!', 'Close', { duration: 3000 });
          this.router.navigate(['/admin/reviews']);
        },
        error: (error) => {
          console.error('Error updating review:', error);
          this.snackBar.open('Error updating review. Please try again.', 'Close', { duration: 3000 });
        }
      });
    }
  }

  /**
   * Cancel and navigate back
   */
  onCancel(): void {
    this.router.navigate(['/admin/reviews']);
  }
}
```

**File:** `angular-app/src/app/edit-review/edit-review.html` (NEW FILE)

```html
<div class="page-container">
  <mat-card class="form-card">
    <mat-card-header class="card-header">
      <mat-card-title class="card-title">
        <mat-icon class="title-icon">edit</mat-icon>
        <span>Edit Review</span>
      </mat-card-title>
    </mat-card-header>

    <mat-card-content>
      <form [formGroup]="reviewForm" (ngSubmit)="onSubmit()" class="review-form">
        
        <!-- Reviewer Name -->
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Reviewer Name</mat-label>
          <input matInput formControlName="reviewerName" placeholder="Enter reviewer name" required>
          <mat-icon matPrefix>person</mat-icon>
          <mat-error *ngIf="reviewForm.get('reviewerName')?.hasError('required')">
            Reviewer name is required
          </mat-error>
          <mat-error *ngIf="reviewForm.get('reviewerName')?.hasError('maxlength')">
            Reviewer name cannot exceed 100 characters
          </mat-error>
        </mat-form-field>

        <!-- Product Selection -->
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Product</mat-label>
          <mat-select formControlName="productId" required>
            <mat-option [value]="null">Select a product</mat-option>
            <mat-option *ngFor="let product of products()" [value]="product.id">
              {{ product.name }} - {{ product.price | currency: 'COP' }}
            </mat-option>
          </mat-select>
          <mat-icon matPrefix>shopping_bag</mat-icon>
          <mat-error *ngIf="reviewForm.get('productId')?.hasError('required')">
            Product selection is required
          </mat-error>
        </mat-form-field>

        <!-- Rating -->
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Rating</mat-label>
          <mat-select formControlName="rating" required>
            <mat-option [value]="null">Select rating</mat-option>
            <mat-option *ngFor="let rating of ratingOptions" [value]="rating">
              {{ rating }} ⭐ {{ rating === 1 ? 'star' : 'stars' }}
            </mat-option>
          </mat-select>
          <mat-icon matPrefix>star</mat-icon>
          <mat-error *ngIf="reviewForm.get('rating')?.hasError('required')">
            Rating is required
          </mat-error>
        </mat-form-field>

        <!-- Comment -->
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Comment</mat-label>
          <textarea 
            matInput 
            formControlName="comment" 
            placeholder="Write your review here..."
            rows="5"
          ></textarea>
          <mat-icon matPrefix>comment</mat-icon>
          <mat-hint align="end">
            {{ reviewForm.get('comment')?.value?.length || 0 }} / 1000
          </mat-hint>
          <mat-error *ngIf="reviewForm.get('comment')?.hasError('maxlength')">
            Comment cannot exceed 1000 characters
          </mat-error>
        </mat-form-field>

        <!-- Action Buttons -->
        <div class="button-group">
          <button mat-raised-button type="button" (click)="onCancel()" class="cancel-button">
            <mat-icon>cancel</mat-icon>
            Cancel
          </button>
          <button mat-raised-button color="primary" type="submit" [disabled]="!reviewForm.valid" class="submit-button">
            <mat-icon>save</mat-icon>
            Update Review
          </button>
        </div>
      </form>
    </mat-card-content>
  </mat-card>
</div>
```

**File:** `angular-app/src/app/edit-review/edit-review.css` (NEW FILE)

Copy the exact same CSS from `create-review.css`.

##### Step 2.18: Add Routes

**File:** `angular-app/src/app/app.routes.ts`

Add the new routes under the Admin section:

```typescript
import { Routes } from '@angular/router';
import { Landing } from './landing/landing';
import { HomeComponent } from './home/home';

// Legacy imports (currently active)
import { IndexProducts } from './index-products/index-products';
import { CreateVegproduct } from './create-vegproduct/create-vegproduct';
import { EditVegproduct } from './edit-vegproduct/edit-vegproduct';
import { IndexVegcategories } from './index-vegcategories/index-vegcategories';
import { CreateVegcategory } from './create-vegcategory/create-vegcategory';
import { EditVegcategory } from './edit-vegcategory/edit-vegcategory';

// NEW - Import review components
import { IndexReviews } from './index-reviews/index-reviews';
import { CreateReview } from './create-review/create-review';
import { EditReview } from './edit-review/edit-review';

export const routes: Routes = [
    // ===================================
    // PUBLIC ROUTES - eCommerce Site
    // ===================================
    { 
        path: '', 
        component: HomeComponent,
        title: 'VeggyWorldShop - Fresh Organic Vegetables'
    },
    
    // ===================================
    // ADMIN ROUTES - Admin Panel
    // ===================================
    { 
        path: 'admin', 
        component: Landing,
        title: 'Admin Dashboard - VeggyWorldShop'
    },
    
    // Products Management
    { 
        path: 'admin/products', 
        component: IndexProducts,
        title: 'Manage Products - Admin'
    },
    { 
        path: 'admin/products/create', 
        component: CreateVegproduct,
        title: 'Create Product - Admin'
    },
    { 
        path: 'admin/products/edit/:id', 
        component: EditVegproduct,
        title: 'Edit Product - Admin'
    },
    
    // Categories Management
    { 
        path: 'admin/categories', 
        component: IndexVegcategories,
        title: 'Manage Categories - Admin'
    },
    { 
        path: 'admin/categories/create', 
        component: CreateVegcategory,
        title: 'Create Category - Admin'
    },
    { 
        path: 'admin/categories/edit/:id', 
        component: EditVegcategory,
        title: 'Edit Category - Admin'
    },
    
    // NEW - Reviews Management
    { 
        path: 'admin/reviews', 
        component: IndexReviews,
        title: 'Manage Reviews - Admin'
    },
    { 
        path: 'admin/reviews/create', 
        component: CreateReview,
        title: 'Create Review - Admin'
    },
    { 
        path: 'admin/reviews/edit/:id', 
        component: EditReview,
        title: 'Edit Review - Admin'
    },
    
    // Fallback for old admin URLs (redirect to new admin routes)
    { path: 'products', redirectTo: 'admin/products', pathMatch: 'full' },
    { path: 'products/create', redirectTo: 'admin/products/create', pathMatch: 'full' },
    { path: 'products/edit/:id', redirectTo: 'admin/products/edit/:id', pathMatch: 'full' },
    { path: 'categories', redirectTo: 'admin/categories', pathMatch: 'full' },
    { path: 'categories/create', redirectTo: 'admin/categories/create', pathMatch: 'full' },
    { path: 'categories/edit/:id', redirectTo: 'admin/categories/edit/:id', pathMatch: 'full' },
];
```

##### Step 2.19: Add Menu Link (Optional)

**File:** `angular-app/src/app/menu/menu.html`

Add a link to the reviews section in the admin menu:

```html
<mat-toolbar class="navbar">
  <div class="navbar-content">
    <div class="navbar-brand">
      <span class="brand-icon">🥗</span>
      <span class="app-title">VegShop</span>
      <span class="brand-tagline">Admin Panel</span>
    </div>

    <span class="spacer"></span>

    <nav class="navbar-actions">
      <a routerLink="/admin" mat-button routerLinkActive="active" [routerLinkActiveOptions]="{exact: true}" class="nav-action">
        <mat-icon>dashboard</mat-icon>
        <span>Dashboard</span>
      </a>
      <a routerLink="/admin/products" mat-button routerLinkActive="active" class="nav-action">
        <mat-icon>inventory_2</mat-icon>
        <span>Products</span>
      </a>
      <a routerLink="/admin/categories" mat-button routerLinkActive="active" class="nav-action">
        <mat-icon>category</mat-icon>
        <span>Categories</span>
      </a>
      <!-- NEW - Add this link -->
      <a routerLink="/admin/reviews" mat-button routerLinkActive="active" class="nav-action">
        <mat-icon>rate_review</mat-icon>
        <span>Reviews</span>
      </a>
      <button mat-icon-button class="nav-action-btn" matTooltip="Settings">
        <mat-icon>settings</mat-icon>
      </button>
    </nav>
  </div>
</mat-toolbar>
```

##### Step 2.20: Test the Complete Flow

1. **Start Backend:**
   ```powershell
   cd DotNetCoreWebApi/DotNetCoreWebApi
   dotnet run
   ```

2. **Start Frontend:**
   ```powershell
   cd angular-app
   npm start
   ```

3. **Test Complete Review Flow:**
   - Navigate to `http://localhost:4201/admin/reviews`
   - Click "Add Review" button
   - Select a product from dropdown
   - Enter reviewer name, rating, and comment
   - Click "Create Review"
   - Verify review appears in the list
   - Click Edit icon on a review
   - Modify the review details
   - Click "Update Review"
   - Verify changes are saved
   - Test the delete functionality
   - Verify the review is removed from the list

4. **Test Foreign Key Relationship:**
   - Try to create a review for a non-existent product ID using API (should fail)
   - Delete a product that has reviews (reviews should be deleted automatically due to CASCADE)
   - Verify the product name appears in the reviews list

---

### Summary: Key Differences Between Scenarios

**SCENARIO 1 (Adding Field):**
- Modify existing entity, DTOs, service layer
- Single migration
- Update forms in existing components
- No new routes or components needed

**SCENARIO 2 (New Entity with FK):**
- Create complete new entity structure (8 backend files, 3 frontend components)
- Configure foreign key relationship in DbContext
- Create repository, service, controller from scratch
- Create index, create, edit components
- Add new routes
- Update menu (optional)
- Test foreign key constraints

---
    <mat-option [value]="null">None</mat-option>
    <mat-option *ngFor="let parent of parentEntities" [value]="parent.id">
      {{ parent.name }}
    </mat-option>
  </mat-select>
</mat-form-field>
```

---

## Testing Guidelines

### Backend Testing

Create unit tests for services:

```csharp
[Fact]
public async Task CreateProduct_ValidData_ReturnsProductDto()
{
    // Arrange
    var mockRepo = new Mock<IVegProductRepository>();
    var service = new VegProductService(mockRepo.Object);
    var dto = new VegProductCreateUpdateDto { Name = "Test" };
    
    // Act
    var result = await service.CreateProductAsync(dto);
    
    // Assert
    Assert.NotNull(result);
    Assert.Equal("Test", result.Name);
}
```

### Frontend Testing

Tests should be placed alongside components:

```typescript
describe('ProductService', () => {
  it('should fetch all products', () => {
    const httpMock = TestBed.inject(HttpTestingController);
    service.getAll().subscribe(products => {
      expect(products.length).toBeGreaterThan(0);
    });
  });
});
```

---

## Deployment

### Development

**Backend:**
```powershell
cd DotNetCoreWebApi/DotNetCoreWebApi
dotnet run
```

**Frontend:**
```powershell
cd angular-app
npm start
```

### Production

**Backend:**
```powershell
dotnet publish -c Release -o ./publish
```

**Frontend:**
```powershell
ng build --configuration production
```

---

## Quick Reference Commands

### Backend
```powershell
# Run backend
dotnet run

# Create migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update

# Clean and rebuild
dotnet clean
dotnet build
```

### Frontend
```powershell
# Run frontend
npm start

# Build for production
npm run build

# Run tests
npm test
```

---

## Support

For issues or questions, refer to:
- Angular documentation: https://angular.dev
- .NET documentation: https://docs.microsoft.com/dotnet
- Entity Framework Core: https://docs.microsoft.com/ef/core

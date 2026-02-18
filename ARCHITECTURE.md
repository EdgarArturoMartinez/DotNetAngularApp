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

### Backend Steps

#### 1. Create Entity Class

`Application/Entities/NewEntity.cs`:
```csharp
public class NewEntity
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    // Add other properties
}
```

#### 2. Create DTOs

`DTOs/NewEntityDto.cs`:
```csharp
public class NewEntityDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}

public class NewEntityCreateUpdateDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}
```

#### 3. Update DbContext

`Application/DBContext/ApplicationDBContext.cs`:
```csharp
public DbSet<NewEntity> NewEntities { get; set; }

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<NewEntity>()
        .HasKey(e => e.Id);
}
```

#### 4. Create Repository Interface and Implementation

`Application/Interfaces/INewEntityRepository.cs`:
```csharp
public interface INewEntityRepository : IRepository<NewEntity>
{
    // Add specific methods if needed
}
```

`Infrastructure/Repositories/NewEntityRepository.cs`:
```csharp
public class NewEntityRepository : Repository<NewEntity>, INewEntityRepository
{
    public NewEntityRepository(ApplicationDBContext context) : base(context) { }
}
```

#### 5. Create Service Interface and Implementation

`Application/Interfaces/INewEntityService.cs`:
```csharp
public interface INewEntityService
{
    Task<IEnumerable<NewEntityDto>> GetAllAsync();
    Task<NewEntityDto?> GetByIdAsync(int id);
    Task<NewEntityDto> CreateAsync(NewEntityCreateUpdateDto dto);
    Task UpdateAsync(int id, NewEntityCreateUpdateDto dto);
    Task DeleteAsync(int id);
}
```

`Application/Services/NewEntityService.cs`:
```csharp
public class NewEntityService : INewEntityService
{
    private readonly INewEntityRepository _repository;
    
    public NewEntityService(INewEntityRepository repository)
    {
        _repository = repository;
    }
    
    // Implement all methods with DTO mapping
}
```

#### 6. Create Controller

`Controllers/NewEntitiesController.cs`:
```csharp
[ApiController]
[Route("api/[controller]")]
public class NewEntitiesController : ControllerBase
{
    private readonly INewEntityService _service;
    
    public NewEntitiesController(INewEntityService service)
    {
        _service = service;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<NewEntityDto>>> GetAll()
    {
        var entities = await _service.GetAllAsync();
        return Ok(entities);
    }
    
    // Add other endpoints
}
```

#### 7. Register Services in Program.cs

```csharp
builder.Services.AddScoped<INewEntityRepository, NewEntityRepository>();
builder.Services.AddScoped<INewEntityService, NewEntityService>();
```

#### 8. Create Migration

```powershell
cd DotNetCoreWebApi/DotNetCoreWebApi
dotnet ef migrations add AddNewEntity
dotnet ef database update
```

### Frontend Steps

#### 1. Add Interface to Models

`shared/models/entities.ts`:
```typescript
export interface NewEntity {
  id: number;
  name: string;
  description?: string;
}

export interface NewEntityCreateUpdate {
  name: string;
  description?: string;
}
```

#### 2. Create Service

`features/new-entities/services/new-entity.service.ts`:
```typescript
@Injectable({ providedIn: 'root' })
export class NewEntityService {
  private apiUrl = `${environment.apiURL}/api/newentities`;
  
  constructor(private http: HttpClient) { }
  
  getAll(): Observable<NewEntity[]> {
    return this.http.get<NewEntity[]>(this.apiUrl);
  }
  
  getById(id: number): Observable<NewEntity> {
    return this.http.get<NewEntity>(`${this.apiUrl}/${id}`);
  }
  
  create(data: NewEntityCreateUpdate): Observable<NewEntity> {
    return this.http.post<NewEntity>(this.apiUrl, data);
  }
  
  update(id: number, data: NewEntityCreateUpdate): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, data);
  }
  
  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
```

#### 3. Create Components

Follow the pattern from existing components:
- Create index component for listing
- Create create component for adding new entities
- Create edit component for updating entities

#### 4. Add Routes

`app.routes.ts`:
```typescript
{
  path: 'new-entities',
  component: IndexNewEntities
},
{
  path: 'new-entities/create',
  component: CreateNewEntity
},
{
  path: 'new-entities/edit/:id',
  component: EditNewEntity
}
```

### Adding Foreign Key Relationships

#### Backend

1. Add foreign key property to entity:
```csharp
public int? ParentEntityId { get; set; }
public virtual ParentEntity? ParentEntity { get; set; }
```

2. Configure relationship in DbContext:
```csharp
modelBuilder.Entity<ChildEntity>()
    .HasOne(c => c.ParentEntity)
    .WithMany(p => p.ChildEntities)
    .HasForeignKey(c => c.ParentEntityId)
    .OnDelete(DeleteBehavior.SetNull); // or Cascade, Restrict
```

3. Update DTOs to include foreign key:
```csharp
public class ChildEntityDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int? ParentEntityId { get; set; }
    public ParentEntityBasicDto? ParentEntity { get; set; }
}
```

#### Frontend

1. Update TypeScript interface:
```typescript
export interface ChildEntity {
  id: number;
  name: string;
  parentEntityId?: number;
  parentEntity?: ParentEntityBasic;
}
```

2. Add dropdown in form:
```html
<mat-form-field>
  <mat-label>Parent Entity</mat-label>
  <mat-select formControlName="parentEntityId">
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

# 🥬 Veggie World E-Commerce Admin Dashboard

**Status:** ✅ Production Ready | **Angular:** 21.1.0 | **Backend:** .NET Core | **Database:** SQL Server

---

## 📚 Table of Contents

1. [Project Structure](#-project-structure)
2. [Getting Started](#-getting-started)
3. [Backend Setup](#-backend-setup)
4. [Frontend Setup](#-frontend-setup)
5. [Daily Development Workflow](#-daily-development-workflow)
6. [Adding New Fields to Existing Tables](#-complete-guide-adding-new-fields)
7. [Creating New Tables with Relationships](#-complete-guide-creating-new-tables)
8. [Troubleshooting](#-troubleshooting)

---

## 🗂️ Project Structure

```
DotNetAngularApp/
├── DotNetCoreWebApi/                    # Backend (.NET Core)
│   └── DotNetCoreWebApi/
│       ├── Controllers/                 # API Endpoints
│       │   ├── VegProductsController.cs
│       │   ├── VegCategoriesController.cs
│       │   └── WeatherForecastController.cs
│       ├── Application/
│       │   ├── DBContext/               # Entity Framework DbContext
│       │   │   └── ApplicationDBContext.cs
│       │   └── Entities/                # Database Models
│       │       ├── VegProducts.cs
│       │       ├── VegCategory.cs
│       │       └── other entities...
│       ├── DTOs/                        # Data Transfer Objects
│       │   ├── VegProductDto.cs
│       │   ├── VegCategoryDto.cs
│       │   └── other DTOs...
│       ├── Migrations/                  # Entity Framework Migrations
│       ├── Program.cs                   # Backend Configuration
│       └── appsettings.*.json           # Configuration Files
│
├── angular-app/                          # Frontend (Angular)
│   ├── src/
│   │   ├── app/
│   │   │   ├── shared/                  # Shared Services and Utilities
│   │   │   │   ├── services/
│   │   │   │   ├── models/
│   │   │   │   └── interfaces/
│   │   │   ├── index-products/          # Products List Page
│   │   │   ├── create-vegproduct/       # Create Product Page
│   │   │   ├── edit-vegproduct/         # Edit Product Page
│   │   │   ├── index-vegcategories/     # Categories List Page
│   │   │   ├── create-vegcategory/      # Create Category Page
│   │   │   ├── edit-vegcategory/        # Edit Category Page
│   │   │   ├── landing/                 # Home Page
│   │   │   ├── menu/                    # Navigation Menu
│   │   │   ├── vegproduct.ts            # Product Service
│   │   │   ├── vegcategory.service.ts   # Category Service
│   │   │   ├── app.routes.ts            # Route Configuration
│   │   │   └── app.ts                   # Root Component
│   │   ├── environments/                # Environment Configuration
│   │   │   ├── environment.ts           # Production
│   │   │   └── environment.development.ts
│   │   ├── index.html
│   │   ├── main.ts
│   │   └── styles.css
│   ├── package.json                      # Dependencies
│   ├── angular.json                      # Angular CLI Config
│   ├── tsconfig.json                     # TypeScript Config
│   └── README.md                         # This file
│
└── FINAL-SOLUTION-READ-THIS.md          # Quick reference guide
```

---

## 🚀 Getting Started

### System Requirements

- **Node.js:** v20.x or higher
- **.NET SDK:** v8.0 or higher  
- **SQL Server:** Local or remote instance
- **Git:** For version control

### Installation

```powershell
# Clone the repository
git clone <repository-url>
cd DotNetAngularApp

# Backend setup (see Backend Setup below)
# Frontend setup (see Frontend Setup below)
```

---

## ⚙️ Backend Setup

### Step 1: Open SQL Server Management Studio

Ensure your SQL Server is running and accessible. The connection string is in:

```
DotNetCoreWebApi/DotNetCoreWebApi/appsettings.json
```

### Step 2: Start the Backend

Open PowerShell and run:

```powershell
# Navigate to backend directory
cd "c:\Arthur\Development\2026\DotNetAngularApp\DotNetCoreWebApi\DotNetCoreWebApi"

# Clean previous build (VERY IMPORTANT for changes to take effect)
dotnet clean

# Restore NuGet packages
dotnet restore

# Start the backend server
dotnet run
```

**Expected Output:**
```
Now listening on: https://localhost:7020
```

✅ **LEAVE THIS WINDOW OPEN** while developing

### Verify Backend is Running

Open PowerShell and test:

```powershell
# Test if backend is responding
Invoke-WebRequest -Uri "https://localhost:7020/api/vegproducts" `
    -SkipCertificateCheck

# Should return: StatusCode 200 + list of products
```

---

## 🎨 Frontend Setup

### Step 1: Install Dependencies

Open **NEW PowerShell window** and run:

```powershell
cd "c:\Arthur\Development\2026\DotNetAngularApp\angular-app"

# Install npm dependencies
npm install
```

### Step 2: Start Angular Dev Server

```powershell
npm start
```

**Expected Output:**
```
✓ Application bundle generation complete.
➜  Local:   http://localhost:4200/
```

### Step 3: Open in Browser

- Click the link or navigate to: **`http://localhost:4200`**
- You should see the Veggie World dashboard

✅ **LEAVE THIS WINDOW OPEN** while developing

---

## 📝 Daily Development Workflow

### Working on Code

1. **Backend changes:**
   - Edit code in `DotNetCoreWebApi/` folder
   - Backend auto-reloads (watch mode enabled)

2. **Frontend changes:**
   - Edit code in `angular-app/src/` folder
   - Browser auto-reloads (watch mode enabled)

### How to Kill and Restart Angular (Port 4200)

**When:** You want a clean restart or the app is stuck

#### Option A: Using PowerShell (Recommended)

```powershell
# Kill all Node processes
Get-Process | Where-Object { $_.ProcessName -match 'node' } | Stop-Process -Force -ErrorAction SilentlyContinue

# Wait for cleanup
Start-Sleep -Seconds 3

# Navigate to angular app
cd "c:\Arthur\Development\2026\DotNetAngularApp\angular-app"

# Start again
npm start
```

#### Option B: Using Task Manager

1. Open **Task Manager** (`Ctrl + Shift + Esc`)
2. Find **node.exe** in the list
3. Right-click → **End Task**
4. In PowerShell, run `npm start` again

#### Option C: Kill Specific Port

```powershell
# Kill the process using port 4200
netstat -ano | findstr :4200
# Note the PID from output, then:
taskkill /PID <PID> /F

# Then restart: npm start
```

### How to Kill and Restart Backend (Port 7020)

**When:** You want a clean restart after making code changes

```powershell
# Kill all dotnet processes
Get-Process | Where-Object { $_.ProcessName -match 'dotnet' } | Stop-Process -Force -ErrorAction SilentlyContinue

Start-Sleep -Seconds 3

# Navigate to backend
cd "c:\Arthur\Development\2026\DotNetAngularApp\DotNetCoreWebApi\DotNetCoreWebApi"

# Clean and restart (IMPORTANT: dotnet clean is required for code changes)
dotnet clean
dotnet run
```

---

## 🎯 Complete Guide: Adding New Fields

**Scenario:** You want to add a "Stock Quantity" field to the VegProducts table.

### Step 1: Database Migration (Backend)

#### 1.1 Open Package Manager Console

In **Visual Studio** or **VS Code**:
- Open Terminal → go to `DotNetCoreWebApi/DotNetCoreWebApi`

#### 1.2 Create Migration

```powershell
# Navigate to backend directory
cd "DotNetCoreWebApi/DotNetCoreWebApi"

# Create migration (replace "AddStockQuantity" with your field name)
dotnet ef migrations add AddStockQuantity
```

This creates a new migration file in the `Migrations` folder.

#### 1.3 Verify Migration File

Open the generated file in `Migrations/` folder:

```csharp
// Example migration file structure
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.AddColumn<int>(
        name: "StockQuantity",
        table: "VegProducts",
        type: "int",
        nullable: false,
        defaultValue: 0);  // Default value
}

protected override void Down(MigrationBuilder migrationBuilder)
{
    migrationBuilder.DropColumn(
        name: "StockQuantity",
        table: "VegProducts");
}
```

#### 1.4 Apply Migration

```powershell
# Apply the migration to database
dotnet ef database update
```

✅ Migration is now in the database!

### Step 2: Update Backend Entity Model

#### 2.1 Open Entity File

File: `DotNetCoreWebApi/DotNetCoreWebApi/Application/Entities/VegProducts.cs`

#### 2.2 Add Property

```csharp
public class VegProducts
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string? Description { get; set; }
    
    // ADD THIS LINE:
    public int StockQuantity { get; set; }
    
    public int? IdCategory { get; set; }
    public VegCategory? VegCategory { get; set; }
}
```

### Step 3: Update DTO

#### 3.1 Open DTO File

File: `DotNetCoreWebApi/DotNetCoreWebApi/DTOs/VegProductDto.cs`

#### 3.2 Add Property

```csharp
public class VegProductDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string? Description { get; set; }
    
    // ADD THIS LINE:
    public int StockQuantity { get; set; }
    
    public int? IdCategory { get; set; }
    public VegCategoryBasicDto? VegCategory { get; set; }
}
```

### Step 4: Update Controller

#### 4.1 Open Controller File

File: `DotNetCoreWebApi/DotNetCoreWebApi/Controllers/VegProductsController.cs`

#### 4.2 Update Mapping

In the `GetAllAsync()` method, add the field:

```csharp
var productDtos = products.Select(p => new VegProductDto
{
    Id = p.Id,
    Name = p.Name,
    Price = p.Price,
    Description = p.Description,
    StockQuantity = p.StockQuantity,  // ADD THIS
    IdCategory = p.IdCategory,
    VegCategory = ...
}).ToList();
```

Do the same in `GetByIdAsync()` method.

### Step 5: Restart Backend

```powershell
cd "c:\Arthur\Development\2026\DotNetAngularApp\DotNetCoreWebApi\DotNetCoreWebApi"
dotnet clean
dotnet run
```

✅ Backend now includes the new field!

### Step 6: Update Angular Models (Frontend)

#### 6.1 Open Model File

File: `angular-app/src/app/vegproduct.ts`

```typescript
export interface VegProduct {
  id: number;
  name: string;
  price: number;
  description?: string;
  stockQuantity?: number;  // ADD THIS
  idCategory?: number;
  vegCategory?: VegCategory;
}
```

#### 6.2 Update Service if Needed

File: `angular-app/src/app/vegproduct.ts`

The service should automatically work with the new field in responses.

### Step 7: Update Angular Forms (Frontend)

#### 7.1 Create Form Component

File: `angular-app/src/app/create-vegproduct/create-vegproduct.ts`

```typescript
vegProductForm = this.formBuilder.group({
  name: ['', Validators.required],
  price: ['', Validators.required],
  description: [''],
  stockQuantity: [0, Validators.required],  // ADD THIS
  idCategory: [null as number | null]
});
```

#### 7.2 Update Template

File: `angular-app/src/app/create-vegproduct/create-vegproduct.html`

Add form field before submit button:

```html
<div class="form-field">
  <mat-form-field appearance="outline" class="full-width">
    <mat-label>Stock Quantity</mat-label>
    <input matInput formControlName="stockQuantity" type="number" placeholder="e.g., 50">
    <mat-icon matSuffix>inventory</mat-icon>
    <mat-error *ngIf="vegProductForm.get('stockQuantity')?.hasError('required')">
      Stock quantity is required
    </mat-error>
  </mat-form-field>
</div>
```

#### 7.3 Update Display Component

File: `angular-app/src/app/index-products/index-products.html`

Add in the product card:

```html
<p><strong>Stock:</strong> {{ product.stockQuantity }}</p>
```

### Step 8: Restart Angular

```powershell
# Kill and restart
Get-Process | Where-Object { $_.ProcessName -match 'node' } | Stop-Process -Force -ErrorAction SilentlyContinue
Start-Sleep -Seconds 3

cd "c:\Arthur\Development\2026\DotNetAngularApp\angular-app"
npm start
```

### Step 9: Test

1. Navigate to http://localhost:4200
2. Click **Create Product**
3. Fill in all fields including **Stock Quantity**
4. Click **Create**
5. ✅ Product should appear with stock quantity

---

## 🆕 Complete Guide: Creating New Tables with Foreign Keys

**Scenario:** Create a new `VegTypeProduct` table (e.g., Organic, Conventional) and link it to VegProducts.

### Architecture

```
VegTypeProduct (New Table)
├── IdType (PK)
├── TypeName
└── Description

VegProducts (Existing)
├── Id (PK)
├── Name
├── Price
├── IdCategory (FK) → VegCategory
└── IdType (NEW FK) → VegTypeProduct  ← Adding this relationship
```

---

### Step 1: Create Backend Entity (VegTypeProduct)

#### 1.1 Create Entity File

Path: `DotNetCoreWebApi/DotNetCoreWebApi/Application/Entities/VegTypeProduct.cs`

```csharp
using System.ComponentModel.DataAnnotations.Schema;

namespace DotNetCoreWebApi.Application.Entities
{
    [Table("VegTypeProducts")]
    public class VegTypeProduct
    {
        [Column("IdType")]
        public int IdType { get; set; }

        [Column("TypeName")]
        public string TypeName { get; set; } = string.Empty;

        [Column("Description")]
        public string? Description { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation property (one type has many products)
        public virtual ICollection<VegProducts> VegProducts { get; set; } = new List<VegProducts>();
    }
}
```

### Step 2: Update VegProducts Entity

#### 2.1 Add Foreign Key Property

File: `DotNetCoreWebApi/DotNetCoreWebApi/Application/Entities/VegProducts.cs`

```csharp
public class VegProducts
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string? Description { get; set; }
    
    // EXISTING:
    public int? IdCategory { get; set; }
    public VegCategory? VegCategory { get; set; }
    
    // ADD THESE:
    public int? IdType { get; set; }  // Foreign Key
    public VegTypeProduct? VegType { get; set; }  // Navigation Property
}
```

### Step 3: Update DbContext

#### 3.1 Open DbContext File

File: `DotNetCoreWebApi/DotNetCoreWebApi/Application/DBContext/ApplicationDBContext.cs`

#### 3.2 Add DbSet

```csharp
public class ApplicationDBContext : DbContext
{
    // Existing DbSets...
    public DbSet<VegProducts> VegProducts { get; set; }
    public DbSet<VegCategory> VegCategories { get; set; }
    
    // ADD THIS:
    public DbSet<VegTypeProduct> VegTypeProducts { get; set; }
    
    // Rest of DbContext...
}
```

### Step 4: Create Database Migration

```powershell
cd "DotNetCoreWebApi/DotNetCoreWebApi"

# Create migration for new table
dotnet ef migrations add CreateVegTypeProductTable

# Apply to database
dotnet ef database update
```

### Step 5: Create DTOs

#### 5.1 Create VegTypeProductDto

Path: `DotNetCoreWebApi/DotNetCoreWebApi/DTOs/VegTypeProductDto.cs`

```csharp
namespace DotNetCoreWebApi.DTOs
{
    public class VegTypeProductDto
    {
        public int IdType { get; set; }
        public string TypeName { get; set; }
        public string? Description { get; set; }
    }
}
```

#### 5.2 Update VegProductDto

File: `DotNetCoreWebApi/DotNetCoreWebApi/DTOs/VegProductDto.cs`

```csharp
public class VegProductDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public int? IdCategory { get; set; }
    public VegCategoryBasicDto? VegCategory { get; set; }
    
    // ADD THESE:
    public int? IdType { get; set; }
    public VegTypeProductDto? VegType { get; set; }
}
```

### Step 6: Create Backend Controller

Path: `DotNetCoreWebApi/DotNetCoreWebApi/Controllers/VegTypeProductsController.cs`

```csharp
using DotNetCoreWebApi.Application.DBContext;
using DotNetCoreWebApi.Application.Entities;
using DotNetCoreWebApi.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotNetCoreWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VegTypeProductsController : ControllerBase
    {
        private readonly ApplicationDBContext context;

        public VegTypeProductsController(ApplicationDBContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VegTypeProductDto>>> GetTypes()
        {
            var types = await context.VegTypeProducts.AsNoTracking().ToListAsync();
            var typeDtos = types.Select(t => new VegTypeProductDto
            {
                IdType = t.IdType,
                TypeName = t.TypeName,
                Description = t.Description
            }).ToList();

            return Ok(typeDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VegTypeProductDto>> GetTypeById(int id)
        {
            var type = await context.VegTypeProducts.AsNoTracking()
                .FirstOrDefaultAsync(t => t.IdType == id);

            if (type == null)
                return NotFound();

            return Ok(new VegTypeProductDto
            {
                IdType = type.IdType,
                TypeName = type.TypeName,
                Description = type.Description
            });
        }

        [HttpPost]
        public async Task<ActionResult> CreateType(VegTypeProduct type)
        {
            context.VegTypeProducts.Add(type);
            await context.SaveChangesAsync();
            return Ok(type);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateType(int id, VegTypeProduct type)
        {
            if (id != type.IdType)
                return BadRequest();

            context.Entry(type).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteType(int id)
        {
            var type = await context.VegTypeProducts.FindAsync(id);
            if (type == null)
                return NotFound();

            context.VegTypeProducts.Remove(type);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
```

### Step 7: Update VegProductsController

File: `DotNetCoreWebApi/DotNetCoreWebApi/Controllers/VegProductsController.cs`

Update the `GetAllAsync()` and `GetByIdAsync()` methods to include `.Include(p => p.VegType)`:

```csharp
[HttpGet]
public async Task<ActionResult<IEnumerable<VegProductDto>>> GetAllAsync()
{
    var products = await context.VegProducts
        .Include(p => p.VegCategory)
        .Include(p => p.VegType)  // ADD THIS
        .AsNoTracking()
        .ToListAsync();

    var productDtos = products.Select(p => new VegProductDto
    {
        Id = p.Id,
        Name = p.Name,
        Price = p.Price,
        Description = p.Description,
        IdCategory = p.IdCategory,
        VegCategory = ...,
        IdType = p.IdType,  // ADD THIS
        VegType = p.VegType == null ? null : new VegTypeProductDto  // ADD THIS
        {
            IdType = p.VegType.IdType,
            TypeName = p.VegType.TypeName,
            Description = p.VegType.Description
        }
    }).ToList();

    return Ok(productDtos);
}
```

### Step 8: Restart Backend

```powershell
cd "c:\Arthur\Development\2026\DotNetAngularApp\DotNetCoreWebApi\DotNetCoreWebApi"
dotnet clean
dotnet run
```

### Step 9: Create Angular Models

File: `angular-app/src/app/vegproduct.ts`

Add VegTypeProduct interface:

```typescript
export interface VegTypeProduct {
  idType: number;
  typeName: string;
  description?: string;
}

export interface VegProduct {
  id: number;
  name: string;
  price: number;
  description?: string;
  idCategory?: number;
  vegCategory?: VegCategory;
  idType?: number;  // ADD THIS
  vegType?: VegTypeProduct;  // ADD THIS
}
```

### Step 10: Create Angular Service

File: `angular-app/src/app/vegtype.service.ts`

```typescript
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class VegTypeService {
  private apiUrl = `${environment.apiURL}/api/vegtypeproducts`;

  constructor(private http: HttpClient) { }

  getVegTypes(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  getVegTypeById(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  createVegType(data: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, data);
  }

  updateVegType(id: number, data: any): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, data);
  }

  deleteVegType(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
```

### Step 11: Update Create Product Form

File: `angular-app/src/app/create-vegproduct/create-vegproduct.ts`

```typescript
// Add to component class
vegTypes: any[] = [];

vegProductForm = this.formBuilder.group({
  name: ['', Validators.required],
  price: ['', Validators.required],
  idCategory: [null as number | null],
  idType: [null as number | null]  // ADD THIS
});

ngOnInit() {
  this.loadCategories();
  this.loadVegTypes();  // ADD THIS
}

loadVegTypes() {  // ADD THIS METHOD
  this.vegTypeService.getVegTypes().subscribe({
    next: (data) => {
      this.vegTypes = data;
    },
    error: (error) => {
      console.error('Error loading types:', error);
    }
  });
}
```

### Step 12: Update Create Product Template

File: `angular-app/src/app/create-vegproduct/create-vegproduct.html`

Add dropdown before submit button:

```html
<div class="form-field">
  <mat-form-field appearance="outline" class="full-width">
    <mat-label>Product Type</mat-label>
    <mat-select formControlName="idType">
      <mat-option></mat-option>
      <mat-option *ngFor="let type of vegTypes" [value]="type.idType">
        {{ type.typeName }}
      </mat-option>
    </mat-select>
    <mat-icon matSuffix>category</mat-icon>
  </mat-form-field>
</div>
```

### Step 13: Restart Angular

```powershell
Get-Process | Where-Object { $_.ProcessName -match 'node' } | Stop-Process -Force -ErrorAction SilentlyContinue
Start-Sleep -Seconds 3

cd "c:\Arthur\Development\2026\DotNetAngularApp\angular-app"
npm start
```

### Step 14: Test the Complete Feature

1. Navigate to http://localhost:4200
2. Go to **Admin** section (or add menu item for VegTypeProducts)
3. Create a new **VegTypeProduct** (e.g., "Organic", "Conventional")
4. Create a new **VegProduct** and select the type
5. ✅ Product should save with type relationship

---

## 🐛 Troubleshooting

### Angular Won't Start (Port 4200)

```powershell
# Quick fix: Kill all Node processes
Get-Process | Where-Object { $_.ProcessName -match 'node' } | Stop-Process -Force
Start-Sleep -Seconds 3
npm start
```

### Backend Not Responding

```powershell
# Check if running on 7020
netstat -ano | findstr :7020

# Restart
dotnet clean
dotnet run
```

### Database Connection Error

**Check in:** `appsettings.json`

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=YOUR_DB;Integrated Security=true;Encrypt=false"
}
```

### Migrations Won't Apply

```powershell
# Remove last migration
dotnet ef migrations remove

# Recreate and apply
dotnet ef migrations add YourMigration
dotnet ef database update
```

### "No route matches the supplied values" Error

This means the backend POST endpoint is broken:

1. **Ensure both controllers return `Ok()` not `CreatedAtAction()`**
2. **Restart backend with `dotnet clean` then `dotnet run`**
3. **Clear browser cache** (`Ctrl + Shift + Delete`)

---

## ✅ Quick Commands Reference

```powershell
# === Backend Commands ===
# Clean and start
cd "DotNetCoreWebApi/DotNetCoreWebApi"
dotnet clean
dotnet run

# Create migration for new field
dotnet ef migrations add AddFieldName

# Apply migration
dotnet ef database update

# Remove last migration (if mistake)
dotnet ef migrations remove

# === Angular Commands ===
# Start dev server
npm start

# Kill port 4200
Get-Process | Where-Object { $_.ProcessName -match 'node' } | Stop-Process -Force

# Build for production
ng build

# Run tests
npm test
```

---

## 📞 Getting Help

**For database issues:** Check `appsettings.json` connection string
**For API errors:** Check backend terminal output
**For UI issues:** Check browser console (`F12`)
**For routing:** Ensure all imports are correct and routes defined

---

## 🎯 Summary

- ✅ **Adding Fields:** Add to Entity → Migration → DTO → Controller → Frontend
- ✅ **New Tables:** Create Entity → Add DbSet → Migration → Controller → Angular
- ✅ **Foreign Keys:** Always use `.Include()` in queries
- ✅ **Changes:** Always `dotnet clean` before `dotnet run`
- ✅ **Angular:** `npm start` auto-reloads on changes

**You now have everything you need to extend this application independently!** 🚀

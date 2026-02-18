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

## 🎯 Complete Guide: Adding New Fields to Existing Tables

**Scenario:** You want to add a "Stock Quantity" field to the VegProducts table, or any other field to any existing table.

**What You'll Learn:** How to add a new database column, update entity models, DTOs, controllers, and frontend forms in a complete end-to-end process.

---

### Step 1: Create Database Migration (Backend)

**Purpose:** Create a migration file that will add the new column to the database.

#### 1.1 Open Terminal and Navigate to Backend

```powershell
# Open PowerShell and navigate to the backend directory
cd "c:\Arthur\Development\2026\DotNetAngularApp\DotNetCoreWebApi\DotNetCoreWebApi"
```

#### 1.2 Create Migration File

```powershell
# Create a migration with a descriptive name (use PascalCase)
# Format: dotnet ef migrations add [MigrationName]
dotnet ef migrations add AddStockQuantity

# Example output:
# Build started...
# Build succeeded.
# Successfully added migration 'AddStockQuantity'.
```

**What This Does:** Creates a new migration file in the `Migrations` folder with timestamp.

#### 1.3 Verify Migration File Was Created

Open: `DotNetCoreWebApi/DotNetCoreWebApi/Migrations/[timestamp]_AddStockQuantity.cs`

You should see:

```csharp
public partial class AddStockQuantity : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "StockQuantity",
            table: "VegProducts",
            type: "int",
            nullable: false,
            defaultValue: 0);  // ← Default value for existing rows
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "StockQuantity",
            table: "VegProducts");
    }
}
```

**Explanation:**
- `Up()`: Runs when you apply the migration (adds the column)
- `Down()`: Runs if you rollback (removes the column)
- `defaultValue: 0`: Sets initial value for all existing products

#### 1.4 Apply Migration to Database

```powershell
# Apply the migration to the SQL Server database
dotnet ef database update

# Expected output:
# Build started...
# Build succeeded.
# Done. Building model for context 'ApplicationDBContext'.
# Applying migration '20260217120000_AddStockQuantity'.
# Done.
```

✅ **New column is now in your database!**

---

### Step 2: Update C# Entity Model (Backend)

**Purpose:** Add the new property to the Entity class that represents your database table.

#### 2.1 Open the VegProducts Entity File

File Path: `DotNetCoreWebApi/DotNetCoreWebApi/Application/Entities/VegProducts.cs`

#### 2.2 Add the New Property

Your entity should look like this:

```csharp
namespace DotNetCoreWebApi.Application.Entities;

public class VegProducts
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    
    // ADD YOUR NEW FIELD HERE:
    public int StockQuantity { get; set; }  // ← Matches migration column name exactly

    // Existing foreign key
    public int? IdCategory { get; set; }
    public virtual VegCategory? VegCategory { get; set; }
}
```

**Important Notes:**
- Property name must match the column name in migration (case-sensitive)
- Use appropriate C# data type (int, string, decimal, DateTime, etc.)
- Add `?` for nullable fields (optional): `public string? Description`
- No `?` for required fields: `public int StockQuantity`

---

### Step 3: Update Data Transfer Object (DTO) (Backend)

**Purpose:** The DTO is what the API returns to the frontend. It must match the Entity.

#### 3.1 Open VegProductDto File

File Path: `DotNetCoreWebApi/DotNetCoreWebApi/DTOs/VegProductDto.cs`

#### 3.2 Add the Same Property to DTO

```csharp
namespace DotNetCoreWebApi.DTOs;

public class VegProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public string? Description { get; set; }
    
    // ADD YOUR NEW FIELD HERE (same as entity):
    public int StockQuantity { get; set; }

    public int? IdCategory { get; set; }
    public VegCategoryBasicDto? VegCategory { get; set; }
}
```

**Why?** The DTO ensures only necessary fields are sent to the frontend, and it provides type safety.

---

### Step 4: Update Backend Controller (Backend)

**Purpose:** Map the Entity data to DTO in the controller methods.

#### 4.1 Open VegProductsController

File Path: `DotNetCoreWebApi/DotNetCoreWebApi/Controllers/VegProductsController.cs`

#### 4.2 Update GetAllAsync() Method

Find this method and add the new field to the mapping:

```csharp
[HttpGet]
public async Task<ActionResult<IEnumerable<VegProductDto>>> GetAllAsync()
{
    var products = await context.VegProducts
        .Include(p => p.VegCategory)
        .AsNoTracking()
        .ToListAsync();

    var productDtos = products.Select(p => new VegProductDto
    {
        Id = p.Id,
        Name = p.Name,
        Price = p.Price,
        Description = p.Description,
        StockQuantity = p.StockQuantity,  // ← ADD THIS LINE
        IdCategory = p.IdCategory,
        VegCategory = p.VegCategory == null ? null : new VegCategoryBasicDto
        {
            IdCategory = p.VegCategory.IdCategory,
            CategoryName = p.VegCategory.CategoryName,
            Description = p.VegCategory.Description
        }
    }).ToList();

    return Ok(productDtos);
}
```

#### 4.3 Update GetByIdAsync() Method

Find this method and add the same field:

```csharp
[HttpGet("{id}")]
public async Task<ActionResult<VegProductDto>> GetByIdAsync(int id)
{
    var vegProduct = await context.VegProducts
        .Include(p => p.VegCategory)
        .AsNoTracking()
        .FirstOrDefaultAsync(p => p.Id == id);
    
    if (vegProduct == null)
        return NotFound();

    var dto = new VegProductDto
    {
        Id = vegProduct.Id,
        Name = vegProduct.Name,
        Price = vegProduct.Price,
        Description = vegProduct.Description,
        StockQuantity = vegProduct.StockQuantity,  // ← ADD THIS LINE
        IdCategory = vegProduct.IdCategory,
        VegCategory = vegProduct.VegCategory == null ? null : new VegCategoryBasicDto
        {
            IdCategory = vegProduct.VegCategory.IdCategory,
            CategoryName = vegProduct.VegCategory.CategoryName,
            Description = vegProduct.VegCategory.Description
        }
    };

    return dto;
}
```

#### 4.4 Update PutVegproduct() Method (for Edit)

Find this method and add the field to the update:

```csharp
[HttpPut("{id}")]
public async Task<IActionResult> PutVegproduct(int id, [FromBody] VegProducts vegproduct)
{
    if (id != vegproduct.Id)
        return BadRequest("ID mismatch");

    if (string.IsNullOrWhiteSpace(vegproduct.Name))
        return BadRequest("Product name is required");

    var existingProduct = await context.VegProducts.FindAsync(id);
    if (existingProduct == null)
        return NotFound();

    // Update all fields including the new one
    existingProduct.Name = vegproduct.Name;
    existingProduct.Price = vegproduct.Price;
    existingProduct.Description = vegproduct.Description;
    existingProduct.StockQuantity = vegproduct.StockQuantity;  // ← ADD THIS LINE
    existingProduct.IdCategory = vegproduct.IdCategory == 0 ? null : vegproduct.IdCategory;

    await context.SaveChangesAsync();
    return NoContent();
}
```

---

### Step 5: Restart Backend & Test API

**Purpose:** Reload the backend with your changes.

#### 5.1 Stop Current Backend

```powershell
# Kill all current dotnet processes
Get-Process | Where-Object { $_.ProcessName -match 'dotnet' } | Stop-Process -Force -ErrorAction SilentlyContinue
Start-Sleep -Seconds 3
```

#### 5.2 Clean and Restart

```powershell
# Navigate to backend directory
cd "c:\Arthur\Development\2026\DotNetAngularApp\DotNetCoreWebApi\DotNetCoreWebApi"

# Clean the build
dotnet clean

# Start the backend
dotnet run
```

**Expected Output:**
```
Now listening on: https://localhost:7020
Application started. Press Ctrl+C to shut down.
```

#### 5.3 Quick API Test (Optional)

```powershell
# Test the API in a new PowerShell window
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}
Invoke-RestMethod -Uri "https://localhost:7020/api/vegproducts/1" | ConvertTo-Json
```

✅ **Backend is ready with the new field!**

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

---

### Step 6: Update Angular TypeScript Models (Frontend)

**Purpose:** Update the TypeScript interfaces so Angular knows about the new field with proper type checking.

#### 6.1 Update Main Model Interface

File Path: `angular-app/src/app/vegproduct.ts`

```typescript
import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { VegProductCreation } from './vegproduct.models';
import { environment } from '../environments/environment';
import { VegCategory } from './vegcategory';

@Injectable({
  providedIn: 'root',
})
export class Vegproduct {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiURL}/api/vegproducts`;

  // ... service methods ...
}

// ← VegProduct interface for API responses
export interface VegProduct {
  id: number;
  name: string;
  price: number;
  description?: string;
  
  // ADD THIS NEW FIELD:
  stockQuantity?: number;  // ← New field from API
  
  idCategory?: number;
  vegCategory?: VegCategory;
}
```

**Explanation:**
- `?: number` means "optional number field"
- Used when reading from API (GET requests)
- Matches the VegProductDto from backend

#### 6.2 Update Creation Interface

File Path: `angular-app/src/app/vegproduct.models.ts`

```typescript
// Interface for creating/updating products (POST/PUT)
export interface VegProductCreation {
    id?: number;
    name: string;
    price: number;
    description?: string;
    
    // ADD THIS NEW FIELD:
    stockQuantity?: number;  // ← New field for form submission
    
    idCategory?: number | null;
}
```

**Explanation:**
- Used when submitting forms (CREATE and EDIT)
- Must match the request body sent to backend
- Can be optional if you want to allow skipping it

---

### Step 7: Update Create Product Form (Frontend)

**Purpose:** Add the new field to the create-vegproduct form so users can input the value.

#### 7.1 Update Create Component TypeScript

File Path: `angular-app/src/app/create-vegproduct/create-vegproduct.ts`

Find the `vegProductForm` definition and add the new field:

```typescript
export class CreateVegproduct implements OnInit {
  vegProductForm = this.formBuilder.group({
    name: ['', Validators.required],
    price: ['', Validators.required],
    description: [''],
    
    // ADD THIS NEW FIELD:
    stockQuantity: [0, Validators.required],  // Default 0, required field
    
    idCategory: [null as number | null]
  });

  // ... rest of component ...
}
```

Find the `saveChanges()` method and add the field to the payload:

```typescript
saveChanges() {
  if (this.vegProductForm.valid) {
    const formValue = this.vegProductForm.value;
    const vegProductData: any = {
      name: formValue.name,
      price: parseFloat(formValue.price || '0'),
      
      // ADD THIS NEW FIELD:
      stockQuantity: formValue.stockQuantity || 0,  // ← Send to backend
      
      idCategory: formValue.idCategory || null
    };
    
    this.vegProduct.createVegproduct(vegProductData).subscribe({
      next: (response) => {
        this.snackBar.open(`✓ Product "${formValue.name}" created successfully!`, 'Close', {
          duration: 4000,
          horizontalPosition: 'end',
          verticalPosition: 'bottom',
          panelClass: ['success-snackbar']
        });
        
        setTimeout(() => {
          this.router.navigate(['/products']);
        }, 1000);
      },
      error: (error) => {
        this.snackBar.open(`✗ Error: ${error}`, 'Close', { duration: 5000 });
      }
    });
  }
}
```

#### 7.2 Update Create Component Template

File Path: `angular-app/src/app/create-vegproduct/create-vegproduct.html`

Add this form field before the submit button:

```html
<div class="form-field">
  <mat-form-field appearance="outline" class="full-width">
    <mat-label>Stock Quantity</mat-label>
    <input matInput 
           formControlName="stockQuantity" 
           type="number" 
           placeholder="e.g., 50"
           min="0">
    <mat-icon matSuffix>inventory</mat-icon>
    <mat-error *ngIf="vegProductForm.get('stockQuantity')?.hasError('required')">
      Stock quantity is required
    </mat-error>
  </mat-form-field>
</div>

<!-- Submit button below -->
<div class="form-actions">
  <button mat-flat-button type="submit" class="submit-btn" [disabled]="!vegProductForm.valid">
    <mat-icon>check_circle</mat-icon>
    Create Product
  </button>
  <a mat-stroked-button routerLink="/products" class="cancel-btn">
    <mat-icon>close</mat-icon>
    Cancel
  </a>
</div>
```

**Form Field Explanation:**
- `min="0"`: Prevents negative numbers
- `type="number"`: Shows numeric keyboard on mobile
- `required` validator: User must fill this field
- `<mat-error>`: Shows validation message if empty

---

### Step 8: Update Edit Product Form (Frontend)

**Purpose:** Add the same field to the edit form so users can update existing products.

#### 8.1 Update Edit Component TypeScript

File Path: `angular-app/src/app/edit-vegproduct/edit-vegproduct.ts`

The form definition should already have the field from earlier updates. Verify the `saveChanges()` method includes it:

```typescript
saveChanges() {
  console.log('saveChanges called');
  console.log('Form value:', this.vegProductForm.value);
  
  if (this.vegProductForm.valid && this.productId) {
    const formValue = this.vegProductForm.value;
    
    const vegProductData: any = {
      id: this.productId,
      name: String(formValue.name).trim(),
      price: parseFloat(String(formValue.price)),
      description: '',
      
      // ADD THIS NEW FIELD:
      stockQuantity: formValue.stockQuantity || 0,  // ← Send updated value
      
      idCategory: formValue.idCategory || null
    };
    
    console.log('Submitting product data:', vegProductData);
    
    this.vegProduct.updateVegproduct(this.productId, vegProductData).subscribe({
      next: () => {
        this.snackBar.open(`✓ Product updated successfully!`, 'Close', {
          duration: 4000,
          horizontalPosition: 'end',
          verticalPosition: 'bottom',
          panelClass: ['success-snackbar']
        });
        
        setTimeout(() => {
          this.router.navigate(['/products']);
        }, 1000);
      },
      error: (error) => {
        this.snackBar.open(`✗ Error: ${error}`, 'Close', { duration: 5000 });
      }
    });
  }
}
```

#### 8.2 Update Edit Component Template

File Path: `angular-app/src/app/edit-vegproduct/edit-vegproduct.html`

Add this form field after the category field:

```html
<div class="form-field">
  <mat-form-field appearance="outline" class="full-width">
    <mat-label>Stock Quantity</mat-label>
    <input matInput 
           formControlName="stockQuantity" 
           type="number" 
           placeholder="e.g., 50"
           min="0">
    <mat-icon matSuffix>inventory</mat-icon>
    <mat-error *ngIf="vegProductForm.get('stockQuantity')?.hasError('required')">
      Stock quantity is required
    </mat-error>
  </mat-form-field>
</div>
```

---

### Step 9: Update Product List Display (Frontend)

**Purpose:** Show the new field in the products grid so users can see the data.

#### 9.1 Update Index Products Template

File Path: `angular-app/src/app/index-products/index-products.html`

Find the stock section and make sure it displays the field:

```html
<div class="stock-section">
  <span class="stock-label">Stock</span>
  <div class="stock-display">
    <span class="stock-value">{{ product.stockQuantity }} units</span>
    <!-- Low stock warning -->
    <span class="low-stock-warning" *ngIf="product.stockQuantity <= 10">
      ⚠️ Low
    </span>
  </div>
</div>
```

---

### Step 10: Restart Angular Dev Server

**Purpose:** Reload Angular with all the changes.

#### 10.1 Stop Angular

```powershell
# Kill all Node processes
Get-Process | Where-Object { $_.ProcessName -match 'node' } | Stop-Process -Force -ErrorAction SilentlyContinue
Start-Sleep -Seconds 3
```

#### 10.2 Start Angular

```powershell
cd "c:\Arthur\Development\2026\DotNetAngularApp\angular-app"
npm start
```

**Expected Output:**
```
✓ Application bundle generation complete.
➜  Local:   http://localhost:4200/
```

---

### Step 11: Test the Complete Feature

#### 11.1 Test Creating a Product

1. Open http://localhost:4200 in your browser
2. Click **"Add New Product"**
3. Fill in the form:
   - Product Name: "Organic Carrots"
   - Price: "$5.99"
   - Category: "Fresh Vegetables"
   - **Stock Quantity: "100"** ← New field
4. Click **"Create Product"**
5. ✅ You should see success message and redirect to products list

#### 11.2 Test Displaying the Product

1. On the products page, locate your new product
2. Look for the **"Stock"** section showing **"100 units"**
3. ✅ Value should display correctly

#### 11.3 Test Editing a Product

1. Click the **edit icon** (pencil) on any product
2. Change the **Stock Quantity** to a different value (e.g., "50")
3. Click **"Update Product"**
4. Go back to products list
5. ✅ The new quantity should be displayed

---

### Summary: Adding a New Field

| Layer | File | Changes |
|-------|------|---------|
| **Database** | Migrations/ | `AddColumn` (via migration) |
| **Entity** | VegProducts.cs | Add property `public int StockQuantity` |
| **DTO** | VegProductDto.cs | Add property `public int StockQuantity` |
| **Controller** | VegProductsController.cs | Map field in GetAll, GetById, Put methods |
| **Model** | vegproduct.ts | Add `stockQuantity?: number` to interface |
| **Create Form TS** | create-vegproduct.ts | Add to form group and saveChanges() |
| **Create Form HTML** | create-vegproduct.html | Add `<mat-form-field>` with input |
| **Edit Form TS** | edit-vegproduct.ts | Add to saveChanges() method |
| **Edit Form HTML** | edit-vegproduct.html | Add `<mat-form-field>` with input |
| **List Display** | index-products.html | Display with `{{ product.stockQuantity }}` |

✅ **You've successfully added a new field to VegProducts!**

---

---

## 🆕 Complete Guide: Creating New Tables with Foreign Keys

**Scenario:** You want to create a new `VegTypeProduct` table to categorize products as "Organic", "Conventional", "Genetically Modified", etc.

**What You'll Learn:** Complete process to create a new database table, set up relationships with existing tables, and integrate it fully into the Angular frontend.

### Architecture Diagram

```
VegTypeProduct (NEW TABLE)
├── IdType (Primary Key)
├── TypeName (e.g., "Organic")
├── Description
└── CreatedAt

↓ (One-to-Many Relationship)

VegProducts (EXISTING TABLE)
├── Id (PK)
├── Name
├── Price
├── StockQuantity
├── IdCategory (FK) → VegCategory
└── IdType (NEW FK) → VegTypeProduct  ← Links products to types
```

---

### Step 1: Create Backend Entity for VegTypeProduct

**Purpose:** Define the new table structure in C#.

#### 1.1 Create New Entity File

Path: `DotNetCoreWebApi/DotNetCoreWebApi/Application/Entities/VegTypeProduct.cs`

Create a new file with this content:

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotNetCoreWebApi.Application.Entities
{
    [Table("VegTypeProducts")]
    public class VegTypeProduct
    {
        [Key]
        [Column("IdType")]
        public int IdType { get; set; }

        [Column("TypeName")]
        [Required]
        [MaxLength(100)]
        public string TypeName { get; set; } = string.Empty;

        [Column("Description")]
        [MaxLength(500)]
        public string? Description { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation property: One type has many products
        public virtual ICollection<VegProducts> VegProducts { get; set; } = new List<VegProducts>();
    }
}
```

**Explanation:**
- `[Table("VegTypeProducts")]`: Database table name
- `[Key]`: Marks IdType as primary key
- `[MaxLength(100)]`: Validates string length
- `ICollection<VegProducts>`: One-to-many relationship

---

### Step 2: Update VegProducts Entity to Add Foreign Key

**Purpose:** Link VegProducts to the new VegTypeProduct table.

#### 2.1 Open VegProducts Entity

File: `DotNetCoreWebApi/DotNetCoreWebApi/Application/Entities/VegProducts.cs`

#### 2.2 Add Foreign Key and Navigation Property

```csharp
namespace DotNetCoreWebApi.Application.Entities;

public class VegProducts
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }

    // EXISTING foreign key
    public int? IdCategory { get; set; }
    public virtual VegCategory? VegCategory { get; set; }

    // ADD THESE NEW LINES:
    [ForeignKey("IdType")]  // ← Specifies the FK column
    public int? IdType { get; set; }  // ← Foreign Key to VegTypeProducts
    
    public virtual VegTypeProduct? VegType { get; set; }  // ← Navigation property
}
```

**Don't forget to add using statements:**
```csharp
using System.ComponentModel.DataAnnotations.Schema;
```

---

### Step 3: Update DbContext to Register New Entity

**Purpose:** Tell Entity Framework about the new table.

#### 3.1 Open ApplicationDBContext

File: `DotNetCoreWebApi/DotNetCoreWebApi/Application/DBContext/ApplicationDBContext.cs`

#### 3.2 Add DbSet for VegTypeProduct

```csharp
public class ApplicationDBContext : DbContext
{
    public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) 
        : base(options) { }

    // EXISTING DbSets
    public DbSet<VegProducts> VegProducts { get; set; }
    public DbSet<VegCategory> VegCategories { get; set; }

    // ADD THIS:
    public DbSet<VegTypeProduct> VegTypeProducts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Optional: Configure the relationship explicitly
        modelBuilder.Entity<VegProducts>()
            .HasOne(p => p.VegType)
            .WithMany(t => t.VegProducts)
            .HasForeignKey(p => p.IdType);
    }
}
```

---

### Step 4: Create Database Migration for New Table

**Purpose:** Create a migration to add the new VegTypeProduct table to your database.

#### 4.1 Create Migration

```powershell
cd "DotNetCoreWebApi/DotNetCoreWebApi"

# Create migration with descriptive name
dotnet ef migrations add CreateVegTypeProductTable

# Expected output:
# Successfully added migration 'CreateVegTypeProductTable'.
```

#### 4.2 Verify Migration File (Optional Check)

Open the generated migration file to verify it looks correct:
- Should create `VegTypeProducts` table
- Should add `IdType` column to `VegProducts` table
- Should create foreign key constraint

#### 4.3 Apply Migration to Database

```powershell
# Apply the migration
dotnet ef database update

# Expected output:
# Build started...
# Applying migration '20260217120500_CreateVegTypeProductTable'.
# Done.
```

✅ **New table is now in your database!**

---

### Step 5: Create DTOs (Data Transfer Objects)

**Purpose:** Define what data gets sent to/from the API.

#### 5.1 Create VegTypeProductDto

Path: `DotNetCoreWebApi/DotNetCoreWebApi/DTOs/VegTypeProductDto.cs`

Create new file:

```csharp
namespace DotNetCoreWebApi.DTOs
{
    public class VegTypeProductDto
    {
        public int IdType { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
```

#### 5.2 Update VegProductDto

File: `DotNetCoreWebApi/DotNetCoreWebApi/DTOs/VegProductDto.cs`

Add the type fields:

```csharp
namespace DotNetCoreWebApi.DTOs;

public class VegProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public int StockQuantity { get; set; }

    public int? IdCategory { get; set; }
    public VegCategoryBasicDto? VegCategory { get; set; }

    // ADD THESE NEW FIELDS:
    public int? IdType { get; set; }  // ← FK to type
    public VegTypeProductDto? VegType { get; set; }  // ← Type details
}
```

---

### Step 6: Create Backend Controller for VegTypeProduct

**Purpose:** Create API endpoints to manage types (GET, POST, PUT, DELETE).

#### 6.1 Create New Controller

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

        /// <summary>
        /// Get all product types
        /// GET /api/vegtypeproducts
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VegTypeProductDto>>> GetAllTypes()
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

        /// <summary>
        /// Get single type by ID
        /// GET /api/vegtypeproducts/1
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<VegTypeProductDto>> GetTypeById(int id)
        {
            var type = await context.VegTypeProducts.AsNoTracking()
                .FirstOrDefaultAsync(t => t.IdType == id);

            if (type == null)
                return NotFound($"Type with ID {id} not found");

            return Ok(new VegTypeProductDto
            {
                IdType = type.IdType,
                TypeName = type.TypeName,
                Description = type.Description
            });
        }

        /// <summary>
        /// Create new product type
        /// POST /api/vegtypeproducts
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<VegTypeProduct>> CreateType([FromBody] VegTypeProduct type)
        {
            if (string.IsNullOrWhiteSpace(type.TypeName))
                return BadRequest("Type name is required");

            context.VegTypeProducts.Add(type);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTypeById), new { id = type.IdType }, type);
        }

        /// <summary>
        /// Update existing product type
        /// PUT /api/vegtypeproducts/1
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateType(int id, [FromBody] VegTypeProduct type)
        {
            if (id != type.IdType)
                return BadRequest("ID mismatch");

            if (string.IsNullOrWhiteSpace(type.TypeName))
                return BadRequest("Type name is required");

            var existingType = await context.VegTypeProducts.FindAsync(id);
            if (existingType == null)
                return NotFound();

            existingType.TypeName = type.TypeName;
            existingType.Description = type.Description;

            context.Entry(existingType).State = EntityState.Modified;
            await context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Delete product type
        /// DELETE /api/vegtypeproducts/1
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteType(int id)
        {
            var type = await context.VegTypeProducts.FindAsync(id);
            if (type == null)
                return NotFound();

            // Check if any products use this type
            var productsWithType = await context.VegProducts
                .Where(p => p.IdType == id)
                .CountAsync();

            if (productsWithType > 0)
                return BadRequest($"Cannot delete type. {productsWithType} product(s) are using this type.");

            context.VegTypeProducts.Remove(type);
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}
```

---

### Step 7: Update VegProductsController to Include Type Data

**Purpose:** When fetching products, also include their type information.

#### 7.1 Update GetAllAsync Method

File: `DotNetCoreWebApi/DotNetCoreWebApi/Controllers/VegProductsController.cs`

```csharp
[HttpGet]
public async Task<ActionResult<IEnumerable<VegProductDto>>> GetAllAsync()
{
    var products = await context.VegProducts
        .Include(p => p.VegCategory)
        .Include(p => p.VegType)  // ← ADD THIS LINE
        .AsNoTracking()
        .ToListAsync();

    var productDtos = products.Select(p => new VegProductDto
    {
        Id = p.Id,
        Name = p.Name,
        Price = p.Price,
        Description = p.Description,
        StockQuantity = p.StockQuantity,
        IdCategory = p.IdCategory,
        VegCategory = p.VegCategory == null ? null : new VegCategoryBasicDto
        {
            IdCategory = p.VegCategory.IdCategory,
            CategoryName = p.VegCategory.CategoryName,
            Description = p.VegCategory.Description
        },
        
        // ADD THESE FIELDS:
        IdType = p.IdType,
        VegType = p.VegType == null ? null : new VegTypeProductDto
        {
            IdType = p.VegType.IdType,
            TypeName = p.VegType.TypeName,
            Description = p.VegType.Description
        }
    }).ToList();

    return Ok(productDtos);
}
```

#### 7.2 Update GetByIdAsync Method

Do the same in `GetByIdAsync()` method - add `.Include(p => p.VegType)` and map the type data.

#### 7.3 Update PutVegproduct (Edit) Method

```csharp
[HttpPut("{id}")]
public async Task<IActionResult> PutVegproduct(int id, [FromBody] VegProducts vegproduct)
{
    if (id != vegproduct.Id)
        return BadRequest("ID mismatch");

    var existingProduct = await context.VegProducts.FindAsync(id);
    if (existingProduct == null)
        return NotFound();

    existingProduct.Name = vegproduct.Name;
    existingProduct.Price = vegproduct.Price;
    existingProduct.Description = vegproduct.Description;
    existingProduct.StockQuantity = vegproduct.StockQuantity;
    existingProduct.IdCategory = vegproduct.IdCategory == 0 ? null : vegproduct.IdCategory;
    
    // ADD THIS LINE:
    existingProduct.IdType = vegproduct.IdType;  // ← Allow updating the type

    await context.SaveChangesAsync();
    return NoContent();
}
```

---

### Step 8: Restart Backend

```powershell
cd "c:\Arthur\Development\2026\DotNetAngularApp\DotNetCoreWebApi\DotNetCoreWebApi"

# Kill any running backend processes first
Get-Process | Where-Object { $_.ProcessName -match 'dotnet' } | Stop-Process -Force -ErrorAction SilentlyContinue
Start-Sleep -Seconds 3

# Clean and run
dotnet clean
dotnet run
```

---

### Step 9: Create Angular Model and Service

**Purpose:** Add TypeScript interfaces and HTTP service for the new entity.

#### 9.1 Update Angular Interfaces

File: `angular-app/src/app/vegproduct.ts`

```typescript
// ADD NEW INTERFACE:
export interface VegTypeProduct {
  idType: number;
  typeName: string;
  description?: string;
}

// UPDATE EXISTING INTERFACE:
export interface VegProduct {
  id: number;
  name: string;
  price: number;
  description?: string;
  stockQuantity?: number;
  idCategory?: number;
  vegCategory?: VegCategory;
  
  // ADD THESE NEW FIELDS:
  idType?: number;  // ← Foreign key
  vegType?: VegTypeProduct;  // ← Type details
}
```

####9.2 Create VegTypeProduct Service

File: `angular-app/src/app/vegtype.service.ts`

Create new service file:

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

  // Get all product types
  getAllTypes(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  // Get single type
  getTypeById(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  // Create new type
  createType(data: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, data);
  }

  // Update type
  updateType(id: number, data: any): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, data);
  }

  // Delete type
  deleteType(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
```

---

### Step 10: Update Create Product Form to Include Type

**Purpose:** Allow users to select a product type when creating products.

#### 10.1 Update Create Component TypeScript

File: `angular-app/src/app/create-vegproduct/create-vegproduct.ts`

```typescript
import { VegTypeService } from '../vegtype.service';  // ← ADD THIS

export class CreateVegproduct implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  vegProduct = inject(Vegproduct);
  vegCategoryService = inject(VegCategoryService);
  vegTypeService = inject(VegTypeService);  // ← ADD THIS
  router = inject(Router);
  snackBar = inject(MatSnackBar);

  categories: VegCategory[] = [];
  vegTypes: any[] = [];  // ← ADD THIS

  vegProductForm = this.formBuilder.group({
    name: ['', Validators.required],
    price: ['', Validators.required],
    description: [''],
    stockQuantity: [0, Validators.required],
    idCategory: [null as number | null],
    idType: [null as number | null]  // ← ADD THIS
  });

  ngOnInit() {
    this.loadCategories();
    this.loadVegTypes();  // ← ADD THIS
  }

  loadCategories() {
    this.vegCategoryService.getVegcategories().subscribe({
      next: (data) => {
        this.categories = data.map(cat => {
          if (!cat.idCategory && ((cat as any).Id || (cat as any).id)) {
            cat.idCategory = (cat as any).Id || (cat as any).id;
          }
          return cat;
        });
      },
      error: (error) => {
        console.error('Error loading categories:', error);
      }
    });
  }

  // ADD THIS NEW METHOD:
  loadVegTypes() {
    this.vegTypeService.getAllTypes().subscribe({
      next: (data) => {
        this.vegTypes = data;
        console.log('Product types loaded:', this.vegTypes);
      },
      error: (error) => {
        console.error('Error loading product types:', error);
      }
    });
  }

  saveChanges() {
    if (this.vegProductForm.valid) {
      const formValue = this.vegProductForm.value;
      const vegProductData: any = {
        name: formValue.name,
        price: parseFloat(formValue.price || '0'),
        description: formValue.description || '',
        stockQuantity: formValue.stockQuantity || 0,
        idCategory: formValue.idCategory || null,
        idType: formValue.idType || null  // ← ADD THIS
      };
      
      this.vegProduct.createVegproduct(vegProductData).subscribe({
        next: (response) => {
          this.snackBar.open(`✓ Product created!`, 'Close', {
            duration: 4000,
            horizontalPosition: 'end',
            verticalPosition: 'bottom',
            panelClass: ['success-snackbar']
          });
          
          setTimeout(() => {
            this.router.navigate(['/products']);
          }, 1000);
        },
        error: (error) => {
          this.snackBar.open(`✗ Error: ${error}`, 'Close', { duration: 5000 });
        }
      });
    }
  }
}
```

#### 10.2 Update CreateForm HTML

File: `angular-app/src/app/create-vegproduct/create-vegproduct.html`

Add this dropdown before the submit button:

```html
<div class="form-field">
  <mat-form-field appearance="outline" class="full-width">
    <mat-label>Product Type (Optional)</mat-label>
    <mat-select formControlName="idType">
      <mat-option></mat-option>
      <mat-option *ngFor="let type of vegTypes" [value]="type.idType">
        {{ type.typeName }}
      </mat-option>
    </mat-select>
    <mat-icon matSuffix>category</mat-icon>
    <mat-hint>Choose how the product is produced</mat-hint>
  </mat-form-field>
</div>
```

---

### Step 11: Update Edit Product Form

Do the same as Step 10 but for:
- File: `angular-app/src/app/edit-vegproduct/edit-vegproduct.ts`
- File: `angular-app/src/app/edit-vegproduct/edit-vegproduct.html`

Add the same service injection, loading method, form field, and data submission.

---

### Step 12: Update Product List Display

**Purpose:** Show the product type in the products grid.

File: `angular-app/src/app/index-products/index-products.html`

Add this section to display the type:

```html
<div class="type-section" *ngIf="product.vegType">
  <span class="type-label">Type</span>
  <span class="type-badge">{{ product.vegType.typeName }}</span>
</div>
```

Add styling to `index-products.css`:

```css
.type-section {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: 12px;
}

.type-label {
  font-size: 13px;
  font-weight: 600;
  color: #7f8c8d;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.type-badge {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 6px 12px;
  border-radius: 16px;
  font-size: 12px;
  font-weight: 600;
}
```

---

### Step 13: Restart Angular

```powershell
Get-Process | Where-Object { $_.ProcessName -match 'node' } | Stop-Process -Force -ErrorAction SilentlyContinue
Start-Sleep -Seconds 3

cd "c:\Arthur\Development\2026\DotNetAngularApp\angular-app"
npm start
```

---

### Step 14: Test the Complete Feature

#### 14.1 Test Creating a Product Type (via API)

```powershell
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}

$body = @{
    idType = 0
    typeName = "Organic"
    description = "100% Organic Products"
} | ConvertTo-Json

Invoke-RestMethod -Uri "https://localhost:7020/api/vegtypeproducts" `
  -Method POST -Body $body -ContentType "application/json"
```

#### 14.2 Test Creating a Product with Type

1. Go to http://localhost:4200/products
2. Click **"Add New Product"**
3. Fill the form:
   - Name: "Organic Apples"
   - Price: "$3.99"
   - Category: "Fruits"
   - **Product Type: "Organic"** ← New field
   - Stock Quantity: "75"
4. Click **"Create Product"**
5. ✅ Product should appear with type displayed

#### 14.3 Test Editing a Product Type

1. Click edit on a product
2. Change the **Product Type**
3. Click **"Update Product"**
4. ✅ Type should update

---

### Summary: Creating a New Table with Foreign Key

| Layer | File | Changes |
|-------|------|---------|
| **Entity** | VegTypeProduct.cs | Create new entity with IdType PK |
| **Entity** | VegProducts.cs | Add IdType FK + VegType navigation |
| **DbContext** | ApplicationDBContext.cs | Add DbSet<VegTypeProduct> |
| **Migration** | Migrations/ | Create migration for new table |
| **DTO** | VegTypeProductDto.cs | Create DTO for API responses |
| **DTO** | VegProductDto.cs | Add IdType + VegType fields |
| **Controller** | VegTypeProductsController.cs | Create CRUD endpoints |
| **Controller** | VegProductsController.cs | Update to include type data |
| **Model** | vegproduct.ts | Add VegTypeProduct interface |
| **Service** | vegtype.service.ts | Create service for API calls |
| **Form TS** | create-vegproduct.ts | Inject service, load types, add form field |
| **Form HTML** | create-vegproduct.html | Add type dropdown |
| **List HTML** | index-products.html | Display type in product cards |

✅ **You've successfully created a new table with foreign key relationship!**

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

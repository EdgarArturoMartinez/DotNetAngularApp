# Vegetarian Products Admin Dashboard

A professional Angular 17+ admin dashboard for managing vegetarian products with categories, built with Angular Material Design and .NET Core 8 backend.

## Quick Start

```bash
npm install
ng serve
```

Navigate to `http://localhost:4201/`

---

## 15-Step Implementation Guide: Adding Categories with One-to-Many Relationships

### Backend Implementation (Steps 1-6)

#### Step 1: Create VegCategory Model
**File:** `..\DotNetAngularApp\Models\VegCategory.cs`

```csharp
namespace DotNetAngularApp.Models
{
    public class VegCategory
    {
        public int IdCategory { get; set; }
        public string CategoryName { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation property for One-to-Many relationship
        public virtual ICollection<VegProduct> VegProducts { get; set; } = new List<VegProduct>();
    }
}
```

#### Step 2: Update VegProduct Model with Foreign Key
**File:** `..\DotNetAngularApp\Models\VegProduct.cs`

```csharp
namespace DotNetAngularApp.Models
{
    public class VegProduct
    {
        public int IdVegproduct { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public string? Description { get; set; }
        
        // Add Foreign Key
        public int? IdCategory { get; set; }
        
        // Navigation property for Many-to-One relationship
        public virtual VegCategory? VegCategory { get; set; }
    }
}
```

#### Step 3: Update DbContext Configuration
**File:** `..\DotNetAngularApp\Data\ApplicationDbContext.cs`

```csharp
public DbSet<VegCategory> VegCategories { get; set; }
public DbSet<VegProduct> VegProducts { get; set; }

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);
    
    // Configure One-to-Many relationship
    modelBuilder.Entity<VegProduct>()
        .HasOne(p => p.VegCategory)
        .WithMany(c => c.VegProducts)
        .HasForeignKey(p => p.IdCategory)
        .OnDelete(DeleteBehavior.SetNull);
}
```

#### Step 4: Create Entity Framework Migration
**Terminal:**

```bash
cd ..\DotNetAngularApp
dotnet ef migrations add AddVegCategoryAndForeignKey
dotnet ef database update
```

#### Step 5: Create VegCategoriesController
**File:** `..\DotNetAngularApp\Controllers\VegCategoriesController.cs`

```csharp
[ApiController]
[Route("api/[controller]")]
public class VegCategoriesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    
    public VegCategoriesController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VegCategory>>> GetCategories()
    {
        return await _context.VegCategories.AsNoTracking().ToListAsync();
    }
    
    [HttpPost]
    public async Task<ActionResult<VegCategory>> CreateCategory(VegCategory category)
    {
        _context.VegCategories.Add(category);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetCategories), new { id = category.IdCategory }, category);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(int id, VegCategory category)
    {
        if (id != category.IdCategory)
            return BadRequest();
        
        _context.Entry(category).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var category = await _context.VegCategories.FindAsync(id);
        if (category == null)
            return NotFound();
        
        _context.VegCategories.Remove(category);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
```

#### Step 6: Update VegProductsController
**File:** `..\DotNetAngularApp\Controllers\VegProductsController.cs` - Include `.Include(p => p.VegCategory)` in GetVegproducts:

```csharp
[HttpGet]
public async Task<ActionResult<IEnumerable<VegProduct>>> GetVegproducts()
{
    return await _context.VegProducts
        .Include(p => p.VegCategory)
        .AsNoTracking()
        .ToListAsync();
}

[HttpPost]
public async Task<ActionResult<VegProduct>> PostVegproduct(VegProduct vegproduct)
{
    _context.VegProducts.Add(vegproduct);
    await _context.SaveChangesAsync();
    return CreatedAtAction(nameof(GetVegproducts), new { id = vegproduct.IdVegproduct }, vegproduct);
}

[HttpPut("{id}")]
public async Task<IActionResult> PutVegproduct(int id, VegProduct vegproduct)
{
    if (id != vegproduct.IdVegproduct)
        return BadRequest();
    
    vegproduct.IdCategory = vegproduct.IdCategory == 0 ? null : vegproduct.IdCategory;
    _context.Entry(vegproduct).State = EntityState.Modified;
    await _context.SaveChangesAsync();
    return NoContent();
}
```

### Frontend Implementation (Steps 7-15)

#### Step 7: Create VegCategory Interface
**File:** `src/app/vegcategory.ts`

```typescript
export interface VegCategory {
  idCategory: number;
  categoryName: string;
  description?: string;
  createdAt?: string;
  vegProducts?: any[];
}
```

#### Step 8: Create VegCategoryService
**File:** `src/app/services/vegcategory.service.ts`

```typescript
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { VegCategory } from '../vegcategory';

@Injectable({
  providedIn: 'root'
})
export class VegCategoryService {
  private apiUrl = 'http://localhost:5122/api/vegcategories';
  
  constructor(private http: HttpClient) { }
  
  getVegcategories(): Observable<VegCategory[]> {
    return this.http.get<VegCategory[]>(this.apiUrl);
  }
  
  createVegcategory(data: VegCategory): Observable<VegCategory> {
    return this.http.post<VegCategory>(this.apiUrl, data);
  }
  
  updateVegcategory(id: number, data: VegCategory): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, data);
  }
  
  deleteVegcategory(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
```

#### Step 9: Update VegProductService
**File:** `src/app/services/vegproduct.service.ts` - Add Import:

```typescript
import { VegCategory } from '../vegcategory';

// Update interface to include category
export interface VegProduct {
  idVegproduct: number;
  name: string;
  price: number;
  description?: string;
  idCategory?: number;
  vegCategory?: VegCategory;  // Add this line
}
```

#### Step 10: Add Category Dropdown to Create Product Form
**File:** `src/app/create-vegproduct/create-vegproduct.ts`

```typescript
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { VegProductService } from '../services/vegproduct.service';
import { VegCategoryService } from '../services/vegcategory.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { VegCategory } from '../vegcategory';

@Component({
  selector: 'app-create-vegproduct',
  standalone: true,
  imports: [ /* existing imports */ ],
  templateUrl: './create-vegproduct.html',
  styleUrl: './create-vegproduct.css'
})
export class CreateVegproductComponent implements OnInit {
  form!: FormGroup;
  categories: VegCategory[] = [];
  
  constructor(
    private fb: FormBuilder,
    private vegProduct: VegProductService,
    private vegCategory: VegCategoryService,
    private snackBar: MatSnackBar,
    private router: Router
  ) {}
  
  ngOnInit() {
    this.form = this.fb.group({
      name: ['', Validators.required],
      price: ['', [Validators.required, Validators.pattern(/^\d+(\.\d{1,2})?$/)]],
      description: [''],
      idCategory: ['']
    });
    
    this.loadCategories();
  }
  
  loadCategories() {
    this.vegCategory.getVegcategories().subscribe(
      data => this.categories = data,
      error => console.error('Error loading categories:', error)
    );
  }
  
  submit() {
    if (this.form.valid) {
      this.vegProduct.createVegproduct(this.form.value).subscribe(
        () => {
          this.snackBar.open('Product created successfully!', 'Close', { duration: 3000 });
          this.router.navigate(['/products']);
        },
        error => this.snackBar.open('Error creating product', 'Close', { duration: 3000 })
      );
    }
  }
}
```

**File:** `src/app/create-vegproduct/create-vegproduct.html` - Add to form:

```html
<mat-form-field appearance="outline" class="full-width">
  <mat-label>Category (Optional)</mat-label>
  <mat-select formControlName="idCategory">
    <mat-option></mat-option>
    <mat-option *ngFor="let cat of categories" [value]="cat.idCategory">
      {{ cat.categoryName }}
    </mat-option>
  </mat-select>
</mat-form-field>
```

#### Step 11: Update Edit Product Form
**File:** `src/app/edit-vegproduct/edit-vegproduct.ts` - Add same category dropdown loading logic as Step 10

#### Step 12: Update Product List to Show Category Names
**File:** `src/app/index-products/index-products.html`

```html
<h3 class="product-category">
  {{ product.vegCategory?.categoryName || 'Uncategorized' }}
</h3>
```

**File:** `src/app/index-products/index-products.css` - Add:

```css
.product-category {
  font-size: 0.85rem;
  color: #4caf50;
  font-weight: 500;
  margin: 8px 0;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}
```

#### Step 13: Create Index Categories Component
**Terminal:**

```bash
ng generate component index-categories
```

**File:** `src/app/index-categories/index-categories.ts`

```typescript
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { VegCategoryService } from '../services/vegcategory.service';
import { VegCategory } from '../vegcategory';

@Component({
  selector: 'app-index-categories',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './index-categories.html',
  styleUrl: './index-categories.css'
})
export class IndexCategoriesComponent implements OnInit {
  categories: VegCategory[] = [];
  isLoading = false;
  
  constructor(
    private vegCategory: VegCategoryService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog
  ) {}
  
  ngOnInit() {
    this.loadCategories();
  }
  
  loadCategories() {
    this.isLoading = true;
    this.vegCategory.getVegcategories().subscribe(
      data => {
        this.categories = data;
        this.isLoading = false;
      },
      error => {
        this.snackBar.open('Error loading categories', 'Close', { duration: 3000 });
        this.isLoading = false;
      }
    );
  }
  
  deleteCategory(id: number) {
    if (confirm('Are you sure?')) {
      this.vegCategory.deleteVegcategory(id).subscribe(
        () => {
          this.snackBar.open('Category deleted', 'Close', { duration: 3000 });
          this.loadCategories();
        }
      );
    }
  }
}
```

#### Step 14: Update App Routes
**File:** `src/app/app.routes.ts`

```typescript
import { IndexCategoriesComponent } from './index-categories/index-categories';

export const routes: Routes = [
  { path: '', component: LandingComponent },
  { path: 'products', component: IndexProductsComponent },
  { path: 'products/create', component: CreateVegproductComponent },
  { path: 'products/edit/:id', component: EditVegproductComponent },
  { path: 'categories', component: IndexCategoriesComponent }
];
```

#### Step 15: Update Navigation Menu
**File:** `src/app/menu/menu.html` - Add to navigation:

```html
<a routerLink="/categories" routerLinkActive="active">
  <mat-icon>category</mat-icon>
  Categories
</a>
```

---

## Project Structure

```
src/
├── app/
│   ├── services/
│   │   ├── vegproduct.service.ts
│   │   └── vegcategory.service.ts
│   ├── create-vegproduct/
│   ├── edit-vegproduct/
│   ├── index-products/
│   ├── index-categories/
│   ├── menu/
│   ├── landing/
│   ├── vegcategory.ts
│   ├── weatherforecast.ts
│   └── app.routes.ts
```

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/vegproducts` | Get all products |
| POST | `/api/vegproducts` | Create product |
| PUT | `/api/vegproducts/{id}` | Update product |
| DELETE | `/api/vegproducts/{id}` | Delete product |
| GET | `/api/vegcategories` | Get all categories |
| POST | `/api/vegcategories` | Create category |
| PUT | `/api/vegcategories/{id}` | Update category |
| DELETE | `/api/vegcategories/{id}` | Delete category |

## Key Features

- ✅ Professional Material Design UI
- ✅ Complete Product CRUD
- ✅ Category Management with One-to-Many Relationships
- ✅ Responsive Mobile Design
- ✅ Change Detection Optimization
- ✅ Loading States & Error Handling
- ✅ Form Validation
- ✅ Currency Formatting

## Technologies

- **Frontend:** Angular 17+, Angular Material 18+, RxJS
- **Backend:** .NET Core 8, Entity Framework Core
- **Database:** SQL Server
- **Architecture:** Standalone Components, Signals, Reactive Forms

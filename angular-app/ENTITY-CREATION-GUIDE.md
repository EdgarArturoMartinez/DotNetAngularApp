# Entity Management System - Complete Guide

## Architecture Overview

This project uses a **clean architecture** pattern with reusable services and components. The system is built to minimize code duplication and maximize scalability.

### Folder Structure

```
src/app/
├── shared/              # Shared utilities, services, and base classes
│   ├── services/
│   │   └── base-crud.service.ts      # Base service for all CRUD operations
│   ├── components/
│   │   ├── base-index.component.ts   # Base class for list/index components
│   │   └── base-create-edit.component.ts  # Base class for create/edit components
│   ├── models/
│   │   └── entities.ts               # All entity interfaces and DTOs
│   └── interfaces/
│       └── entity.interface.ts       # Base interfaces
├── core/                # Core services (future use)
├── features/            # Feature modules
│   ├── products/
│   │   ├── product.service.ts
│   │   ├── product-form.component.ts
│   │   ├── product-form.component.html
│   │   ├── product-index.component.ts
│   │   └── product-index.component.html
│   └── categories/
│       ├── category.service.ts
│       ├── category-form.component.ts
│       ├── category-form.component.html
│       ├── category-index.component.ts
│       └── category-index.component.html
└── app.routes.ts        # Main routing configuration
```

---

## How to Create a New Entity (Step-by-Step)

### Step 1: Add Entity Model to `shared/models/entities.ts`

```typescript
// Add your entity interface
export interface MyEntity extends IEntity {
  id?: number;
  name: string;
  description?: string;
  createdAt?: string;
  updatedAt?: string;
  // Add other fields specific to your entity
}

// Add a DTO for create/update operations
export interface MyEntityCreateUpdateDto {
  name: string;
  description?: string;
  // Add fields that can be modified
}
```

**Why DTOs?** They prevent exposing sensitive fields and allow you to control which fields can be updated.

---

### Step 2: Create the Service in `features/my-entity/`

```typescript
// File: src/app/features/my-entity/my-entity.service.ts

import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { BaseCrudService } from '../../shared/services/base-crud.service';
import { MyEntity, MyEntityCreateUpdateDto } from '../../shared/models/entities';

@Injectable({
  providedIn: 'root'
})
export class MyEntityService extends BaseCrudService<MyEntity> {
  constructor() {
    super(inject(HttpClient), `${environment.apiURL}/api/my-entities`);
  }

  // Override for type-safe creation
  override create(data: MyEntityCreateUpdateDto) {
    return super.create(data);
  }

  // Override for type-safe update
  override update(id: number | string, data: MyEntityCreateUpdateDto) {
    return super.update(id, data);
  }

  // Add custom methods specific to your entity if needed
  // Example:
  // public searchByName(name: string): Observable<MyEntity[]> {
  //   return this.http.get<MyEntity[]>(`${this.apiUrl}/search?name=${name}`);
  // }
}
```

**Key Points:**
- Always extend `BaseCrudService<YourEntity>`
- Pass the API URL in the constructor
- Override `create()` and `update()` for type safety

---

### Step 3: Create the Index Component

```typescript
// File: src/app/features/my-entity/my-entity-index.component.ts

import { Component, inject, ChangeDetectorRef } from '@angular/core';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { RouterLink } from '@angular/router';

import { BaseIndexComponent } from '../../shared/components/base-index.component';
import { MyEntityService } from './my-entity.service';
import { MyEntity } from '../../shared/models/entities';

@Component({
  selector: 'app-my-entity-index',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    RouterLink
  ],
  templateUrl: './my-entity-index.component.html',
  styleUrl: './my-entity-index.component.css'
})
export class MyEntityIndexComponent extends BaseIndexComponent<MyEntity> {
  private cdr = inject(ChangeDetectorRef);

  constructor(
    protected override service: MyEntityService,
    protected override router: Router,
    protected override snackBar: MatSnackBar
  ) {
    super(service, router, snackBar);
    this.entityName = 'My Entity';
    this.editRoute = '/my-entity/edit';
    this.indexRoute = '/my-entity';
  }

  // Override loadItems() if you need custom loading logic
  // Otherwise, the base class handles everything
}
```

**HTML Template** (`my-entity-index.component.html`):
```html
<div class="entities-wrapper">
  <div class="header-section">
    <div class="header-content">
      <h1 class="page-title">My Entity List</h1>
      <p class="page-subtitle">Manage your entities</p>
    </div>
    <button mat-flat-button routerLink="/my-entity/create" class="create-button">
      <mat-icon>add_circle</mat-icon>
      Add New Entity
    </button>
  </div>

  <div class="entities-container">
    <!-- LOADING STATE -->
    <div *ngIf="isLoading" class="state-container loading-state">
      <mat-spinner diameter="50"></mat-spinner>
      <p>Loading entities...</p>
    </div>

    <!-- ERROR STATE -->
    <div *ngIf="!isLoading && error" class="state-container error-state">
      <h3>Unable to Load Entities</h3>
      <p>{{ error }}</p>
      <button mat-stroked-button (click)="loadItems()">Try Again</button>
    </div>

    <!-- EMPTY STATE -->
    <div *ngIf="!isLoading && !error && items.length === 0" class="state-container empty-state">
      <h3>No Entities Yet</h3>
      <button mat-flat-button routerLink="/my-entity/create">Create First Entity</button>
    </div>

    <!-- ITEMS GRID -->
    <div *ngIf="!isLoading && !error && items.length > 0" class="items-grid">
      <div class="item-card" *ngFor="let item of items">
        <h3>{{ item.name }}</h3>
        <p>{{ item.description }}</p>
        <div class="actions">
          <button mat-icon-button (click)="editItem(item)">
            <mat-icon>edit</mat-icon>
          </button>
          <button mat-icon-button color="warn" (click)="deleteItem(item)">
            <mat-icon>delete</mat-icon>
          </button>
        </div>
      </div>
    </div>
  </div>
</div>
```

---

### Step 4: Create the Form Component (Create/Edit)

```typescript
// File: src/app/features/my-entity/my-entity-form.component.ts

import { Component, inject, ChangeDetectorRef } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/form-field';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { BaseCreateEditComponent } from '../../shared/components/base-create-edit.component';
import { MyEntityService } from './my-entity.service';
import { MyEntity, MyEntityCreateUpdateDto } from '../../shared/models/entities';

@Component({
  selector: 'app-my-entity-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    RouterLink
  ],
  templateUrl: './my-entity-form.component.html',
  styleUrl: './my-entity-form.component.css'
})
export class MyEntityFormComponent extends BaseCreateEditComponent<MyEntity> {
  private readonly formBuilder = inject(FormBuilder);
  private cdr = inject(ChangeDetectorRef);

  override form = this.formBuilder.group({
    name: ['', Validators.required],
    description: ['']
  });

  constructor(
    protected override service: MyEntityService,
    protected override router: Router,
    protected override activatedRoute: ActivatedRoute,
    protected override snackBar: MatSnackBar
  ) {
    super(service, router, activatedRoute, snackBar);
    this.entityName = 'My Entity';
    this.indexRoute = '/my-entity';
  }

  override protected populateForm(entity: MyEntity): void {
    this.form.patchValue({
      name: entity.name,
      description: entity.description || ''
    });
    this.cdr.detectChanges();
  }

  get formTitle(): string {
    return this.isEditMode ? 'Edit My Entity' : 'Create New My Entity';
  }

  get submitButtonText(): string {
    return this.isEditMode ? 'Update' : 'Create';
  }
}
```

**HTML Template** (`my-entity-form.component.html`):
```html
<div class="form-wrapper">
  <h2>{{ formTitle }}</h2>
  
  <div *ngIf="isLoading">Loading...</div>

  <form [formGroup]="form" (ngSubmit)="saveEntity()" *ngIf="!isLoading">
    <mat-form-field>
      <mat-label>Name</mat-label>
      <input matInput formControlName="name" required />
    </mat-form-field>

    <mat-form-field>
      <mat-label>Description</mat-label>
      <input matInput formControlName="description" />
    </mat-form-field>

    <button mat-raised-button type="submit" [disabled]="!form.valid">
      {{ submitButtonText }}
    </button>
    <a mat-button routerLink="/my-entity">Cancel</a>
  </form>
</div>
```

---

### Step 5: Register Routes in `app.routes.ts`

```typescript
export const routes: Routes = [
  // ... existing routes ...
  
  // My Entity routes
  { path: 'my-entity', component: MyEntityIndexComponent },
  { path: 'my-entity/create', component: MyEntityFormComponent },
  { path: 'my-entity/edit/:id', component: MyEntityFormComponent },
];
```

---

## Key Benefits of This Architecture

| Benefit | Explanation |
|---------|-------------|
| **DRY** | Base classes eliminate code repetition |
| **Type-Safe** | DTOs and interfaces provide compile-time safety |
| **Consistent** | All entities follow the same pattern |
| **Testable** | Services and components are independently testable |
| **Scalable** | Adding new entities requires minimal code |
| **Maintainable** | Bug fixes in base classes benefit all entities |

---

## Common Patterns & Best Practices

### Pattern 1: Adding Custom Methods to Services

```typescript
export class MyEntityService extends BaseCrudService<MyEntity> {
  // Custom method for searching
  public search(query: string): Observable<MyEntity[]> {
    return this.http.get<MyEntity[]>(
      `${this.apiUrl}/search?query=${query}`
    );
  }

  // Custom method for filtering
  public getByCategory(categoryId: number): Observable<MyEntity[]> {
    return this.http.get<MyEntity[]>(
      `${this.apiUrl}/category/${categoryId}`
    );
  }
}
```

### Pattern 2: Custom Form Validation

```typescript
export class MyEntityFormComponent extends BaseCreateEditComponent<MyEntity> {
  override form = this.formBuilder.group({
    name: ['', [Validators.required, Validators.minLength(3)]],
    email: ['', [Validators.required, Validators.email]],
    price: ['', [Validators.required, Validators.min(0)]]
  });
}
```

### Pattern 3: Handling Related Entities

```typescript
export class MyEntityFormComponent extends BaseCreateEditComponent<MyEntity> {
  categories: Category[] = [];
  private categoryService = inject(CategoryService);

  override ngOnInit(): void {
    this.loadCategories();
    super.ngOnInit();
  }

  private loadCategories(): void {
    this.categoryService.getAll().subscribe({
      next: (categories) => {
        this.categories = categories;
      }
    });
  }
}
```

---

## Testing Structure (Future)

```
tests/
├── unit/
│   ├── services/
│   │   ├── my-entity.service.spec.ts
│   │   └── base-crud.service.spec.ts
│   └── components/
│       ├── my-entity-form.component.spec.ts
│       └── my-entity-index.component.spec.ts
└── e2e/
    └── my-entity.e2e.spec.ts
```

---

## Checklist for Adding a New Entity

- [ ] Add entity interface to `shared/models/entities.ts`
- [ ] Add DTO interface to `shared/models/entities.ts`
- [ ] Create service in `features/my-entity/my-entity.service.ts`
- [ ] Create index component in `features/my-entity/my-entity-index.component.ts`
- [ ] Create index HTML template
- [ ] Create form component in `features/my-entity/my-entity-form.component.ts`
- [ ] Create form HTML template
- [ ] Register routes in `app.routes.ts`
- [ ] Update navigation menu if needed
- [ ] Test all CRUD operations
- [ ] Create unit tests (optional but recommended)
- [ ] Create E2E tests (optional but recommended)

---

## Troubleshooting

### Issue: "No routes matched the supplied values"
**Solution:** Ensure ID is properly normalized before navigation:
```typescript
const id = item.id || item.idEntity;
if (!id) {
  console.error('Missing ID!');
  return;
}
this.router.navigate(['/my-entity/edit', id]);
```

### Issue: Form not populated in edit mode
**Solution:** Ensure you override `populateForm()`:
```typescript
override protected populateForm(entity: MyEntity): void {
  this.form.patchValue({
    name: entity.name,
    // ... other fields
  });
  this.cdr.detectChanges();
}
```

### Issue: Categories/relations not loading
**Solution:** Load related data in component's `ngOnInit()`:
```typescript
override ngOnInit(): void {
  this.loadRelations(); // Load categories, etc.
  super.ngOnInit();     // This handles entity loading
}
```

---

## Next Steps

1. **Create a new entity** using this guide (e.g., Suppliers, Orders, Reviews)
2. **Run tests** to ensure everything works
3. **Document any custom methods** you add to the service
4. **Update the navigation menu** to include the new entity
5. **Create E2E tests** for critical workflows

---

**Happy coding! 🚀**

For questions or issues, refer to existing implementations in:
- `features/products/`
- `features/categories/`

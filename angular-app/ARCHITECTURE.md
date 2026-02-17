# Veggie World E-Commerce - Architecture Documentation

## 🗂️ Project Structure

This project follows **Clean Architecture** principles with clear separation of concerns.

### Directory Layout

```
src/app/
├── shared/                          # Shared across all features
│   ├── services/
│   │   ├── base-crud.service.ts     # ⭐ Generic CRUD service for all entities
│   │   └── base-crud.service.spec.ts
│   ├── components/
│   │   ├── base-index.component.ts  # ⭐ Reusable list/index component
│   │   └── base-create-edit.component.ts  # ⭐ Reusable form component
│   ├── models/
│   │   └── entities.ts              # ⭐ All entity definitions in one place
│   └── interfaces/
│       └── entity.interface.ts      # ⭐ Base interfaces for all entities
│
├── core/                             # Core application services (future expansion)
│   └── (place global services here)
│
├── features/                         # Feature modules
│   ├── products/
│   │   ├── product.service.ts       # Extends BaseCrudService
│   │   ├── product.service.spec.ts
│   │   ├── product-form.component.ts       # Extends BaseCreateEditComponent
│   │   ├── product-form.component.html
│   │   ├── product-form.component.css
│   │   ├── product-index.component.ts      # Extends BaseIndexComponent
│   │   ├── product-index.component.html
│   │   └── product-index.component.css
│   │
│   └── categories/
│       ├── category.service.ts
│       ├── category.service.spec.ts
│       ├── category-form.component.ts
│       ├── category-form.component.html
│       ├── category-form.component.css
│       ├── category-index.component.ts
│       ├── category-index.component.html
│       └── category-index.component.css
│
├── legacy/                          # Old components (being phased out)
│   ├── landing/
│   ├── menu/
│   ├── index-products/
│   ├── create-vegproduct/
│   ├── edit-vegproduct/
│   ├── index-vegcategories/
│   ├── create-vegcategory/
│   └── edit-vegcategory/
│
├── app.routes.ts                    # Route definitions
├── app.config.ts                    # Application configuration
└── app.ts                           # Root component

tests/
├── unit/
│   ├── services/
│   │   ├── product.service.spec.ts  # Example service test
│   │   └── base-crud.service.spec.ts
│   └── components/
│       ├── product-form.component.spec.ts  # Example component test
│       └── product-index.component.spec.ts
└── e2e/
    └── (end-to-end tests)
```

---

## 🎯 Core Concepts

### 1. Base CRUD Service (`BaseCrudService<T>`)

**Location:** `src/app/shared/services/base-crud.service.ts`

A generic service that provides standard CRUD operations for any entity:

```typescript
// Generic methods available to all child services:
- getAll(): Observable<T[]>
- getById(id): Observable<T>
- create(data): Observable<T>
- update(id, data): Observable<void>
- delete(id): Observable<void>
```

**Benefits:**
- ✅ No code duplication across services
- ✅ Consistent error handling
- ✅ Consistent logging
- ✅ Type-safe operations

**How to Use:**
```typescript
@Injectable({ providedIn: 'root' })
export class MyEntityService extends BaseCrudService<MyEntity> {
  constructor() {
    super(inject(HttpClient), 'https://localhost:7020/api/my-entities');
  }
}
```

---

### 2. Base Index Component (`BaseIndexComponent<T>`)

**Location:** `src/app/shared/components/base-index.component.ts`

A reusable component for displaying lists of entities:

```typescript
// Provided methods and properties:
- items: T[]                    // All loaded items
- isLoading: boolean           // Loading state
- error: string               // Error message
- loadItems(): void           // Load all items from service
- editItem(item): void        // Navigate to edit page
- deleteItem(item): void      // Delete with confirmation
- showSuccess/Error/Warning() // Show snackbar notifications
```

**Benefits:**
- ✅ Consistent UI/UX across all list pages
- ✅ Built-in error handling
- ✅ Built-in loading states
- ✅ ID normalization handles multiple API formats

---

### 3. Base Create/Edit Component (`BaseCreateEditComponent<T>`)

**Location:** `src/app/shared/components/base-create-edit.component.ts`

A reusable component for creating and editing entities:

```typescript
// Provided methods and properties:
- form: FormGroup            // Abstract - must override
- isEditMode: boolean        // True if editing, false if creating
- isLoading: boolean        // Loading state
- saveEntity(): void         // Save (create or update)
- populateForm(entity): void // Abstract - must override
```

**Benefits:**
- ✅ Single component for both create and edit pages
- ✅ Automatic route parameter detection
- ✅ Proper cleanup with RxJS subscriptions
- ✅ Error handling and user feedback

---

### 4. Entity Models (`entities.ts`)

**Location:** `src/app/shared/models/entities.ts`

All entity interfaces and DTOs are defined in one place:

```typescript
// Entity interfaces
export interface VegProduct extends IEntity { ... }
export interface VegCategory extends IEntity { ... }

// DTOs for create/update
export interface VegProductCreateUpdateDto { ... }
export interface VegCategoryCreateUpdateDto { ... }
```

**Benefits:**
- ✅ Single source of truth for entity definitions
- ✅ Easy to maintain and update
- ✅ DTOs prevent exposing sensitive fields
- ✅ Type-safe form handling

---

## 📊 Data Flow

### Create/Edit Flow

```
Component (extends BaseCreateEditComponent)
    ↓
Form Submission → saveEntity()
    ↓
Service (extends BaseCrudService)
    ↓
HTTP Request → Backend API
    ↓
Success/Error Handler
    ↓
Show Snackbar Notification
    ↓
Navigate to Index Page
```

### List/Index Flow

```
Component (extends BaseIndexComponent)
    ↓
ngOnInit() → loadItems()
    ↓
Service (extends BaseCrudService)
    ↓
HTTP GET Request
    ↓
Map/Transform Data (normalize IDs)
    ↓
Bind to Template *ngFor
    ↓
User Clicks Edit/Delete
    ↓
editItem() or deleteItem()
```

---

## 🔑 Key Design Patterns

### 1. Template Method Pattern

Base classes define the structure, child classes fill in the details:

```typescript
// Base class
export abstract class BaseCreateEditComponent {
  abstract form: FormGroup;
  protected abstract populateForm(entity: T): void;
}

// Child class
export class ProductCreateEditComponent extends BaseCreateEditComponent {
  override form = this.formBuilder.group({ ... });
  
  override protected populateForm(entity: VegProduct): void {
    this.form.patchValue({...});
  }
}
```

### 2. Dependency Injection

All services and dependencies are injected:

```typescript
export class MyComponent {
  private myService = inject(MyService);
  private router = inject(Router);
}
```

### 3. Observable-Based Reactive Programming

All async operations use RxJS Observables:

```typescript
this.service.getAll().subscribe({
  next: (data) => { /* handle success */ },
  error: (error) => { /* handle error */ }
});
```

### 4. Smart ID Normalization

Handles multiple API response formats:

```typescript
// Normalizes these different ID properties:
// - item.id
// - item.Id  
// - item.idEntity
// - item.idVegproduct

const id = item.id || item.idEntity || (item as any).Id;
```

---

## 🚀 Adding a New Entity (Quick Start)

1. **Define the model** in `shared/models/entities.ts`
2. **Create service** in `features/my-entity/my-entity.service.ts`
3. **Create form component** in `features/my-entity/my-entity-form.component.ts`
4. **Create form template** in `features/my-entity/my-entity-form.component.html`
5. **Create index component** in `features/my-entity/my-entity-index.component.ts`
6. **Create index template** in `features/my-entity/my-entity-index.component.html`
7. **Register routes** in `app.routes.ts`

**⏱️ Time estimate:** 15-20 minutes per entity

📖 **Detailed guide:** See `ENTITY-CREATION-GUIDE.md`

---

## 🧪 Testing

### Unit Tests

Located in `tests/unit/`:

```bash
# Run a specific test
npm test -- --include="**/product.service.spec.ts"

# Run all tests
npm test

# Run with coverage
npm test -- --coverage
```

### Example Test Patterns

**Service test:**
```typescript
it('should fetch all products', () => {
  const mockProducts = [...];
  service.getAll().subscribe(products => {
    expect(products).toEqual(mockProducts);
  });
  const req = httpMock.expectOne(apiUrl);
  req.flush(mockProducts);
});
```

**Component test:**
```typescript
it('should have valid form when required fields are filled', () => {
  component.form.patchValue({
    name: 'Test',
    price: 10.5
  });
  expect(component.form.valid).toBeTruthy();
});
```

---

## 🔄 Migration From Legacy Code

### Current Status
- ✅ New architecture implemented
- ✅ New services created (BaseCrudService pattern)
- ✅ New components created (BaseIndexComponent, BaseCreateEditComponent)
- ⏳ Routes still pointing to legacy components
- ⏳ Legacy components being phased out

### Next Steps
1. Update `app.routes.ts` to use new components
2. Verify all functionality works
3. Delete legacy component folders
4. Update navigation menu

---

## 📋 Best Practices

### ✅ DO

- Extend `BaseCrudService` for new entity services
- Extend base components for CRUD operations
- Use DTOs for form submission
- Normalize IDs before navigation
- Use RxJS operators (map, filter, etc.) for transformations
- Unsubscribe in OnDestroy for components

### ❌ DON'T

- Duplicate CRUD logic in multiple services
- Hardcode API URLs in components
- Skip error handling
- Skip form validation
- Create unnecessary subscriptions without cleanup

---

## 🔗 Related Documentation

- **Entity Creation Guide:** `ENTITY-CREATION-GUIDE.md`
- **API Integration Guide:** (TBD)
- **Testing Guide:** (TBD)
- **Deployment Guide:** (TBD)

---

## 📞 Support

For questions or issues:
1. Check ENTITY-CREATION-GUIDE.md
2. Review existing implementations (products, categories)
3. Check base class documentation in source files
4. Run tests to validate changes

---

## 🎓 Learning Path

1. **Understand:** Read this document
2. **Explore:** Look at `features/products/` implementation
3. **Create:** Add a new entity following the guide
4. **Test:** Write tests for your entity
5. **Deploy:** Push to production

---

**Last Updated:** February 2026
**Angular Version:** 21.1.0
**Status:** In Production ✅

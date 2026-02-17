# 🚀 Quick Reference - Veggie World Architecture

## 📝 Add a New Entity in 5 Minutes

### 1. Model Definition
**File:** `src/app/shared/models/entities.ts`

```typescript
export interface MyEntity extends IEntity {
  id?: number;
  name: string;
  description?: string;
}

export interface MyEntityCreateUpdateDto {
  name: string;
  description?: string;
}
```

### 2. Service
**File:** `src/app/features/my-entity/my-entity.service.ts`

```typescript
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { BaseCrudService } from '../../shared/services/base-crud.service';
import { MyEntity, MyEntityCreateUpdateDto } from '../../shared/models/entities';

@Injectable({ providedIn: 'root' })
export class MyEntityService extends BaseCrudService<MyEntity> {
  constructor() {
    super(inject(HttpClient), `${environment.apiURL}/api/my-entities`);
  }

  override create(data: MyEntityCreateUpdateDto) {
    return super.create(data);
  }

  override update(id: number | string, data: MyEntityCreateUpdateDto) {
    return super.update(id, data);
  }
}
```

### 3. Form Component
**File:** `src/app/features/my-entity/my-entity-form.component.ts`

```typescript
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule, MatFormFieldModule, MatInputModule } from '@angular/material';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CommonModule } from '@angular/common';

import { BaseCreateEditComponent } from '../../shared/components/base-create-edit.component';
import { MyEntityService } from './my-entity.service';
import { MyEntity, MyEntityCreateUpdateDto } from '../../shared/models/entities';

@Component({
  selector: 'app-my-entity-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatButtonModule, MatFormFieldModule, MatInputModule, RouterLink],
  template: `
    <form [formGroup]="form" (ngSubmit)="saveEntity()">
      <input matInput formControlName="name" placeholder="Name" required />
      <input matInput formControlName="description" placeholder="Description" />
      <button type="submit" [disabled]="!form.valid">{{ isEditMode ? 'Update' : 'Create' }}</button>
    </form>
  `
})
export class MyEntityFormComponent extends BaseCreateEditComponent<MyEntity> {
  override form = inject(FormBuilder).group({
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
    this.form.patchValue(entity);
  }
}
```

### 4. Index Component
**File:** `src/app/features/my-entity/my-entity-index.component.ts`

```typescript
import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CommonModule } from '@angular/common';
import { MatButtonModule, MatIconModule, MatProgressSpinnerModule } from '@angular/material';
import { RouterLink } from '@angular/router';

import { BaseIndexComponent } from '../../shared/components/base-index.component';
import { MyEntityService } from './my-entity.service';
import { MyEntity } from '../../shared/models/entities';

@Component({
  selector: 'app-my-entity-index',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatIconModule, MatProgressSpinnerModule, RouterLink],
  template: `
    <div *ngIf="isLoading">Loading...</div>
    <div *ngIf="error">{{ error }}</div>
    <div *ngIf="!isLoading && !error">
      <button routerLink="/my-entity/create">Add New</button>
      <div *ngFor="let item of items">
        <h3>{{ item.name }}</h3>
        <button (click)="editItem(item)">Edit</button>
        <button (click)="deleteItem(item)">Delete</button>
      </div>
    </div>
  `
})
export class MyEntityIndexComponent extends BaseIndexComponent<MyEntity> {
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
}
```

### 5. Routes
**File:** `src/app/app.routes.ts`

```typescript
export const routes: Routes = [
  // ... existing routes ...
  { path: 'my-entity', component: MyEntityIndexComponent },
  { path: 'my-entity/create', component: MyEntityFormComponent },
  { path: 'my-entity/edit/:id', component: MyEntityFormComponent },
];
```

---

## 🔧 Common Customizations

### Add Search Method to Service

```typescript
export class MyEntityService extends BaseCrudService<MyEntity> {
  public search(query: string): Observable<MyEntity[]> {
    return this.http.get<MyEntity[]>(`${this.apiUrl}/search?q=${query}`);
  }
}
```

### Add Form Validation

```typescript
override form = this.formBuilder.group({
  name: ['', [Validators.required, Validators.minLength(3)]],
  email: ['', [Validators.required, Validators.email]],
  price: ['', [Validators.required, Validators.min(0)]]
});
```

### Load Related Data

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
      next: (data) => { this.categories = data; }
    });
  }
}
```

### Custom Create Logic

```typescript
override protected createEntity(data: MyEntityCreateUpdateDto): void {
  // Pre-process before sending
  const processedData = {
    ...data,
    name: data.name.trim().toUpperCase()
  };
  super.createEntity(processedData);
}
```

---

## 🧪 Testing

### Service Test Template

```typescript
import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { MyEntityService } from './my-entity.service';

describe('MyEntityService', () => {
  let service: MyEntityService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [MyEntityService]
    });
    service = TestBed.inject(MyEntityService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('should fetch all items', () => {
    const data = [{ id: 1, name: 'Test' }];
    service.getAll().subscribe(items => expect(items).toEqual(data));
    httpMock.expectOne(/api\/my-entities/).flush(data);
  });
});
```

### Component Test Template

```typescript
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MyEntityFormComponent } from './my-entity-form.component';
import { MyEntityService } from './my-entity.service';
import { of } from 'rxjs';

describe('MyEntityFormComponent', () => {
  let component: MyEntityFormComponent;
  let fixture: ComponentFixture<MyEntityFormComponent>;
  let service: jasmine.SpyObj<MyEntityService>;

  beforeEach(async () => {
    const serviceSpy = jasmine.createSpyObj('MyEntityService', ['getAll', 'getById', 'create', 'update']);

    await TestBed.configureTestingModule({
      imports: [MyEntityFormComponent],
      providers: [{ provide: MyEntityService, useValue: serviceSpy }]
    }).compileComponents();

    service = TestBed.inject(MyEntityService) as jasmine.SpyObj<MyEntityService>;
    fixture = TestBed.createComponent(MyEntityFormComponent);
    component = fixture.componentInstance;
  });

  it('should be invalid when empty', () => {
    expect(component.form.valid).toBeFalsy();
  });

  it('should be valid when required fields filled', () => {
    component.form.patchValue({ name: 'Test' });
    expect(component.form.valid).toBeTruthy();
  });
});
```

---

## 📱 Common Patterns

### Pattern: Async Pipe in Template
```html
<div *ngIf="items$ | async as items">
  <div *ngFor="let item of items">{{ item.name }}</div>
</div>
```

### Pattern: Error Handling
```typescript
this.service.getAll().subscribe({
  next: (data) => { /* success */ },
  error: (error) => {
    const msg = error.error?.message || 'Unknown error';
    this.showError(msg);
  }
});
```

### Pattern: Loading State
```html
<div *ngIf="isLoading" class="spinner">Loading...</div>
<div *ngIf="!isLoading">
  <!-- content -->
</div>
```

### Pattern: Optional Fields
```typescript
// In service, concat with null for optional fields
export interface MyEntity extends IEntity {
  id?: number;
  name: string;
  optionalField?: string;  // ← Optional
}
```

---

## 🐛 Troubleshooting

| Issue | Solution |
|-------|----------|
| "No routes matched" | Ensure ID is normalized before navigation |
| Form not populating | Override `populateForm()` method |
| Data not loading | Check API URL in service constructor |
| Category dropdown empty | Load categories in `ngOnInit()` |
| Delete not working | Ensure `deleteItem()` is called on base component |
| Subscriptions not cleaning up | Extend BaseCreateEditComponent (has auto cleanup) |

---

## 📚 More Info

- **Full Architecture Guide:** `ARCHITECTURE.md`
- **Entity Creation Guide:** `ENTITY-CREATION-GUIDE.md`
- **Example Implementation:** Check `features/products/`

---

**Happy coding! 🎉**

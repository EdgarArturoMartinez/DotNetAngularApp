import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { ICrudService } from '../interfaces/entity.interface';
import { IEntity } from '../interfaces/entity.interface';

/**
 * Base class for create/edit components
 * Provides common form handling and CRUD operations
 * 
 * Extend this class in your create/edit components:
 * 
 * @Component({...})
 * export class ProductCreateEditComponent extends BaseCreateEditComponent<Product> {
 *   constructor(
 *     protected override service: ProductService,
 *     protected override router: Router,
 *     protected override activatedRoute: ActivatedRoute,
 *     protected override snackBar: MatSnackBar
 *   ) {
 *     super(service, router, activatedRoute, snackBar);
 *     this.entityName = 'Product';
 *     this.indexRoute = '/products';
 *   }
 *
 *   ngOnInit(): void {
 *     super.ngOnInit();
 *     // Additional initialization specific to your component
 *   }
 * }
 */
@Component({ template: '' })
export abstract class BaseCreateEditComponent<T extends IEntity> implements OnInit, OnDestroy {
  service!: ICrudService<T>;
  router = inject(Router);
  activatedRoute = inject(ActivatedRoute);
  snackBar = inject(MatSnackBar);

  // Override these in child components
  protected entityName = 'Entity';
  protected indexRoute = '/index';

  abstract form: FormGroup;

  entityId: number | null = null;
  isLoading = false;
  isEditMode = false;
  private destroy$ = new Subject<void>();

  ngOnInit(): void {
    this.checkIfEditMode();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  /**
   * Check if this is create or edit mode based on route parameters
   */
  protected checkIfEditMode(): void {
    this.activatedRoute.paramMap.pipe(takeUntil(this.destroy$)).subscribe(params => {
      const id = params.get('id');

      if (id && id !== 'undefined' && id.trim() !== '') {
        this.entityId = parseInt(id, 10);

        if (!isNaN(this.entityId)) {
          this.isEditMode = true;
          this.loadEntity(this.entityId);
        } else {
          this.showError(`Invalid ${this.entityName} ID`);
          this.navigateToIndex();
        }
      } else {
        this.isEditMode = false;
      }
    });
  }

  /**
   * Load entity for edit mode
   * Override in child components if you need custom loading logic
   */
  protected loadEntity(id: number): void {
    this.isLoading = true;
    this.service
      .getById(id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (entity: T) => {
          console.log(`[${this.entityName}] Loaded:`, entity);
          this.populateForm(entity);
          this.isLoading = false;
        },
        error: (error: any) => {
          console.error(`[${this.entityName}] Error loading:`, error);
          const errorMessage = this.extractErrorMessage(error);
          this.showError(`Error: ${errorMessage}`);
          this.isLoading = false;

          setTimeout(() => {
            this.navigateToIndex();
          }, 2000);
        }
      });
  }

  /**
   * Populate form with entity data
   * Abstract method - must be implemented in child components
   */
  protected abstract populateForm(entity: T): void;

  /**
   * Save entity (create or update)
   */
  public saveEntity(): void {
    if (!this.form.valid) {
      this.showError('Please fill in all required fields correctly');
      return;
    }

    const formData = this.form.value;

    if (this.isEditMode && this.entityId) {
      this.updateEntity(this.entityId, formData);
    } else {
      this.createEntity(formData);
    }
  }

  /**
   * Create new entity
   */
  protected createEntity(data: any): void {
    this.service
      .create(data)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response: T) => {
          console.log(`[${this.entityName}] Created:`, response);
          this.showSuccess(`✓ ${this.entityName} created successfully!`);
          this.form.reset();

          setTimeout(() => {
            this.navigateToIndex();
          }, 1000);
        },
        error: (error: any) => {
          console.error(`[${this.entityName}] Error creating:`, error);
          const errorMessage = this.extractErrorMessage(error);
          this.showError(`✗ Error creating ${this.entityName}: ${errorMessage}`);
        }
      });
  }

  /**
   * Update existing entity
   */
  protected updateEntity(id: number, data: any): void {
    this.service
      .update(id, data)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          console.log(`[${this.entityName}] Updated ID ${id}`);
          this.showSuccess(`✓ ${this.entityName} updated successfully!`);

          setTimeout(() => {
            this.navigateToIndex();
          }, 1000);
        },
        error: (error: any) => {
          console.error(`[${this.entityName}] Error updating:`, error);
          const errorMessage = this.extractErrorMessage(error);
          this.showError(`✗ Error updating ${this.entityName}: ${errorMessage}`);
        }
      });
  }

  /**
   * Extract error message from various error formats
   */
  protected extractErrorMessage(error: any): string {
    if (error.status === 0) {
      return `Cannot connect to backend API`;
    }

    if (error.error) {
      if (typeof error.error === 'string') {
        return error.error;
      }
      if (error.error.message) {
        return error.error.message;
      }
      if (error.error.title) {
        return error.error.title;
      }
    }

    return error.message || error.statusText || 'Unknown error';
  }

  /**
   * Navigate to index page
   */
  protected navigateToIndex(): void {
    this.router.navigate([this.indexRoute]);
  }

  /**
   * Show error snackbar
   */
  protected showError(message: string, duration = 5000): void {
    this.snackBar.open(message, 'Close', {
      duration,
      horizontalPosition: 'end',
      verticalPosition: 'bottom',
      panelClass: ['error-snackbar']
    });
  }

  /**
   * Show success snackbar
   */
  protected showSuccess(message: string, duration = 4000): void {
    this.snackBar.open(message, 'Close', {
      duration,
      horizontalPosition: 'end',
      verticalPosition: 'bottom',
      panelClass: ['success-snackbar']
    });
  }

  /**
   * Show warning snackbar
   */
  protected showWarning(message: string, duration = 3000): void {
    this.snackBar.open(message, 'Close', {
      duration,
      horizontalPosition: 'end',
      verticalPosition: 'bottom',
      panelClass: ['warn-snackbar']
    });
  }
}

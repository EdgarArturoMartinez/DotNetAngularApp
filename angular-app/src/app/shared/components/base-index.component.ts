import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ICrudService } from '../interfaces/entity.interface';
import { IEntity } from '../interfaces/entity.interface';

/**
 * Base class for all index/list components
 * Provides common functionality for displaying entities
 * 
 * Extend this class in your index components:
 * 
 * @Component({...})
 * export class ProductIndexComponent extends BaseIndexComponent<Product> {
 *   constructor(
 *     protected override service: ProductService,
 *     protected override router: Router,
 *     protected override snackBar: MatSnackBar
 *   ) {
 *     super(service, router, snackBar);
 *     this.entityName = 'Product';
 *     this.editRoute = '/products/edit';
 *     this.indexRoute = '/products';
 *   }
 * }
 */
@Component({ template: '' })
export abstract class BaseIndexComponent<T extends IEntity> implements OnInit {
  protected service!: ICrudService<T>;
  protected router = inject(Router);
  protected snackBar = inject(MatSnackBar);

  // Override these in child components
  protected entityName = 'Entity';
  protected editRoute = '/edit';
  protected indexRoute = '/index';

  items: T[] = [];
  isLoading = false;
  error = '';

  ngOnInit(): void {
    this.loadItems();
  }

  /**
   * Load all items from the service
   */
  loadItems(): void {
    this.isLoading = true;
    this.error = '';
    this.items = [];

    this.service.getAll().subscribe({
      next: (data: T[]) => {
        console.log(`[${this.entityName}] Loaded:`, data);
        this.items = Array.isArray(data) ? this.normalizeIds(data) : [];
        this.isLoading = false;
      },
      error: (error: any) => {
        console.error(`[${this.entityName}] Error loading:`, error);
        this.error = error.message || 'Failed to load items';
        this.isLoading = false;
      }
    });
  }

  /**
   * Normalize ID properties across different API response formats
   * Handles: id, Id, idEntity, idXXX properties
   */
  protected normalizeIds(items: T[]): T[] {
    return items.map(item => {
      if (!item.id && (item as any).Id) {
        (item as any).id = (item as any).Id;
      }
      return item;
    });
  }

  /**
   * Navigate to edit page for an item
   */
  editItem(item: T): void {
    const id = item.id || (item as any).idEntity || (item as any).idXXX;

    if (!id) {
      this.showError(`Error: ${this.entityName} ID not found. Please refresh and try again.`);
      return;
    }

    this.router.navigate([this.editRoute, id]);
  }

  /**
   * Delete an item with confirmation
   */
  deleteItem(item: T): void {
    const id = item.id || (item as any).idEntity;
    const name = (item as any).name || (item as any).categoryName || `ID ${id}`;

    if (confirm(`Are you sure you want to delete "${name}"?`)) {
      this.service.delete(id).subscribe({
        next: () => {
          this.showSuccess(`✓ ${this.entityName} "${name}" deleted successfully!`);
          this.loadItems();
        },
        error: (error: any) => {
          const errorMessage = error.error?.message || error.statusText || 'Unknown error';
          this.showError(`✗ Error deleting ${this.entityName}: ${errorMessage}`);
        }
      });
    }
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
   * Show info snackbar
   */
  protected showInfo(message: string, duration = 3000): void {
    this.snackBar.open(message, 'Close', {
      duration,
      horizontalPosition: 'end',
      verticalPosition: 'bottom',
      panelClass: ['info-snackbar']
    });
  }
}

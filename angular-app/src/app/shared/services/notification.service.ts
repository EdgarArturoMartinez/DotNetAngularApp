import { Injectable, inject } from '@angular/core';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';

export type NotificationType = 'success' | 'error' | 'warning' | 'info';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private snackBar = inject(MatSnackBar);

  private readonly defaultConfig: MatSnackBarConfig = {
    horizontalPosition: 'end',
    verticalPosition: 'bottom',
    duration: 4000
  };

  /**
   * Show a success notification
   * @param message The message to display
   * @param duration Duration in milliseconds (default: 4000)
   */
  success(message: string, duration?: number): void {
    this.show(message, 'success', duration);
  }

  /**
   * Show an error notification
   * @param message The message to display
   * @param duration Duration in milliseconds (default: 5000)
   */
  error(message: string, duration?: number): void {
    this.show(message, 'error', duration || 5000);
  }

  /**
   * Show a warning notification
   * @param message The message to display
   * @param duration Duration in milliseconds (default: 4000)
   */
  warning(message: string, duration?: number): void {
    this.show(message, 'warning', duration);
  }

  /**
   * Show an info notification
   * @param message The message to display
   * @param duration Duration in milliseconds (default: 3000)
   */
  info(message: string, duration?: number): void {
    this.show(message, 'info', duration || 3000);
  }

  /**
   * Show a notification with custom type
   * @param message The message to display
   * @param type The type of notification
   * @param duration Duration in milliseconds
   */
  private show(message: string, type: NotificationType, duration?: number): void {
    const config: MatSnackBarConfig = {
      ...this.defaultConfig,
      duration: duration || this.defaultConfig.duration,
      panelClass: [`${type}-snackbar`]
    };

    this.snackBar.open(message, 'Close', config);
  }

  /**
   * Convenience methods for CRUD operations
   */

  created(entityName: string, itemName?: string): void {
    const name = itemName ? `"${itemName}"` : 'Item';
    this.success(`✓ ${name} created successfully!`);
  }

  updated(entityName: string, itemName?: string): void {
    const name = itemName ? `"${itemName}"` : 'Item';
    this.success(`✓ ${name} updated successfully!`);
  }

  deleted(entityName: string, itemName?: string): void {
    const name = itemName ? `"${itemName}"` : 'Item';
    this.success(`✓ ${name} deleted successfully!`, 3000);
  }

  loadError(entityName: string, error?: string): void {
    const errorMsg = error ? `: ${error}` : '';
    this.error(`✗ Failed to load ${entityName}${errorMsg}`);
  }

  saveError(action: 'create' | 'update' | 'delete', error?: string): void {
    const errorMsg = error ? `: ${error}` : '';
    this.error(`✗ Error ${action === 'create' ? 'creating' : action === 'update' ? 'updating' : 'deleting'} item${errorMsg}`);
  }

  validationError(message?: string): void {
    this.warning(message || 'Please fill in all required fields correctly', 3000);
  }
}

# Shared Modal & Notification System

## Overview

This document describes the professional, reusable modal and notification system implemented for CRUD operations across the application. The system follows Angular best practices and provides a consistent user experience.

## Architecture

### Components

1. **ConfirmationDialogComponent** (`src/app/shared/components/confirmation-dialog/`)
   - Reusable dialog component for confirmations
   - Supports multiple types: `delete`, `warning`, `info`, `success`
   - Customizable icons, colors, and messages
   - Material Design styling with dark mode support

2. **DialogService** (`src/app/shared/services/dialog.service.ts`)
   - Centralized service for opening dialogs
   - Provides helper methods for common scenarios
   - Returns Observables for easy integration with RxJS

3. **NotificationService** (`src/app/shared/services/notification.service.ts`)
   - Standardized notification system using Material Snackbar
   - Type-specific methods: `success()`, `error()`, `warning()`, `info()`
   - Convenience methods for CRUD operations

## Usage Examples

### 1. Confirmation Dialogs

#### Delete Confirmation
```typescript
import { DialogService } from '../shared/services/dialog.service';

export class MyComponent {
  dialogService = inject(DialogService);
  
  deleteItem(item: any) {
    this.dialogService.confirmDelete('Product', item.name).subscribe(confirmed => {
      if (confirmed) {
        // Proceed with deletion
        this.productService.delete(item.id).subscribe({
          next: () => this.notificationService.deleted('Product', item.name),
          error: (err) => this.notificationService.saveError('delete', err.message)
        });
      }
    });
  }
}
```

#### Custom Warning Dialog
```typescript
this.dialogService.confirmWarning(
  'Unsaved Changes',
  'You have unsaved changes. Do you want to proceed?'
).subscribe(confirmed => {
  if (confirmed) {
    // Navigate away or perform action
  }
});
```

#### Generic Action Confirmation
```typescript
this.dialogService.confirmAction(
  'Submit',
  'Report',
  'This will submit the report for approval. Continue?'
).subscribe(confirmed => {
  if (confirmed) {
    // Submit report
  }
});
```

#### Discard Changes Confirmation
```typescript
canDeactivate(): Observable<boolean> {
  if (this.hasUnsavedChanges) {
    return this.dialogService.confirmDiscardChanges();
  }
  return of(true);
}
```

### 2. Notifications

#### Success Notifications
```typescript
import { NotificationService } from '../shared/services/notification.service';

export class MyComponent {
  notificationService = inject(NotificationService);
  
  // Generic success
  this.notificationService.success('Operation completed successfully!');
  
  // CRUD-specific methods
  this.notificationService.created('Product', 'Organic Carrots');
  this.notificationService.updated('Category', 'Vegetables');
  this.notificationService.deleted('Product', 'Old Item');
}
```

#### Error Notifications
```typescript
// Generic error
this.notificationService.error('An error occurred', 5000);

// CRUD operation errors
this.notificationService.saveError('create', errorMessage);
this.notificationService.saveError('update', errorMessage);
this.notificationService.saveError('delete', errorMessage);

// Load errors
this.notificationService.loadError('products', errorMessage);
```

#### Warning & Info Notifications
```typescript
// Warning
this.notificationService.warning('Please review your input');

// Info
this.notificationService.info('Loading data...', 2000);

// Validation error
this.notificationService.validationError(); // Uses default message
this.notificationService.validationError('Custom validation message');
```

## DialogService API

### Methods

#### `confirm(data: ConfirmationDialogData): Observable<boolean>`
Opens a confirmation dialog with custom configuration.

**Parameters:**
- `title` (string): Dialog title
- `message` (string): Dialog message (supports HTML)
- `confirmText?` (string): Confirm button text (default: "Confirm")
- `cancelText?` (string): Cancel button text (default: "Cancel")
- `type?` ('delete' | 'warning' | 'info' | 'success'): Dialog type (default: "warning")
- `entityName?` (string): Name of entity being acted upon (displayed prominently)

**Returns:** `Observable<boolean>` - Emits `true` if confirmed, `false` if cancelled

#### `confirmDelete(entityName: string, itemName: string): Observable<boolean>`
Quick method for delete confirmations.

#### `confirmWarning(title: string, message: string): Observable<boolean>`
Quick method for warning dialogs.

#### `confirmInfo(title: string, message: string): Observable<boolean>`
Quick method for info dialogs.

#### `confirmAction(action: string, entityName: string, message?: string): Observable<boolean>`
Generic action confirmation.

#### `confirmDiscardChanges(): Observable<boolean>`
Standard discard changes confirmation.

## NotificationService API

### Methods

#### `success(message: string, duration?: number): void`
Show success notification (default: 4000ms)

#### `error(message: string, duration?: number): void`
Show error notification (default: 5000ms)

#### `warning(message: string, duration?: number): void`
Show warning notification (default: 4000ms)

#### `info(message: string, duration?: number): void`
Show info notification (default: 3000ms)

#### CRUD Convenience Methods

- `created(entityName: string, itemName?: string): void`
- `updated(entityName: string, itemName?: string): void`
- `deleted(entityName: string, itemName?: string): void`
- `loadError(entityName: string, error?: string): void`
- `saveError(action: 'create' | 'update' | 'delete', error?: string): void`
- `validationError(message?: string): void`

## Styling

### Dialog Styles

The confirmation dialog includes:
- Color-coded headers based on type (delete: red, warning: orange, info: blue, success: green)
- Material Design elevation and animations
- Dark mode support
- Responsive design (max-width adapts to screen size)
- Keyboard navigation support

### Notification Styles

Global snackbar styles are defined in `src/styles.css`:
- `.success-snackbar` - Green background (#4caf50)
- `.error-snackbar` - Red background (#f44336)
- `.warning-snackbar` - Orange background (#ff9800)
- `.info-snackbar` - Blue background (#2196f3)

## Best Practices

### 1. Always Use Observable Subscriptions

```typescript
// ✅ GOOD
this.dialogService.confirmDelete('Product', name).subscribe(confirmed => {
  if (confirmed) {
    // Handle confirmation
  }
});

// ❌ BAD - Don't use .then() or promises
```

### 2. Provide Context in Messages

```typescript
// ✅ GOOD - Specific and informative
this.notificationService.error('Failed to save product: Server connection timeout');

// ❌ BAD - Vague
this.notificationService.error('Error');
```

### 3. Use Type-Specific Methods

```typescript
// ✅ GOOD
this.notificationService.created('Product', productName);

// ❌ BAD - Less semantic
this.notificationService.success(`✓ Product "${productName}" created successfully!`);
```

### 4. Handle Dialog Cancellation

```typescript
// ✅ GOOD - Check confirmation result
this.dialogService.confirmDelete('Product', name).subscribe(confirmed => {
  if (confirmed) {
    this.deleteProduct(id);
  }
  // Optionally handle cancellation
});
```

### 5. Set Appropriate Durations

```typescript
// Quick messages
this.notificationService.info('Loading...', 2000);

// Standard messages (use defaults)
this.notificationService.success('Item saved');

// Important errors (longer duration)
this.notificationService.error('Connection failed', 7000);
```

## Migration Guide

### From Browser `confirm()` to DialogService

**Before:**
```typescript
if (confirm(\`Delete "${name}"?\`)) {
  this.deleteItem(id);
}
```

**After:**
```typescript
this.dialogService.confirmDelete('Product', name).subscribe(confirmed => {
  if (confirmed) {
    this.deleteItem(id);
  }
});
```

### From MatSnackBar to NotificationService

**Before:**
```typescript
this.snackBar.open('✓ Product created!', 'Close', {
  duration: 4000,
  horizontalPosition: 'end',
  verticalPosition: 'bottom',
  panelClass: ['success-snackbar']
});
```

**After:**
```typescript
this.notificationService.created('Product', productName);
```

## Future Enhancements

Potential improvements for this system:

1. **Loading Indicators** - Progress dialog for long operations
2. **Action Snackbars** - Notifications with action buttons (e.g., "Undo")
3. **Stacked Dialogs** - Support for multiple simultaneous dialogs
4. **Custom Templates** - Allow HTML templates in dialog content
5. **Animation Customization** - Different enter/exit animations
6. **Sound Notifications** - Optional audio feedback for critical actions
7. **Toast Notifications** - Alternative lightweight notification style

## Testing

### Dialog Service Testing

```typescript
import { TestBed } from '@angular/core/testing';
import { MatDialogModule } from '@angular/material/dialog';
import { DialogService } from './dialog.service';

describe('DialogService', () => {
  let service: DialogService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [MatDialogModule]
    });
    service = TestBed.inject(DialogService);
  });

  it('should open delete confirmation dialog', () => {
    service.confirmDelete('Product', 'Test Item').subscribe(result => {
      expect(result).toBeDefined();
    });
  });
});
```

### Notification Service Testing

```typescript
import { TestBed } from '@angular/core/testing';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { NotificationService } from './notification.service';

describe('NotificationService', () => {
  let service: NotificationService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [MatSnackBarModule]
    });
    service = TestBed.inject(NotificationService);
  });

  it('should show success notification', () => {
    expect(() => service.success('Test message')).not.toThrow();
  });
});
```

## Troubleshooting

### Dialog Not Showing

**Issue:** Dialog doesn't appear when called.

**Solution:** Ensure MatDialogModule is imported in your app config:
```typescript
import { MatDialogModule } from '@angular/material/dialog';

export const appConfig: ApplicationConfig = {
  providers: [
    provideAnimationsAsync(),
    importProvidersFrom(MatDialogModule)
  ]
};
```

### Snackbar Styles Not Applied

**Issue:** Notification colors don't show correctly.

**Solution:** Verify `styles.css` is imported in `angular.json`:
```json
"styles": [
  "src/styles.css",
  "src/material-theme.scss"
]
```

### TypeScript Errors with Dialog Data

**Issue:** Type errors when passing data to dialog.

**Solution:** Import and use the ConfirmationDialogData interface:
```typescript
import { ConfirmationDialogData } from '../shared/components/confirmation-dialog/confirmation-dialog.component';

const dialogData: ConfirmationDialogData = {
  title: 'Delete Item',
  message: 'Are you sure?',
  type: 'delete'
};
```

## Summary

This modal and notification system provides:

✅ **Consistent UX** - Standard look and feel across the app  
✅ **Type Safety** - TypeScript interfaces and enums  
✅ **Reusability** - Single implementation used everywhere  
✅ **Maintainability** - Centralized logic easy to update  
✅ **Accessibility** - Keyboard navigation and ARIA labels  
✅ **Flexibility** - Customizable for any use case  
✅ **Best Practices** - Follows Angular Material guidelines  

The system is ready for immediate use and can be extended for future CRUD tables and operations.

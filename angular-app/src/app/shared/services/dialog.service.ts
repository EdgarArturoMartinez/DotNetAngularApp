import { Injectable, inject } from '@angular/core';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { Observable } from 'rxjs';
import { ConfirmationDialogComponent, ConfirmationDialogData } from '../components/confirmation-dialog/confirmation-dialog.component';

@Injectable({
  providedIn: 'root'
})
export class DialogService {
  private dialog = inject(MatDialog);

  /**
   * Open a confirmation dialog
   * @param data Dialog configuration data
   * @returns Observable that emits true if confirmed, false if cancelled
   */
  confirm(data: ConfirmationDialogData): Observable<boolean> {
    const dialogConfig: MatDialogConfig = {
      width: '500px',
      maxWidth: '90vw',
      disableClose: false,
      autoFocus: true,
      data
    };

    const dialogRef = this.dialog.open(ConfirmationDialogComponent, dialogConfig);
    return dialogRef.afterClosed();
  }

  /**
   * Show a delete confirmation dialog
   * @param entityName The name of the entity type (e.g., "Product", "Category")
   * @param itemName The specific item name to delete
   * @returns Observable that emits true if confirmed, false if cancelled
   */
  confirmDelete(entityName: string, itemName: string): Observable<boolean> {
    return this.confirm({
      title: `Delete ${entityName}?`,
      message: `Are you sure you want to delete this ${entityName.toLowerCase()}? This action cannot be undone.`,
      entityName: itemName,
      confirmText: 'Delete',
      cancelText: 'Cancel',
      type: 'delete'
    });
  }

  /**
   * Show a warning confirmation dialog
   * @param title Dialog title
   * @param message Dialog message
   * @returns Observable that emits true if confirmed, false if cancelled
   */
  confirmWarning(title: string, message: string): Observable<boolean> {
    return this.confirm({
      title,
      message,
      confirmText: 'Proceed',
      cancelText: 'Cancel',
      type: 'warning'
    });
  }

  /**
   * Show an info confirmation dialog
   * @param title Dialog title
   * @param message Dialog message
   * @returns Observable that emits true if confirmed, false if cancelled
   */
  confirmInfo(title: string, message: string): Observable<boolean> {
    return this.confirm({
      title,
      message,
      confirmText: 'OK',
      cancelText: 'Cancel',
      type: 'info'
    });
  }

  /**
   * Show a generic action confirmation dialog
   * @param action The action being performed (e.g., "Save", "Submit", "Update")
   * @param entityName The name of the entity
   * @param message Optional custom message
   * @returns Observable that emits true if confirmed, false if cancelled
   */
  confirmAction(action: string, entityName: string, message?: string): Observable<boolean> {
    return this.confirm({
      title: `${action} ${entityName}?`,
      message: message || `Are you sure you want to ${action.toLowerCase()} this ${entityName.toLowerCase()}?`,
      confirmText: action,
      cancelText: 'Cancel',
      type: 'info'
    });
  }

  /**
   * Show a discard changes confirmation dialog
   * @returns Observable that emits true if confirmed, false if cancelled
   */
  confirmDiscardChanges(): Observable<boolean> {
    return this.confirm({
      title: 'Discard Changes?',
      message: 'You have unsaved changes. Are you sure you want to leave this page? All changes will be lost.',
      confirmText: 'Discard',
      cancelText: 'Keep Editing',
      type: 'warning'
    });
  }
}

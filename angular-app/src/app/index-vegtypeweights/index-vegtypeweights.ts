import { Component, OnInit, inject, ChangeDetectorRef, ChangeDetectionStrategy } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { RouterLink, Router } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { CommonModule } from '@angular/common';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { VegTypeWeightService } from '../shared/services/veg-type-weight.service';
import { VegTypeWeight, VegTypeWeightCreateUpdateDto } from '../shared/models/entities';
import { DialogService } from '../shared/services/dialog.service';
import { NotificationService } from '../shared/services/notification.service';
import { GenericDataTableComponent, ColumnDefinition, TableAction } from '../shared/components/generic-data-table/generic-data-table.component';

@Component({
  selector: 'app-index-vegtypeweights',
  standalone: true,
  imports: [MatButtonModule, RouterLink, MatIconModule, CommonModule, MatProgressSpinnerModule, MatTooltipModule, GenericDataTableComponent],
  templateUrl: './index-vegtypeweights.html',
  styleUrl: './index-vegtypeweights.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class IndexVegtypeweights implements OnInit {
  typeWeightService = inject(VegTypeWeightService);
  cdr = inject(ChangeDetectorRef);
  router = inject(Router);
  dialogService = inject(DialogService);
  notificationService = inject(NotificationService);
  typeWeights: VegTypeWeight[] = [];
  isLoading = false;
  error = '';

  // Table configuration
  columns: ColumnDefinition[] = [
    { key: 'idTypeWeight', label: 'ID', type: 'number', width: '80px' },
    { key: 'name', label: 'Name', type: 'text' },
    { key: 'abbreviationWeight', label: 'Abbreviation', type: 'text', width: '120px' },
    { key: 'description', label: 'Description', type: 'text' },
    { key: 'isActive', label: 'Active', type: 'boolean', width: '100px' },
    { key: 'createdAt', label: 'Created', type: 'date', width: '150px' }
  ];

  hiddenColumns: string[] = ['idTypeWeight'];

  actions: TableAction[] = [
    { label: 'Edit', icon: 'edit', action: 'edit', tooltip: 'Edit this weight type', isPrimary: true },
    { label: 'Delete', icon: 'delete', action: 'delete', color: 'warn', tooltip: 'Delete this weight type' }
  ];

  ngOnInit() {
    this.loadTypeWeights();
  }

  loadTypeWeights() {
    this.isLoading = true;
    this.error = '';
    this.typeWeights = [];
    
    console.log('Loading weight types from API...');
    this.typeWeightService.getAll().subscribe({
      next: (data) => {
        console.log('Weight types loaded successfully:', data);
        this.typeWeights = data;
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: (error) => {
        console.error('Error loading weight types:', error);
        if (error.status === 0) {
          this.error = 'Cannot connect to backend. Check CORS settings on the .NET API.';
        } else {
          this.error = 'Failed to load weight types: ' + (error.message || error.statusText || 'Unknown');
        }
        this.isLoading = false;
        this.cdr.markForCheck();
      }
    });
  }

  onEdit(typeWeight: VegTypeWeight) {
    if (!typeWeight.idTypeWeight) {
      this.notificationService.error('Weight Type ID not found. Please refresh and try again.');
      return;
    }
    this.router.navigate(['/admin/weight-types/edit', typeWeight.idTypeWeight]);
  }

  onDelete(typeWeight: VegTypeWeight) {
    this.dialogService.confirmDelete('Weight Type', typeWeight.name).subscribe(confirmed => {
      if (confirmed) {
        this.typeWeightService.delete(typeWeight.idTypeWeight!).subscribe({
          next: () => {
            this.notificationService.deleted('Weight Type', typeWeight.name);
            // Remove the deleted item from the array immediately for instant UI update
            this.typeWeights = this.typeWeights.filter(t => t.idTypeWeight !== typeWeight.idTypeWeight);
            this.cdr.markForCheck();
            // Also reload to ensure data consistency
            this.loadTypeWeights();
          },
          error: (error) => {
            const errorMessage = error.error?.message || error.statusText || 'Unknown error';
            this.notificationService.saveError('delete', errorMessage);
          }
        });
      }
    });
  }

  onBooleanToggle(event: { key: string; item: VegTypeWeight; value: boolean }) {
    if (event.key !== 'isActive') {
      return;
    }

    const previousValue = event.item.isActive;
    const updatedItem = { ...event.item, isActive: event.value };

    this.typeWeights = this.typeWeights.map(t =>
      t.idTypeWeight === event.item.idTypeWeight ? updatedItem : t
    );
    this.cdr.markForCheck();

    const payload: VegTypeWeightCreateUpdateDto = {
      name: event.item.name,
      abbreviationWeight: event.item.abbreviationWeight,
      description: event.item.description,
      isActive: event.value
    };

    this.typeWeightService.update(event.item.idTypeWeight, payload).subscribe({
      next: () => {
        this.notificationService.updated('Weight Type', event.item.name);
      },
      error: (error) => {
        const errorMessage = error.error?.message || error.statusText || 'Unknown error';
        this.typeWeights = this.typeWeights.map(t =>
          t.idTypeWeight === event.item.idTypeWeight ? { ...t, isActive: previousValue } : t
        );
        this.cdr.markForCheck();
        this.notificationService.saveError('update', errorMessage);
      }
    });
  }
}

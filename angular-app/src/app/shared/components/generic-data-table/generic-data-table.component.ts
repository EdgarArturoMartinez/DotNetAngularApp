import { Component, Input, Output, EventEmitter, OnChanges, SimpleChanges, ChangeDetectionStrategy, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatSelectModule } from '@angular/material/select';

export interface ColumnDefinition {
  key: string;
  label: string;
  type?: 'text' | 'currency' | 'date' | 'number' | 'custom' | 'boolean';
  sortable?: boolean;
  width?: string;
  customTemplate?: (item: any) => string;
}

export interface TableAction {
  label: string;
  icon: string;
  color?: string;
  tooltip?: string;
  action: 'edit' | 'delete' | 'view' | 'custom';
  isPrimary?: boolean; // Mark as primary action for special styling
}

@Component({
  selector: 'app-generic-data-table',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatPaginatorModule,
    MatInputModule,
    MatFormFieldModule,
    MatTooltipModule,
    MatSelectModule
  ],
  templateUrl: './generic-data-table.component.html',
  styleUrl: './generic-data-table.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class GenericDataTableComponent implements OnChanges {
  @Input() items: any[] = [];
  @Input() columns: ColumnDefinition[] = [];
  @Input() actions: TableAction[] = [];
  @Input() displayedColumns: string[] = [];
  @Input() hiddenColumns: string[] = [];
  @Input() cardEmoji = ''; // Emoji to display on card headers
  @Input() searchPlaceholder = 'Search...';
  @Input() noDataMessage = 'No data available';
  
  @Output() edit = new EventEmitter<any>();
  @Output() delete = new EventEmitter<any>();
  @Output() view = new EventEmitter<any>();
  @Output() customAction = new EventEmitter<{ action: string; item: any }>();
  @Output() booleanToggle = new EventEmitter<{ key: string; item: any; value: boolean }>();
  @Output() reload = new EventEmitter<void>();

  // Signals for reactive state
  viewMode = signal<'card' | 'list'>('list');
  searchQuery = signal('');
  pageSize = signal(10);
  pageIndex = signal(0);
  columnFilters = signal<{ [key: string]: string }>({});
  sortColumn = signal<string | null>(null);
  sortDirection = signal<'asc' | 'desc' | null>(null);

  // Computed values
  filteredItems = computed(() => {
    const globalQuery = this.searchQuery().toLowerCase();
    const filters = this.columnFilters();
    let filtered = this.items.filter(item => {
      // Global search filter
      if (globalQuery) {
        const matchesGlobalSearch = this.columns.some(col => {
          const value = this.getNestedValue(item, col.key);
          return value?.toString().toLowerCase().includes(globalQuery);
        });
        if (!matchesGlobalSearch) return false;
      }

      // Column-specific filters
      for (const [columnKey, filterValue] of Object.entries(filters)) {
        if (filterValue && filterValue.trim()) {
          const value = this.getNestedValue(item, columnKey);
          if (!value?.toString().toLowerCase().includes(filterValue.toLowerCase())) {
            return false;
          }
        }
      }

      return true;
    });

    // Apply sorting
    const sortCol = this.sortColumn();
    const sortDir = this.sortDirection();
    
    if (sortCol && sortDir) {
      filtered = [...filtered].sort((a, b) => {
        const aValue = this.getNestedValue(a, sortCol);
        const bValue = this.getNestedValue(b, sortCol);

        if (aValue == null || bValue == null) {
          return aValue == null ? 1 : -1;
        }

        let comparison = 0;
        
        if (typeof aValue === 'number' && typeof bValue === 'number') {
          comparison = aValue - bValue;
        } else if (aValue instanceof Date && bValue instanceof Date) {
          comparison = aValue.getTime() - bValue.getTime();
        } else {
          const aStr = String(aValue).toLowerCase();
          const bStr = String(bValue).toLowerCase();
          comparison = aStr.localeCompare(bStr, 'es-CO');
        }

        return sortDir === 'asc' ? comparison : -comparison;
      });
    }

    return filtered;
  });

  paginatedItems = computed(() => {
    const filtered = this.filteredItems();
    const start = this.pageIndex() * this.pageSize();
    return filtered.slice(start, start + this.pageSize());
  });

  totalFilteredRecords = computed(() => this.filteredItems().length);

  displayActionsColumn = computed(() => this.actions.length > 0);

  ngOnChanges(changes: SimpleChanges) {
    if (changes['items'] && !changes['items'].firstChange) {
      // Reset pagination when data changes
      this.pageIndex.set(0);
    }
  }

  toggleViewMode() {
    this.viewMode.set(this.viewMode() === 'card' ? 'list' : 'card');
  }

  onSearchChange(query: string) {
    this.searchQuery.set(query);
    this.pageIndex.set(0); // Reset to first page on search
  }

  onColumnFilterChange(columnKey: string, filterValue: string) {
    const filters = this.columnFilters();
    if (filterValue.trim()) {
      filters[columnKey] = filterValue;
    } else {
      delete filters[columnKey];
    }
    this.columnFilters.set({ ...filters });
    this.pageIndex.set(0); // Reset to first page on filter change
  }

  onSortChange(columnKey: string) {
    const currentSort = this.sortColumn();
    const currentDir = this.sortDirection();

    if (currentSort === columnKey) {
      // Toggle sort direction
      if (currentDir === 'asc') {
        this.sortDirection.set('desc');
      } else if (currentDir === 'desc') {
        this.sortColumn.set(null);
        this.sortDirection.set(null);
      }
    } else {
      // New column, start with ascending
      this.sortColumn.set(columnKey);
      this.sortDirection.set('asc');
    }
    
    this.pageIndex.set(0); // Reset to first page on sort change
  }

  getSortIcon(columnKey: string): string {
    if (this.sortColumn() !== columnKey) {
      return 'unfold_more';
    }
    return this.sortDirection() === 'asc' ? 'arrow_upward' : 'arrow_downward';
  }

  onPageChange(event: PageEvent) {
    this.pageIndex.set(event.pageIndex);
    this.pageSize.set(event.pageSize);
  }

  onPageSizeChange(newSize: number) {
    this.pageSize.set(newSize);
    this.pageIndex.set(0);
  }

  getNestedValue(obj: any, path: string): any {
    return path.split('.').reduce((acc, part) => acc?.[part], obj);
  }

  getDisplayValue(item: any, column: ColumnDefinition): string {
    const value = this.getNestedValue(item, column.key);

    if (column.customTemplate) {
      return column.customTemplate(item);
    }

    switch (column.type) {
      case 'currency':
        return new Intl.NumberFormat('es-CO', {
          style: 'currency',
          currency: 'COP',
          minimumFractionDigits: 0
        }).format(value);
      case 'date':
        return new Date(value).toLocaleDateString('es-CO');
      case 'number':
        return Number(value).toLocaleString('es-CO');
      default:
        return value ?? '-';
    }
  }

  onActionClick(action: TableAction, item: any) {
    switch (action.action) {
      case 'view':
        this.view.emit(item);
        break;
      case 'edit':
        this.edit.emit(item);
        break;
      case 'delete':
        this.delete.emit(item);
        break;
      case 'custom':
        this.customAction.emit({ action: action.label, item });
        break;
    }
  }

  onReload() {
    this.pageIndex.set(0);
    this.searchQuery.set('');
    this.columnFilters.set({});
    this.reload.emit();
  }

  getVisibleColumns(): ColumnDefinition[] {
    return this.columns.filter(col => !this.hiddenColumns.includes(col.key));
  }

  getTableColumns(): string[] {
    const cols = this.displayedColumns.length > 0 
      ? this.displayedColumns.filter(c => !this.hiddenColumns.includes(c))
      : this.getVisibleColumns().map(c => c.key);
    
    if (this.displayActionsColumn()) {
      cols.unshift('actions');
    }
    return cols;
  }

  trackByIndex(index: number): number {
    return index;
  }

  onBooleanToggle(columnKey: string, item: any, value: boolean) {
    this.booleanToggle.emit({ key: columnKey, item, value });
  }
}

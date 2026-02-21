# Generic Data Table Component - Implementation Guide

## Overview

A professional, reusable Angular data table component with the following features:
- **Dual View Modes**: Toggle between card view and list view
- **Advanced Search**: Real-time search across all fields
- **Pagination**: Configurable page sizes (5, 10, 25, 50 items)
- **Responsive Design**: Mobile-optimized Material Design
- **Field Formatting**: Built-in support for currency, dates, numbers, and custom formatting
- **Action Buttons**: Edit, delete, and custom action support
- **Zero Configuration**: Works out-of-the-box with sensible defaults

## Component Location

```
src/app/shared/components/generic-data-table/
├── generic-data-table.component.ts    # Component logic
├── generic-data-table.component.html  # Template
└── generic-data-table.component.css   # Styling
```

## How to Use

### 1. Import the Component

```typescript
import { GenericDataTableComponent, ColumnDefinition, TableAction } from '../shared/components/generic-data-table/generic-data-table.component';
```

### 2. Define Column Configuration

```typescript
columns: ColumnDefinition[] = [
  { key: 'id', label: 'ID', type: 'number', width: '80px' },
  { key: 'name', label: 'Product Name', type: 'text' },
  { key: 'price', label: 'Price', type: 'currency', width: '150px' },
  { key: 'category.categoryName', label: 'Category', type: 'text' },
  { key: 'createdAt', label: 'Created', type: 'date', width: '150px' },
  { key: 'description', label: 'Description', type: 'text' }
];
```

#### Column Definition Properties

- **key** (required): Property path in your data object (supports nested paths like 'category.name')
- **label** (required): Display label for the column
- **type** (optional): Field type for formatting
  - `'text'` - Plain text (default)
  - `'currency'` - COP currency formatting
  - `'date'` - Date formatting (es-CO locale)
  - `'number'` - Number formatting (es-CO locale)
  - `'custom'` - Use customTemplate function
- **width** (optional): Column width (default: auto)
- **sortable** (optional): Enable column sorting (future feature)
- **customTemplate** (optional): Function `(item: any) => string` for custom formatting

### 3. Define Action Buttons

```typescript
actions: TableAction[] = [
  { 
    label: 'Edit', 
    icon: 'edit', 
    action: 'edit', 
    tooltip: 'Edit this item',
    color: 'primary' 
  },
  { 
    label: 'Delete', 
    icon: 'delete', 
    action: 'delete', 
    color: 'warn',
    tooltip: 'Delete this item'
  }
];
```

#### Table Action Properties

- **label** (required): Button text/identifier
- **icon** (required): Material icon name
- **action** (required): Action type - 'edit', 'delete', or custom identifier
- **tooltip** (optional): Hover tooltip text
- **color** (optional): Material color - 'primary', 'accent', 'warn'

### 4. Use in Template

```html
<app-generic-data-table 
  [items]="products"
  [columns]="columns"
  [actions]="actions"
  [displayedColumns]="['id', 'name', 'price']"
  searchPlaceholder="Search products..."
  noDataMessage="No products found"
  (edit)="onEdit($event)"
  (delete)="onDelete($event)"
  (customAction)="onCustomAction($event)"
  (reload)="loadData()"
></app-generic-data-table>
```

### 5. Handle Events in Component

```typescript
onEdit(item: YourType) {
  this.router.navigate(['/edit', item.id]);
}

onDelete(item: YourType) {
  this.dialogService.confirmDelete('Item', item.name).subscribe(confirmed => {
    if (confirmed) {
      this.service.delete(item.id!).subscribe({
        next: () => {
          this.loadData();
        }
      });
    }
  });
}

onCustomAction(event: { action: string; item: YourType }) {
  console.log(`Action: ${event.action}`, event.item);
}

loadData() {
  // Reload your data
}
```

## Component Inputs

| Input | Type | Default | Description |
|-------|------|---------|-------------|
| `items` | `any[]` | `[]` | Data array to display |
| `columns` | `ColumnDefinition[]` | `[]` | Column definitions |
| `actions` | `TableAction[]` | `[]` | Action button configurations |
| `displayedColumns` | `string[]` | All columns | Columns to display (column keys) |
| `hiddenColumns` | `string[]` | `[]` | Column keys to hide from view |
| `cardEmoji` | `string` | `''` | Emoji to display on card headers in card view (e.g., '📋', '🕋', '📦') |
| `searchPlaceholder` | `string` | `'Search...'` | Search input placeholder |
| `noDataMessage` | `string` | `'No data available'` | Message when no data exists |

## Component Outputs

| Output | Type | Description |
|--------|------|-------------|
| `@Output() edit` | `EventEmitter<any>` | Emitted when edit action is clicked |
| `@Output() delete` | `EventEmitter<any>` | Emitted when delete action is clicked |
| `@Output() customAction` | `EventEmitter<{action: string; item: any}>` | Custom action events |
| `@Output() reload` | `EventEmitter<void>` | Emitted when refresh button is clicked |

## Real Examples from Codebase

### Products Component

See implementation in:
- Component TypeScript: `src/app/index-products/index-products.ts`
- Component HTML: `src/app/index-products/index-products.html`

**Key Features Used:**
- Nested field access: `vegCategory.categoryName`
- Currency formatting
- 6 columns with different types

### Categories Component

See implementation in:
- Component TypeScript: `src/app/index-vegcategories/index-vegcategories.ts`
- Component HTML: `src/app/index-vegcategories/index-vegcategories.html`

**Key Features Used:**
- Date formatting
- Multiple field types
- Both edit and delete actions

## Adding to New Entities

### Example: BlogPost Index Component

```typescript
// 1. Import
import { GenericDataTableComponent, ColumnDefinition, TableAction } from '../shared/components/generic-data-table/generic-data-table.component';

// 2. Define columns
columns: ColumnDefinition[] = [
  { key: 'id', label: 'ID', type: 'number', width: '80px' },
  { key: 'title', label: 'Title', type: 'text' },
  { key: 'author', label: 'Author', type: 'text' },
  { key: 'publishedAt', label: 'Published', type: 'date' },
  { key: 'views', label: 'Views', type: 'number' }
];

// 3. Define actions
actions: TableAction[] = [
  { label: 'Edit', icon: 'edit', action: 'edit' },
  { label: 'Delete', icon: 'delete', action: 'delete', color: 'warn' },
  { label: 'View', icon: 'visibility', action: 'view' }
];

// 4. Add in template
// (same as above)
```

## Styling & Customization

### CSS Variables (Future Enhancement)

The component uses hardcoded Material colors. Future versions can support CSS variables for custom theming.

### Current Color Palette

- **Primary**: #4caf50 (Green)
- **Warn**: #f44336 (Red)
- **Text**: #2c3e50 (Dark gray)
- **Secondary text**: #7f8c8d (Light gray)

### Responsive Breakpoints

- Desktop: 1024px and above
- Tablet: 768px - 1024px
- Mobile: 480px - 768px
- Small Mobile: Below 480px

## Migration Notes

### Previous Card-Only View

The old implementation displayed only card views. The new component:
- ✅ Preserves card view functionality
- ✅ Adds professional table/list view
- ✅ Includes search across all fields
- ✅ Adds proper pagination
- ✅ Maintains Material Design consistency

Old component-specific CSS has been removed from:
- `index-products.css`
- `index-vegcategories.css`

## Performance Considerations

- **Change Detection**: Uses `OnPush` strategy for optimal performance
- **Signals**: Uses Angular signals for reactive state management
- **TrackBy**: Implements trackBy for efficient rendering in paginated lists
- **Computed Properties**: Uses computed signals for filtering and pagination

## Future Enhancements

Potential features for future versions:

1. **Sorting**: Click column headers to sort
2. **Filtering**: Advanced filter builder
3. **Cell Templates**: Custom cell templates beyond string formatting
4. **Selection**: Multi-select checkboxes
5. **Export**: CSV/Excel export functionality
6. **Row Actions**: Inline row expansion
7. **Column Reordering**: Drag-to-reorder columns
8. **Theming**: CSS variable support for custom colors
9. **Accessibility**: Enhanced a11y features
10. **Lazy Loading**: Virtual scrolling for large datasets

## Troubleshooting

### Nested Fields Not Displaying

Make sure to use dot notation: `'category.categoryName'`

### Currency Not Formatting

Ensure column type is set to `'currency'`:
```typescript
{ key: 'price', label: 'Price', type: 'currency' }
```

### Actions Not Firing

1. Check component is importing `GenericDataTableComponent`
2. Verify event handler method names match: `(edit)="onEdit($event)"`
3. Ensure `actions` array is defined and not empty

## Support & Examples

For working examples, see:
- Products: `/index-products`
- Categories: `/index-vegcategories`

Both components demonstrate the full feature set with real data and operations.

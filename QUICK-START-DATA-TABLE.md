# Quick Start Guide: Adding Data Tables to New Entities

## 📋 5-Minute Setup

### Step 1: Import Required Components (30 seconds)

```typescript
import { GenericDataTableComponent, ColumnDefinition, TableAction } 
  from '../shared/components/generic-data-table/generic-data-table.component';
```

Add to your component's `imports`:
```typescript
imports: [
  // ... existing imports
  GenericDataTableComponent
],
```

### Step 2: Define Your Columns (1 minute)

```typescript
columns: ColumnDefinition[] = [
  { key: 'id', label: 'ID', type: 'number', width: '80px' },
  { key: 'name', label: 'Name', type: 'text' },
  { key: 'email', label: 'Email', type: 'text' },
  { key: 'createdAt', label: 'Created', type: 'date', width: '150px' }
];
```

**Column Type Quick Reference:**
- `'text'` - Plain text (default)
- `'number'` - Formatted number
- `'currency'` - COP formatting
- `'date'` - Date formatting

### Step 3: Define Your Actions (1 minute)

```typescript
actions: TableAction[] = [
  { label: 'Edit', icon: 'edit', action: 'edit', tooltip: 'Edit' },
  { label: 'Delete', icon: 'delete', action: 'delete', color: 'warn', tooltip: 'Delete' }
];
```

### Step 4: Add to Template (1 minute)

```html
<app-generic-data-table 
  [items]="items"
  [columns]="columns"
  [actions]="actions"
  [cardEmoji]="'📦'"
  searchPlaceholder="Search {{ entityName }}..."
  [noDataMessage]="'No ' + entityName + ' found'"
  (edit)="onEdit($event)"
  (delete)="onDelete($event)"
  (reload)="loadData()"
></app-generic-data-table>
```

**Note on cardEmoji:** Replace `'📦'` with the emoji you want displayed on card headers in card view. See **Step 4.5** below.

### Step 4.5: Choose Your Card Emoji (30 seconds) ⭐ NEW

The `cardEmoji` property displays an emoji icon on every card header in card view. This is administrator-configurable per entity.

```typescript
// In your component TypeScript file:
@Component({...})
export class YourEntityComponent {
  // Choose ONE emoji that represents your entity
  cardEmoji = '📦'; // For packages/items
  // OR use: '📋' for lists, '🕋' for categories, '📁' for folders, etc.
}
```

Then in your template:
```html
<app-generic-data-table 
  [items]="items"
  [columns]="columns"
  [actions]="actions"
  [cardEmoji]="cardEmoji"
  (edit)="onEdit($event)"
  (delete)="onDelete($event)"
  (reload)="loadData()"
></app-generic-data-table>
```

**Recommended Emojis by Entity Type:**
| Entity | Emoji | Example |
|--------|-------|---------|
| Products | 📋 | `[cardEmoji]="'📋'"` |
| Categories | 🕋 | `[cardEmoji]="'🕋'"` |
| Users | 👤 | `[cardEmoji]="'👤'"` |
| Orders | 📦 | `[cardEmoji]="'📦'"` |
| Invoices | 📄 | `[cardEmoji]="'📄'"` |
| Settings | ⚙️ | `[cardEmoji]="'⚙️'"` |
| Reports | 📊 | `[cardEmoji]="'📊'"` |

Each card in card view will display your chosen emoji in a styled badge on the header, along with your primary field.

### Step 5: Add Event Handlers (1-2 minutes)

```typescript
onEdit(item: YourType) {
  this.router.navigate(['/your-path/edit', item.id]);
}

onDelete(item: YourType) {
  this.dialogService.confirmDelete('Item Name', item.name).subscribe(confirmed => {
    if (confirmed) {
      this.service.delete(item.id!).subscribe({
        next: () => {
          this.notificationService.deleted('Item', item.name);
          this.loadData();
        }
      });
    }
  });
}

loadData() {
  this.service.getAll().subscribe(data => {
    this.items = data;
  });
}
```

---

## 🎨 Real Example: Blog Posts

### TypeScript Component

```typescript
import { Component, OnInit, inject } from '@angular/core';
import { GenericDataTableComponent, ColumnDefinition, TableAction } from '../shared/components/generic-data-table/generic-data-table.component';
import { BlogPostService } from '../services/blog-post.service';

@Component({
  selector: 'app-blog-posts',
  imports: [GenericDataTableComponent],
  template: `
    <div class="posts-wrapper">
      <h1>Blog Posts</h1>
      <app-generic-data-table 
        [items]="posts"
        [columns]="columns"
        [actions]="actions"
        searchPlaceholder="Search posts by title or author..."
        (edit)="onEdit($event)"
        (delete)="onDelete($event)"
        (reload)="loadPosts()"
      ></app-generic-data-table>
    </div>
  `
})
export class BlogPostsComponent implements OnInit {
  postService = inject(BlogPostService);
  posts: any[] = [];

  columns: ColumnDefinition[] = [
    { key: 'id', label: 'ID', type: 'number', width: '60px' },
    { key: 'title', label: 'Title', type: 'text' },
    { key: 'author', label: 'Author', type: 'text' },
    { key: 'publishedAt', label: 'Published', type: 'date', width: '120px' },
    { key: 'views', label: 'Views', type: 'number', width: '100px' }
  ];

  actions: TableAction[] = [
    { label: 'Edit', icon: 'edit', action: 'edit' },
    { label: 'Delete', icon: 'delete', action: 'delete', color: 'warn' }
  ];

  ngOnInit() {
    this.loadPosts();
  }

  loadPosts() {
    this.postService.getAll().subscribe(data => {
      this.posts = data;
    });
  }

  onEdit(post: any) {
    // Navigate to edit page
  }

  onDelete(post: any) {
    // Delete logic
  }
}
```

---

## 🪲 Common Patterns

### Pattern 1: Nested Object Access
```typescript
// If your data has nested objects:
// { id: 1, user: { name: 'John', email: 'john@...' } }

columns: ColumnDefinition[] = [
  { key: 'user.name', label: 'User Name', type: 'text' },
  { key: 'user.email', label: 'Email', type: 'text' }
];
```

### Pattern 2: Custom Value Formatting
```typescript
// For complex formatting needs:
columns: ColumnDefinition[] = [
  { 
    key: 'status', 
    label: 'Status',
    type: 'custom',
    customTemplate: (item) => item.status.toUpperCase()
  }
];
```

### Pattern 3: Hide Sensitive Columns
```typescript
// Only show specific columns in the table:
displayedColumns = ['id', 'name', 'email'];
// Show all columns except these when card view is enabled
```

### Pattern 4: Conditional Actions
```typescript
// Show different actions based on item state:
// (In future enhancement - currently all items see same actions)
```

---

## ✅ Checklist for New Entity

- [ ] Import `GenericDataTableComponent` and types
- [ ] Define `columns` array with ColumnDefinition objects
- [ ] Define `actions` array with TableAction objects  
- [ ] Choose appropriate `cardEmoji` for your entity type
- [ ] Add `<app-generic-data-table>` to template with emoji binding
- [ ] Implement `onEdit()` handler
- [ ] Implement `onDelete()` handler
- [ ] Implement `loadData()` handler
- [ ] Test search functionality
- [ ] Test pagination (add 15+ items)
- [ ] Test view toggle and emoji visibility on cards
- [ ] Test on mobile screen

---

## 🎯 What You Get Automatically

Just by adding the component, users get:

✅ List view with sortable-looking headers
✅ Card view with responsive grid + emoji icons
✅ Real-time search across all fields
✅ Pagination with page size selector
✅ Display count indicator
✅ Refresh button
✅ View toggle button
✅ Professional Material Design styling
✅ Mobile responsive layout
✅ Loading states (from parent)
✅ Empty state messages
✅ Date/currency formatting
✅ Action buttons with tooltips
✅ Configurable emoji icons per entity

---

## 📖 Complete Documentation

For detailed information, see:
- **GENERIC-DATA-TABLE-GUIDE.md** - Full API reference and advanced usage
- **DATA-TABLE-IMPLEMENTATION-SUMMARY.md** - Architecture and design decisions

---

## 🚀 Example Code Templates

### Copy-Paste Template 1: Simple CRUD List

```typescript
import { Component, OnInit, inject } from '@angular/core';
import { Router } from '@angular/router';
import { GenericDataTableComponent, ColumnDefinition, TableAction } from '../shared/components/generic-data-table/generic-data-table.component';
import { YourService } from './your.service';
import { DialogService } from '../shared/services/dialog.service';
import { NotificationService } from '../shared/services/notification.service';

@Component({
  selector: 'app-your-list',
  standalone: true,
  imports: [GenericDataTableComponent],
  template: `
    <div class="wrapper">
      <div class="header">
        <h1>Your Items</h1>
        <button mat-flat-button routerLink="/new">Add New</button>
      </div>
      <app-generic-data-table 
        [items]="items"
        [columns]="columns"
        [actions]="actions"
        (edit)="onEdit($event)"
        (delete)="onDelete($event)"
        (reload)="loadItems()"
      ></app-generic-data-table>
    </div>
  `
})
export class YourListComponent implements OnInit {
  service = inject(YourService);
  router = inject(Router);
  dialogService = inject(DialogService);
  notificationService = inject(NotificationService);
  
  items: any[] = [];

  columns: ColumnDefinition[] = [
    { key: 'id', label: 'ID', type: 'number', width: '80px' },
    { key: 'name', label: 'Name', type: 'text' }
  ];

  actions: TableAction[] = [
    { label: 'Edit', icon: 'edit', action: 'edit' },
    { label: 'Delete', icon: 'delete', action: 'delete', color: 'warn' }
  ];

  ngOnInit() { this.loadItems(); }

  loadItems() {
    this.service.getAll().subscribe(data => {
      this.items = data;
    });
  }

  onEdit(item: any) {
    this.router.navigate(['/edit', item.id]);
  }

  onDelete(item: any) {
    this.dialogService.confirmDelete('Item', item.name).subscribe(confirmed => {
      if (confirmed) {
        this.service.delete(item.id).subscribe({
          next: () => {
            this.notificationService.deleted('Item', item.name);
            this.loadItems();
          }
        });
      }
    });
  }
}
```

---

## 🎓 Learning Path

1. **Start with**: Production examples in codebase
   - `src/app/index-products/`
   - `src/app/index-vegcategories/`

2. **Read**: This quick start guide

3. **Copy**: Template from above

4. **Customize**: For your entity

5. **Test**: With real data

6. **Refer to**: Full API docs when needed

---

## ❓ FAQ

**Q: How do I show/hide specific columns?**
A: Use the `displayedColumns` input to specify which column keys to display.

**Q: Can I add custom actions?**
A: Yes! Set `action` to any string and handle it in `(customAction)` output.

**Q: How do I format complex data?**
A: Use `customTemplate` function in ColumnDefinition.

**Q: Does it work with large datasets?**
A: Works well up to ~1000 rows. For larger datasets, consider server-side pagination.

**Q: Can I change the colors?**
A: Currently uses app's green color. Future update will support theming via CSS variables.

**Q: How do I add sorting?**
A: Currently not implemented. See GENERIC-DATA-TABLE-GUIDE.md for future enhancements.

---

## 📞 Support

If you encounter issues:

1. Check the complete **GENERIC-DATA-TABLE-GUIDE.md**
2. Look at working examples: Products and Categories pages
3. Verify column key paths match your data structure
4. Check browser console for Angular errors
5. Ensure all imports are correct

---

**Time to implement: ~5-10 minutes per entity** ⚡

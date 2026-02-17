import { Routes } from '@angular/router';
import { Landing } from './landing/landing';

// Legacy imports (currently active)
import { IndexProducts } from './index-products/index-products';
import { CreateVegproduct } from './create-vegproduct/create-vegproduct';
import { EditVegproduct } from './edit-vegproduct/edit-vegproduct';
import { IndexVegcategories } from './index-vegcategories/index-vegcategories';
import { CreateVegcategory } from './create-vegcategory/create-vegcategory';
import { EditVegcategory } from './edit-vegcategory/edit-vegcategory';

// New architecture imports from features folder (commented out - features folder was removed)
// import { ProductIndexComponent } from './features/products/product-index.component';
// import { ProductCreateEditComponent } from './features/products/product-form.component';
// import { CategoryIndexComponent } from './features/categories/category-index.component';
// import { CategoryCreateEditComponent } from './features/categories/category-form.component';

/**
 * Application Routes
 * 
 * Note: Currently using legacy paths to maintain backward compatibility.
 * Once all components are migrated to new architecture in features/ folder,
 * switch to the new components below.
 */
export const routes: Routes = [
    { path: '', component: Landing },
    
    // Products routes - Currently using legacy components for backward compatibility
    // TODO: Switch to ProductIndexComponent and ProductCreateEditComponent once legacy components are removed
    { path: 'products', component: IndexProducts },
    { path: 'products/create', component: CreateVegproduct },
    { path: 'products/edit/:id', component: EditVegproduct },
    
    // Categories routes - Currently using legacy components for backward compatibility
    // TODO: Switch to CategoryIndexComponent and CategoryCreateEditComponent once legacy components are removed
    { path: 'categories', component: IndexVegcategories },
    { path: 'categories/create', component: CreateVegcategory },
    { path: 'categories/edit/:id', component: EditVegcategory },
    
    // Future new entities will be added here
    // Each new entity should follow the same pattern:
    // { path: 'entity-name', component: EntityIndexComponent },
    // { path: 'entity-name/create', component: EntityCreateEditComponent },
    // { path: 'entity-name/edit/:id', component: EntityCreateEditComponent }
];

/**
 * MIGRATION PLAN TO NEW ARCHITECTURE:
 * 
 * Phase 1 (Current): Set up new architecture and services
 * - ✅ Create BaseCrudService and base components
 * - ✅ Create new components in features/ folder
 * - Create new services using BaseCrudService
 * 
 * Phase 2: Test and validate new components
 * - Run tests on new architecture
 * - Ensure no breaking changes
 * 
 * Phase 3: Switch routes to new components
 * - Update routes to use new components
 * - Monitor for errors
 * 
 * Phase 4: Remove legacy components
 * - Delete old component folders
 * - Clean up old services
 * 
 * Benefits of new architecture:
 * - 50% less code duplication
 * - Consistent error handling across entities
 * - Easier to add new entities
 * - Better testability
 * - Follows SOLID principles
 */

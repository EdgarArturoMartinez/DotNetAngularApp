import { Routes } from '@angular/router';
import { Landing } from './landing/landing';
import { HomeComponent } from './home/home';

// Legacy imports (currently active)
import { IndexProducts } from './index-products/index-products';
import { CreateVegproduct } from './create-vegproduct/create-vegproduct';
import { EditVegproduct } from './edit-vegproduct/edit-vegproduct';
import { IndexVegcategories } from './index-vegcategories/index-vegcategories';
import { CreateVegcategory } from './create-vegcategory/create-vegcategory';
import { EditVegcategory } from './edit-vegcategory/edit-vegcategory';
import { IndexVegtypeweights } from './index-vegtypeweights/index-vegtypeweights';
import { CreateVegtypeweight } from './create-vegtypeweight/create-vegtypeweight';
import { EditVegtypeweight } from './edit-vegtypeweight/edit-vegtypeweight';

// New architecture imports from features folder (commented out - features folder was removed)
// import { ProductIndexComponent } from './features/products/product-index.component';
// import { ProductCreateEditComponent } from './features/products/product-form.component';
// import { CategoryIndexComponent } from './features/categories/category-index.component';
// import { CategoryCreateEditComponent } from './features/categories/category-form.component';

/**
 * Application Routes
 * 
 * Architecture:
 * - Public routes (/) - eCommerce customer-facing pages
 * - Admin routes (/admin/*) - Admin panel for inventory management
 * 
 * The admin routes are protected (future: will add authentication guards)
 */
export const routes: Routes = [
    // ===================================
    // PUBLIC ROUTES - eCommerce Site
    // ===================================
    { 
        path: '', 
        component: HomeComponent,
        title: 'VeggyWorldShop - Fresh Organic Vegetables'
    },
    
    // Future public routes:
    // { path: 'shop', component: ShopComponent },
    // { path: 'products/:id', component: ProductDetailComponent },
    // { path: 'cart', component: CartComponent },
    // { path: 'checkout', component: CheckoutComponent },
    
    // ===================================
    // ADMIN ROUTES - Admin Panel
    // ===================================
    { 
        path: 'admin', 
        component: Landing,
        title: 'Admin Dashboard - VeggyWorldShop'
    },
    
    // Products Management
    { 
        path: 'admin/products', 
        component: IndexProducts,
        title: 'Manage Products - Admin'
    },
    { 
        path: 'admin/products/create', 
        component: CreateVegproduct,
        title: 'Create Product - Admin'
    },
    { 
        path: 'admin/products/edit/:id', 
        component: EditVegproduct,
        title: 'Edit Product - Admin'
    },
    
    // Categories Management
    { 
        path: 'admin/categories', 
        component: IndexVegcategories,
        title: 'Manage Categories - Admin'
    },
    { 
        path: 'admin/categories/create', 
        component: CreateVegcategory,
        title: 'Create Category - Admin'
    },
    { 
        path: 'admin/categories/edit/:id', 
        component: EditVegcategory,
        title: 'Edit Category - Admin'
    },
    
    // Weight Types Management
    { 
        path: 'admin/weight-types', 
        component: IndexVegtypeweights,
        title: 'Manage Weight Types - Admin'
    },
    { 
        path: 'admin/weight-types/create', 
        component: CreateVegtypeweight,
        title: 'Create Weight Type - Admin'
    },
    { 
        path: 'admin/weight-types/edit/:id', 
        component: EditVegtypeweight,
        title: 'Edit Weight Type - Admin'
    },
    
    // Fallback for old admin URLs (redirect to new admin routes)
    { path: 'products', redirectTo: 'admin/products', pathMatch: 'full' },
    { path: 'products/create', redirectTo: 'admin/products/create', pathMatch: 'full' },
    { path: 'products/edit/:id', redirectTo: 'admin/products/edit/:id', pathMatch: 'full' },
    { path: 'categories', redirectTo: 'admin/categories', pathMatch: 'full' },
    { path: 'categories/create', redirectTo: 'admin/categories/create', pathMatch: 'full' },
    { path: 'categories/edit/:id', redirectTo: 'admin/categories/edit/:id', pathMatch: 'full' },
];

/**
 * FUTURE ENHANCEMENTS:
 * 
 * 1. Authentication & Authorization
 *    - Add AuthGuard for admin routes
 *    - Implement login/logout functionality
 *    - JWT token management
 * 
 * 2. Public Shopping Features
 *    - Product catalog with filtering/search
 *    - Product detail pages
 *    - Shopping cart functionality
 *    - Checkout process
 *    - Order tracking
 * 
 * 3. Admin Features
 *    - Order management
 *    - Customer management
 *    - Inventory tracking
 *    - Sales analytics
 *    - Report generation
 */

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

// Authentication imports
import { LoginComponent } from './features/login/login.component';
import { RegisterComponent } from './features/register/register.component';
import { ForgotPasswordComponent } from './features/forgot-password/forgot-password.component';
import { ResetPasswordComponent } from './features/reset-password/reset-password.component';
import { authGuard, adminGuard, guestGuard } from './core/guards/auth.guard';

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
 * - Auth routes (/login, /register) - Authentication pages
 * - Admin routes (/admin/*) - Admin panel for inventory management (protected by adminGuard)
 */
export const routes: Routes = [
    // ===================================
    // AUTHENTICATION ROUTES
    // ===================================
    { 
        path: 'login', 
        component: LoginComponent,
        canActivate: [guestGuard],  // Redirect to home if already logged in
        title: 'Login - VeggyWorldShop'
    },
    { 
        path: 'register', 
        component: RegisterComponent,
        canActivate: [guestGuard],  // Redirect to home if already logged in
        title: 'Register - VeggyWorldShop'
    },
    { 
        path: 'forgot-password', 
        component: ForgotPasswordComponent,
        canActivate: [guestGuard],  // Redirect to home if already logged in
        title: 'Forgot Password - VeggyWorldShop'
    },
    { 
        path: 'reset-password', 
        component: ResetPasswordComponent,
        canActivate: [guestGuard],  // Redirect to home if already logged in
        title: 'Reset Password - VeggyWorldShop'
    },
    
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
        canActivate: [adminGuard],  // Protected: Admin only
        title: 'Admin Dashboard - VeggyWorldShop'
    },
    
    // Products Management
    { 
        path: 'admin/products', 
        component: IndexProducts,
        canActivate: [adminGuard],  // Protected: Admin only
        title: 'Manage Products - Admin'
    },
    { 
        path: 'admin/products/create', 
        component: CreateVegproduct,
        canActivate: [adminGuard],  // Protected: Admin only
        title: 'Create Product - Admin'
    },
    { 
        path: 'admin/products/edit/:id', 
        component: EditVegproduct,
        canActivate: [adminGuard],  // Protected: Admin only
        title: 'Edit Product - Admin'
    },
    
    // Categories Management
    { 
        path: 'admin/categories', 
        component: IndexVegcategories,
        canActivate: [adminGuard],  // Protected: Admin only
        title: 'Manage Categories - Admin'
    },
    { 
        path: 'admin/categories/create', 
        component: CreateVegcategory,
        canActivate: [adminGuard],  // Protected: Admin only
        title: 'Create Category - Admin'
    },
    { 
        path: 'admin/categories/edit/:id', 
        component: EditVegcategory,
        canActivate: [adminGuard],  // Protected: Admin only
        title: 'Edit Category - Admin'
    },
    
    // Weight Types Management
    { 
        path: 'admin/weight-types', 
        component: IndexVegtypeweights,
        canActivate: [adminGuard],  // Protected: Admin only
        title: 'Manage Weight Types - Admin'
    },
    { 
        path: 'admin/weight-types/create', 
        component: CreateVegtypeweight,
        canActivate: [adminGuard],  // Protected: Admin only
        title: 'Create Weight Type - Admin'
    },
    { 
        path: 'admin/weight-types/edit/:id', 
        component: EditVegtypeweight,
        canActivate: [adminGuard],  // Protected: Admin only
        title: 'Edit Weight Type - Admin'
    },
    
    // User Management
    { 
        path: 'admin/users', 
        loadComponent: () => import('./features/manage-users/manage-users.component').then(m => m.ManageUsersComponent),
        canActivate: [adminGuard],  // Protected: Admin only
        title: 'Manage Users - Admin'
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
 * AUTHENTICATION & AUTHORIZATION:
 * ✅ JWT-based authentication implemented
 * ✅ AuthGuard protects routes requiring authentication
 * ✅ AdminGuard protects admin-only routes
 * ✅ GuestGuard redirects logged-in users from login/register pages
 * 
 * FUTURE ENHANCEMENTS:
 * 
 * 1. Customer Features
 *    - Profile page (view/edit user info)
 *    - Order history
 *    - Wishlist functionality
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
 *    - Customer management UI
 *    - Inventory tracking
 *    - Sales analytics
 *    - Report generation
 */

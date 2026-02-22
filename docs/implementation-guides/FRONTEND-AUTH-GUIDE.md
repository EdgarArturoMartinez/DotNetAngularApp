# Frontend Authentication Implementation Guide

## Overview
This guide shows how to use the Customer authentication system in your Angular frontend application. All components follow standalone Angular patterns with signals and modern best practices.

---

## 🎯 Files Created

### Core Services & Models
- ✅ `core/models/auth.models.ts` - TypeScript interfaces matching backend DTOs
- ✅ `core/services/auth.service.ts` - Authentication service with JWT management
- ✅ `core/interceptors/jwt.interceptor.ts` - HTTP interceptor for automatic token attachment
- ✅ `core/guards/auth.guard.ts` - Route guards (authGuard, adminGuard, guestGuard)

### Components
- ✅ `features/login/login.component.ts|html|css` - Login page
- ✅ `features/register/register.component.ts|html|css` - Registration page

### Configuration
- ✅ `app.config.ts` - Updated with JWT interceptor

---

## 🚀 Quick Start

### 1. Update Your Routes (app.routes.ts)

```typescript
import { Routes } from '@angular/router';
import { authGuard, adminGuard, guestGuard } from './core/guards/auth.guard';
import { LoginComponent } from './features/login/login.component';
import { RegisterComponent } from './features/register/register.component';

export const routes: Routes = [
  // Public routes (redirect to home if already logged in)
  {
    path: 'login',
    component: LoginComponent,
    canActivate: [guestGuard]
  },
  {
    path: 'register',
    component: RegisterComponent,
    canActivate: [guestGuard]
  },

  // Protected routes (require authentication)
  {
    path: 'profile',
    loadComponent: () => import('./features/profile/profile.component').then(m => m.ProfileComponent),
    canActivate: [authGuard]
  },

  // Admin-only routes
  {
    path: 'admin',
    canActivate: [adminGuard],
    children: [
      {
        path: 'customers',
        loadComponent: () => import('./features/admin/customers/customers.component').then(m => m.CustomersComponent)
      },
      // ... other admin routes
    ]
  },

  { path: '', redirectTo: '/home', pathMatch: 'full' }
];
```

---

## 💡 Usage Examples

### Example 1: Login in a Component

```typescript
import { Component } from '@angular/core';
import { AuthService } from '../../core/services/auth.service';
import { CustomerLoginDto } from '../../core/models/auth.models';

export class SomeComponent {
  constructor(private authService: AuthService) {}

  login() {
    const credentials: CustomerLoginDto = {
      email: 'user@example.com',
      password: 'password123'
    };

    this.authService.login(credentials).subscribe({
      next: (response) => {
        console.log('Logged in:', response.customer);
        // Token is automatically stored
        // Navigate to dashboard or home
      },
      error: (error) => {
        console.error('Login failed:', error);
      }
    });
  }
}
```

### Example 2: Check Authentication Status

```typescript
import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../core/services/auth.service';
import { CustomerDto } from '../../core/models/auth.models';

export class HeaderComponent implements OnInit {
  currentUser: CustomerDto | null = null;
  isAuthenticated = false;

  constructor(private authService: AuthService) {}

  ngOnInit() {
    // Subscribe to current user changes
    this.authService.currentUser$.subscribe(user => {
      this.currentUser = user;
      this.isAuthenticated = !!user;
    });

    // Or check synchronously
    this.isAuthenticated = this.authService.isAuthenticated();
  }

  logout() {
    this.authService.logout();
  }
}
```

### Example 3: Display User Information

```typescript
// In your component
export class ProfileComponent implements OnInit {
  user$ = this.authService.currentUser$;

  constructor(private authService: AuthService) {}

  ngOnInit() {
    // Refresh profile from server
    this.authService.getProfile().subscribe();
  }
}
```

```html
<!-- In your template -->
<div *ngIf="user$ | async as user">
  <h2>Welcome, {{ user.firstName }} {{ user.lastName }}!</h2>
  <p>Email: {{ user.email }}</p>
  <p>Role: {{ user.role === 1 ? 'Admin' : 'Customer' }}</p>
</div>
```

### Example 4: Update Profile

```typescript
updateProfile() {
  const updateData: CustomerUpdateDto = {
    firstName: 'John',
    lastName: 'Doe',
    phoneNumber: '+1234567890',
    streetAddress: '123 Main St',
    city: 'New York',
    stateProvince: 'NY',
    postalCode: '10001',
    country: 'USA'
  };

  this.authService.updateProfile(updateData).subscribe({
    next: (updatedUser) => {
      console.log('Profile updated:', updatedUser);
    },
    error: (error) => {
      console.error('Update failed:', error);
    }
  });
}
```

### Example 5: Change Password

```typescript
changePassword() {
  const passwordData: ChangePasswordDto = {
    currentPassword: 'oldPassword123',
    newPassword: 'newPassword456'
  };

  this.authService.changePassword(passwordData).subscribe({
    next: () => {
      console.log('Password changed successfully');
    },
    error: (error) => {
      console.error('Password change failed:', error);
    }
  });
}
```

### Example 6: Check Admin Role

```typescript
export class AdminPanelComponent implements OnInit {
  isAdmin = false;

  constructor(private authService: AuthService) {}

  ngOnInit() {
    this.isAdmin = this.authService.isAdmin();

    if (!this.isAdmin) {
      // Redirect or show error
      console.error('Access denied: Admin only');
    }
  }
}
```

### Example 7: Conditional UI Based on Role

```html
<!-- Show different menus based on role -->
<nav>
  <a routerLink="/home">Home</a>
  
  <ng-container *ngIf="authService.isAuthenticated()">
    <a routerLink="/profile">My Profile</a>
    <a routerLink="/orders">My Orders</a>
    
    <a *ngIf="authService.isAdmin()" routerLink="/admin">Admin Panel</a>
    
    <button (click)="authService.logout()">Logout</button>
  </ng-container>
  
  <ng-container *ngIf="!authService.isAuthenticated()">
    <a routerLink="/login">Login</a>
    <a routerLink="/register">Register</a>
  </ng-container>
</nav>
```

---

## 🔐 How JWT Tokens Work

### Automatic Token Attachment
The `jwtInterceptor` automatically attaches the JWT token to all HTTP requests:

```typescript
// You don't need to manually add Authorization header
this.http.get('/api/vegproducts').subscribe(...);
// The interceptor adds: Authorization: Bearer <token>
```

### Token Storage
- Tokens are stored in `localStorage` with key `jwt_token`
- User data is stored in `localStorage` with key `current_user`
- Tokens expire after 24 hours (configured on backend)

### Auto Logout on 401
If the API returns 401 (token expired), the interceptor automatically:
1. Clears the token
2. Redirects to login page

---

## 🛡️ Route Protection

### Public Routes (No Auth Required)
```typescript
{ path: 'home', component: HomeComponent }
```

### Protected Routes (Auth Required)
```typescript
{
  path: 'profile',
  component: ProfileComponent,
  canActivate: [authGuard]  // Redirects to /login if not authenticated
}
```

### Admin-Only Routes
```typescript
{
  path: 'admin',
  component: AdminComponent,
  canActivate: [adminGuard]  // Redirects to / if not admin
}
```

### Guest-Only Routes (Login/Register)
```typescript
{
  path: 'login',
  component: LoginComponent,
  canActivate: [guestGuard]  // Redirects to / if already logged in
}
```

---

## 📊 Admin Panel Example

For admin functionality, you can create a service to manage customers:

```typescript
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CustomerDto, CustomerAdminDto } from '../models/auth.models';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CustomerManagementService {
  private apiUrl = `${environment.apiURL}/api/customers`;

  constructor(private http: HttpClient) {}

  getAllCustomers(): Observable<CustomerDto[]> {
    return this.http.get<CustomerDto[]>(this.apiUrl);
  }

  getCustomerById(id: number): Observable<CustomerDto> {
    return this.http.get<CustomerDto>(`${this.apiUrl}/${id}`);
  }

  createCustomer(data: CustomerAdminDto): Observable<CustomerDto> {
    return this.http.post<CustomerDto>(this.apiUrl, data);
  }

  updateCustomer(id: number, data: CustomerAdminDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, data);
  }

  deleteCustomer(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getCustomersByRole(role: number): Observable<CustomerDto[]> {
    return this.http.get<CustomerDto[]>(`${this.apiUrl}/role/${role}`);
  }
}
```

---

## 🎨 Styling Notes

The login and register components use a purple gradient theme. To match your app's theme:

1. Update the gradient in CSS:
```css
background: linear-gradient(135deg, #your-color-1 0%, #your-color-2 100%);
```

2. Update the button colors:
```css
.btn-primary {
  background-color: #your-primary-color;
}
```

---

## 🧪 Testing the Implementation

### 1. Start the Backend API
```powershell
cd DotNetCoreWebApi\DotNetCoreWebApi
dotnet run
```
Backend runs on: https://localhost:7020

### 2. Start the Angular App
```powershell
cd angular-app
npm start
```
Frontend runs on: http://localhost:4200

### 3. Test the Flow
1. Navigate to `/register` and create an account
2. Login with your credentials at `/login`
3. You'll be redirected to home (authenticated)
4. Access protected routes (e.g., `/profile`)
5. Try accessing admin routes (should fail if not admin)
6. Logout and verify token is cleared

---

## 🔧 Common Issues & Solutions

### Issue: CORS Errors
**Solution:** Backend already has CORS configured in Program.cs. Ensure it's running.

### Issue: Token Not Attached to Requests
**Solution:** Verify `app.config.ts` has `withInterceptors([jwtInterceptor])`.

### Issue: Login Successful but User Still Redirected
**Solution:** Check if token is stored: Open DevTools → Application → Local Storage → Check for `jwt_token`.

### Issue: Admin Routes Accessible to Regular Users
**Solution:** Ensure routes use `canActivate: [adminGuard]` not just `[authGuard]`.

---

## 📝 Next Steps

1. ✅ **Routes**: Add login/register routes to `app.routes.ts`
2. ✅ **Navigation**: Update your menu/header to show login/logout buttons
3. ✅ **Profile Page**: Create a profile component for users to view/edit their info
4. ✅ **Admin Panel**: Create admin management interface (if needed)
5. ✅ **Error Handling**: Add global error handler for better UX
6. ✅ **Loading States**: Add loading spinners during API calls

---

## 🎓 Key Takeaways

- **AuthService** handles all authentication logic
- **JWT Interceptor** automatically adds tokens to requests
- **Guards** protect routes based on authentication/authorization
- **currentUser$** observable provides reactive user state
- **localStorage** persists authentication across page refreshes
- Token auto-expires after 24 hours (backend configured)

---

**You're all set!** The authentication system is fully functional and ready to use. 🚀

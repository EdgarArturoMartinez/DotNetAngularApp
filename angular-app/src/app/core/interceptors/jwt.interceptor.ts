import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { catchError, throwError } from 'rxjs';
import { Router } from '@angular/router';

/**
 * JWT Interceptor - Automatically adds JWT token to outgoing requests
 */
export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  const token = authService.getToken();

  // Clone the request and add authorization header if token exists
  if (token) {
    req = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }

  // Handle errors (like 401 Unauthorized)
  return next(req).pipe(
    catchError((error) => {
      if (error.status === 401) {
        // Only logout if this is NOT an auth endpoint (login, register, etc.)
        // Auth endpoints naturally return 401 for wrong credentials - we want to show error messages
        const isAuthEndpoint = req.url.includes('/auth/login') || 
                               req.url.includes('/auth/register') || 
                               req.url.includes('/auth/forgot-password') || 
                               req.url.includes('/auth/reset-password');
        
        if (!isAuthEndpoint && token) {
          // Token expired or invalid on a protected route - logout user
          authService.logout();
        }
      }
      return throwError(() => error);
    })
  );
};

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject, tap } from 'rxjs';
import { Router } from '@angular/router';
import { 
  CustomerRegisterDto, 
  CustomerLoginDto, 
  AuthResponseDto, 
  CustomerDto, 
  CustomerUpdateDto,
  ChangePasswordDto,
  ForgotPasswordDto,
  ResetPasswordDto,
  UserRole 
} from '../models/auth.models';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = `${environment.apiURL}/api/auth`;
  private currentUserSubject = new BehaviorSubject<CustomerDto | null>(this.getUserFromStorage());
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient, private router: Router) { }

  /**
   * Register a new customer
   */
  register(data: CustomerRegisterDto): Observable<AuthResponseDto> {
    return this.http.post<AuthResponseDto>(`${this.apiUrl}/register`, data).pipe(
      tap(response => this.handleAuthResponse(response))
    );
  }

  /**
   * Login with email and password
   */
  login(data: CustomerLoginDto): Observable<AuthResponseDto> {
    return this.http.post<AuthResponseDto>(`${this.apiUrl}/login`, data).pipe(
      tap(response => this.handleAuthResponse(response))
    );
  }

  /**
   * Get current user profile
   */
  getProfile(): Observable<CustomerDto> {
    return this.http.get<CustomerDto>(`${this.apiUrl}/profile`).pipe(
      tap(customer => this.currentUserSubject.next(customer))
    );
  }

  /**
   * Update current user profile
   */
  updateProfile(data: CustomerUpdateDto): Observable<CustomerDto> {
    return this.http.put<CustomerDto>(`${this.apiUrl}/profile`, data).pipe(
      tap(customer => {
        this.currentUserSubject.next(customer);
        this.saveUserToStorage(customer);
      })
    );
  }

  /**
   * Change password
   */
  changePassword(data: ChangePasswordDto): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/change-password`, data);
  }

  /**
   * Check if email exists
   */
  checkEmailExists(email: string): Observable<{ exists: boolean }> {
    return this.http.get<{ exists: boolean }>(`${this.apiUrl}/check-email?email=${encodeURIComponent(email)}`);
  }

  /**
   * Request password reset (forgot password)
   */
  forgotPassword(data: ForgotPasswordDto): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.apiUrl}/forgot-password`, data);
  }

  /**
   * Reset password with token
   */
  resetPassword(data: ResetPasswordDto): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.apiUrl}/reset-password`, data);
  }

  /**
   * Logout user
   */
  logout(): void {
    localStorage.removeItem('jwt_token');
    localStorage.removeItem('current_user');
    this.currentUserSubject.next(null);
    this.router.navigate(['/login']);
  }

  /**
   * Get JWT token from storage
   */
  getToken(): string | null {
    return localStorage.getItem('jwt_token');
  }

  /**
   * Check if user is authenticated
   */
  isAuthenticated(): boolean {
    const token = this.getToken();
    if (!token) return false;
    
    // Check if token is expired
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const expirationDate = new Date(payload.exp * 1000);
      return expirationDate > new Date();
    } catch {
      return false;
    }
  }

  /**
   * Check if current user is admin
   */
  isAdmin(): boolean {
    const user = this.currentUserSubject.value;
    return user?.role === UserRole.Admin;
  }

  /**
   * Get current user synchronously
   */
  getCurrentUser(): CustomerDto | null {
    return this.currentUserSubject.value;
  }

  /**
   * Handle authentication response
   */
  private handleAuthResponse(response: AuthResponseDto): void {
    localStorage.setItem('jwt_token', response.token);
    this.saveUserToStorage(response.customer);
    this.currentUserSubject.next(response.customer);
  }

  /**
   * Save user to local storage
   */
  private saveUserToStorage(customer: CustomerDto): void {
    localStorage.setItem('current_user', JSON.stringify(customer));
  }

  /**
   * Get user from local storage
   */
  private getUserFromStorage(): CustomerDto | null {
    const user = localStorage.getItem('current_user');
    return user ? JSON.parse(user) : null;
  }
}

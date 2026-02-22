import { Component, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink, ActivatedRoute } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { CustomerLoginDto } from '../../core/models/auth.models';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  loginForm: FormGroup;
  errorMessage = '';
  isLoading = false;
  returnUrl = '/';
  showPassword = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute,
    private cdr: ChangeDetectorRef
  ) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });

    // Get return URL from route parameters or default to '/'
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
  }

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      this.markFormGroupTouched(this.loginForm);
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    const loginData: CustomerLoginDto = this.loginForm.value;

    this.authService.login(loginData).subscribe({
      next: (response) => {
        console.log('Login successful', response);
        this.isLoading = false;
        this.router.navigate([this.returnUrl]);
      },
      error: (error) => {
        console.error('Login failed', error);
        console.error('Error status:', error.status);
        console.error('Error error:', error.error);
        console.error('Error message:', error.error?.message);
        this.isLoading = false;
        
        // Extract error message from backend response
        if (error.error?.message) {
          this.errorMessage = error.error.message;
          console.log('Set errorMessage from backend:', this.errorMessage);
        } else if (error.status === 401) {
          this.errorMessage = 'Invalid email or password. Please check your credentials and try again.';
          console.log('Set errorMessage for 401:', this.errorMessage);
        } else if (error.status === 403) {
          this.errorMessage = 'Account is inactive. Please contact support.';
          console.log('Set errorMessage for 403:', this.errorMessage);
        } else if (error.status === 0) {
          this.errorMessage = 'Cannot connect to server. Please check your internet connection.';
          console.log('Set errorMessage for connection error:', this.errorMessage);
        } else {
          this.errorMessage = 'An error occurred. Please try again later.';
          console.log('Set errorMessage default:', this.errorMessage);
        }
        console.log('Final errorMessage value:', this.errorMessage);
        
        // Force Angular to detect changes
        this.cdr.detectChanges();
      }
    });
  }

  // Helper method to mark all fields as touched
  private markFormGroupTouched(formGroup: FormGroup): void {
    Object.keys(formGroup.controls).forEach(key => {
      const control = formGroup.get(key);
      control?.markAsTouched();
    });
  }

  // Getters for form controls
  get email() { return this.loginForm.get('email'); }
  get password() { return this.loginForm.get('password'); }
}

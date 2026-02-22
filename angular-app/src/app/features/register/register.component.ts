import { Component, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, AbstractControl } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { CustomerRegisterDto } from '../../core/models/auth.models';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {
  registerForm: FormGroup;
  errorMessage = '';
  isLoading = false;
  showPassword = false;
  showConfirmPassword = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {
    this.registerForm = this.fb.group({
      email: ['', [Validators.required, Validators.email], [this.emailAsyncValidator.bind(this)]],
      password: ['', [
        Validators.required, 
        Validators.minLength(8),
        Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]+$/)
      ]],
      confirmPassword: ['', [Validators.required]],
      firstName: ['', [Validators.required, Validators.minLength(2)]],
      lastName: ['', [Validators.required, Validators.minLength(2)]],
      phoneNumber: ['', [Validators.pattern(/^\+?[\d\s\-()]+$/)]]
    }, { validators: this.passwordMatchValidator });
  }

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  toggleConfirmPasswordVisibility(): void {
    this.showConfirmPassword = !this.showConfirmPassword;
  }

  // Custom validator to check if passwords match
  passwordMatchValidator(control: AbstractControl): { [key: string]: boolean } | null {
    const password = control.get('password');
    const confirmPassword = control.get('confirmPassword');

    if (!password || !confirmPassword) {
      return null;
    }

    return password.value === confirmPassword.value ? null : { passwordMismatch: true };
  }

  // Async validator to check if email already exists
  emailAsyncValidator(control: AbstractControl) {
    if (!control.value) {
      return Promise.resolve(null);
    }

    return new Promise(resolve => {
      this.authService.checkEmailExists(control.value).subscribe({
        next: (response) => {
          resolve(response.exists ? { emailTaken: true } : null);
        },
        error: () => {
          resolve(null);
        }
      });
    });
  }

  onSubmit(): void {
    if (this.registerForm.invalid) {
      this.markFormGroupTouched(this.registerForm);
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    const registerData: CustomerRegisterDto = {
      email: this.registerForm.value.email,
      password: this.registerForm.value.password,
      firstName: this.registerForm.value.firstName,
      lastName: this.registerForm.value.lastName,
      phoneNumber: this.registerForm.value.phoneNumber || undefined
    };

    this.authService.register(registerData).subscribe({
      next: (response) => {
        console.log('Registration successful', response);
        this.isLoading = false;
        this.router.navigate(['/']);
      },
      error: (error) => {
        console.error('Registration failed', error);
        console.error('Error status:', error.status);
        console.error('Error error:', error.error);
        console.error('Error message:', error.error?.message);
        console.error('Full error object:', JSON.stringify(error.error));
        this.isLoading = false;
        
        // Extract error message from backend response
        if (error.error?.message) {
          this.errorMessage = error.error.message;
          console.log('Set errorMessage from backend:', this.errorMessage);
        } else if (error.error?.errors) {
          // Handle validation errors from ASP.NET Core
          const validationErrors = Object.values(error.error.errors).flat();
          this.errorMessage = validationErrors.join('. ');
          console.log('Set errorMessage from validation errors:', this.errorMessage);
        } else if (typeof error.error === 'string') {
          this.errorMessage = error.error;
          console.log('Set errorMessage from string error:', this.errorMessage);
        } else if (error.status === 400) {
          this.errorMessage = 'Invalid registration data. Please check your information and try again.';
          console.log('Set errorMessage for 400:', this.errorMessage);
        } else if (error.status === 409) {
          this.errorMessage = 'This email is already registered. Please use a different email or login.';
          console.log('Set errorMessage for 409:', this.errorMessage);
        } else if (error.status === 0) {
          this.errorMessage = 'Cannot connect to server. Please check your internet connection.';
          console.log('Set errorMessage for connection error:', this.errorMessage);
        } else {
          this.errorMessage = 'An error occurred during registration. Please try again later.';
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
  get email() { return this.registerForm.get('email'); }
  get password() { return this.registerForm.get('password'); }
  get confirmPassword() { return this.registerForm.get('confirmPassword'); }
  get firstName() { return this.registerForm.get('firstName'); }
  get lastName() { return this.registerForm.get('lastName'); }
  get phoneNumber() { return this.registerForm.get('phoneNumber'); }
}

import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, AbstractControl } from '@angular/forms';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { ResetPasswordDto } from '../../core/models/auth.models';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  template: `
    <div class="reset-password-container">
      <div class="reset-password-card">
        <h2>Reset Password</h2>
        <p class="subtitle">Enter your new password</p>
        
        <form [formGroup]="resetPasswordForm" (ngSubmit)="onSubmit()">
          <div class="form-group">
            <label for="email">Email Address</label>
            <input 
              type="email" 
              id="email" 
              formControlName="email"
              class="form-control"
              [class.error]="email?.invalid && email?.touched"
              placeholder="Enter your email"
            />
            <div class="error-message" *ngIf="email?.invalid && email?.touched">
              <span *ngIf="email?.errors?.['required']">Email is required</span>
              <span *ngIf="email?.errors?.['email']">Please enter a valid email</span>
            </div>
          </div>

          <div class="form-group">
            <label for="token">Reset Token</label>
            <input 
              type="text" 
              id="token" 
              formControlName="token"
              class="form-control"
              [class.error]="token?.invalid && token?.touched"
              placeholder="Enter reset token from email"
            />
            <div class="error-message" *ngIf="token?.invalid && token?.touched">
              <span *ngIf="token?.errors?.['required']">Reset token is required</span>
            </div>
          </div>

          <div class="form-group">
            <label for="newPassword">New Password</label>
            <div class="password-input-wrapper">
              <input 
                [type]="showNewPassword ? 'text' : 'password'" 
                id="newPassword" 
                formControlName="newPassword"
                class="form-control"
                [class.error]="newPassword?.invalid && newPassword?.touched"
                placeholder="Enter new password"
              />
              <button 
                type="button" 
                class="toggle-password-btn"
                (click)="toggleNewPasswordVisibility()"
                [attr.aria-label]="showNewPassword ? 'Hide password' : 'Show password'"
              >
                {{ showNewPassword ? '👁️' : '👁️‍🗨️' }}
              </button>
            </div>
            <div class="error-message" *ngIf="newPassword?.invalid && newPassword?.touched">
              <span *ngIf="newPassword?.errors?.['required']">Password is required</span>
              <span *ngIf="newPassword?.errors?.['minlength']">Password must be at least 8 characters</span>
            </div>
          </div>

          <div class="form-group">
            <label for="confirmPassword">Confirm Password</label>
            <div class="password-input-wrapper">
              <input 
                [type]="showConfirmPassword ? 'text' : 'password'" 
                id="confirmPassword" 
                formControlName="confirmPassword"
                class="form-control"
                [class.error]="(confirmPassword?.invalid && confirmPassword?.touched) || resetPasswordForm.errors?.['passwordMismatch']"
                placeholder="Confirm new password"
              />
              <button 
                type="button" 
                class="toggle-password-btn"
                (click)="toggleConfirmPasswordVisibility()"
                [attr.aria-label]="showConfirmPassword ? 'Hide password' : 'Show password'"
              >
                {{ showConfirmPassword ? '👁️' : '👁️‍🗨️' }}
              </button>
            </div>
            <div class="error-message" *ngIf="confirmPassword?.touched">
              <span *ngIf="confirmPassword?.errors?.['required']">Please confirm your password</span>
              <span *ngIf="resetPasswordForm.errors?.['passwordMismatch'] && !confirmPassword?.errors?.['required']">
                Passwords do not match
              </span>
            </div>
          </div>

          <div class="alert alert-success" *ngIf="successMessage">
            {{ successMessage }}
          </div>

          <div class="alert alert-error" *ngIf="errorMessage">
            {{ errorMessage }}
          </div>

          <button 
            type="submit" 
            class="btn btn-primary"
            [disabled]="isLoading || resetPasswordForm.invalid"
          >
            <span *ngIf="!isLoading">Reset Password</span>
            <span *ngIf="isLoading">Resetting...</span>
          </button>

          <div class="reset-password-footer">
            Remember your password? 
            <a routerLink="/login" class="login-link">Login here</a>
          </div>
        </form>
      </div>
    </div>
  `,
  styles: [`
    .reset-password-container {
      display: flex;
      justify-content: center;
      align-items: center;
      min-height: 100vh;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      padding: 20px;
    }

    .reset-password-card {
      background: white;
      padding: 40px;
      border-radius: 10px;
      box-shadow: 0 10px 25px rgba(0, 0, 0, 0.2);
      width: 100%;
      max-width: 450px;
    }

    .reset-password-card h2 {
      margin-bottom: 10px;
      text-align: center;
      color: #333;
      font-size: 24px;
    }

    .subtitle {
      text-align: center;
      color: #666;
      margin-bottom: 30px;
      font-size: 14px;
    }

    .form-group {
      margin-bottom: 20px;
    }

    .form-group label {
      display: block;
      margin-bottom: 8px;
      color: #555;
      font-weight: 500;
      font-size: 14px;
    }

    .form-control {
      width: 100%;
      padding: 12px;
      border: 1px solid #ddd;
      border-radius: 5px;
      font-size: 14px;
      transition: border-color 0.3s;
      box-sizing: border-box;
    }

    .password-input-wrapper {
      position: relative;
    }

    .password-input-wrapper .form-control {
      padding-right: 45px;
    }

    .toggle-password-btn {
      position: absolute;
      right: 10px;
      top: 50%;
      transform: translateY(-50%);
      background: none;
      border: none;
      cursor: pointer;
      font-size: 18px;
      padding: 5px;
      line-height: 1;
      opacity: 0.6;
      transition: opacity 0.2s;
    }

    .toggle-password-btn:hover {
      opacity: 1;
    }

    .form-control:focus {
      outline: none;
      border-color: #667eea;
    }

    .form-control.error {
      border-color: #e74c3c;
    }

    .error-message {
      color: #e74c3c;
      font-size: 12px;
      margin-top: 5px;
    }

    .alert {
      padding: 12px;
      border-radius: 5px;
      margin-bottom: 20px;
      font-size: 14px;
    }

    .alert-error {
      background-color: #fee;
      color: #c33;
      border: 1px solid #fcc;
    }

    .alert-success {
      background-color: #efe;
      color: #3c3;
      border: 1px solid #cfc;
    }

    .btn {
      width: 100%;
      padding: 12px;
      border: none;
      border-radius: 5px;
      font-size: 16px;
      font-weight: 600;
      cursor: pointer;
      transition: background-color 0.3s;
    }

    .btn-primary {
      background-color: #667eea;
      color: white;
    }

    .btn-primary:hover:not(:disabled) {
      background-color: #5568d3;
    }

    .btn:disabled {
      opacity: 0.6;
      cursor: not-allowed;
    }

    .reset-password-footer {
      text-align: center;
      margin-top: 20px;
      color: #666;
      font-size: 14px;
    }

    .login-link {
      color: #667eea;
      text-decoration: none;
      font-weight: 600;
    }

    .login-link:hover {
      text-decoration: underline;
    }
  `]
})
export class ResetPasswordComponent implements OnInit {
  resetPasswordForm: FormGroup;
  errorMessage = '';
  successMessage = '';
  isLoading = false;
  showNewPassword = false;
  showConfirmPassword = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute,
    private cdr: ChangeDetectorRef
  ) {
    this.resetPasswordForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      token: ['', [Validators.required]],
      newPassword: ['', [Validators.required, Validators.minLength(8)]],
      confirmPassword: ['', [Validators.required]]
    }, { validators: this.passwordMatchValidator });
  }

  toggleNewPasswordVisibility(): void {
    this.showNewPassword = !this.showNewPassword;
  }

  toggleConfirmPasswordVisibility(): void {
    this.showConfirmPassword = !this.showConfirmPassword;
  }

  ngOnInit(): void {
    // Get token from query params if provided
    this.route.queryParams.subscribe(params => {
      if (params['token']) {
        this.resetPasswordForm.patchValue({ token: params['token'] });
      }
      if (params['email']) {
        this.resetPasswordForm.patchValue({ email: params['email'] });
      }
    });
  }

  passwordMatchValidator(control: AbstractControl): { [key: string]: boolean } | null {
    const password = control.get('newPassword');
    const confirmPassword = control.get('confirmPassword');

    if (!password || !confirmPassword) {
      return null;
    }

    return password.value === confirmPassword.value ? null : { passwordMismatch: true };
  }

  onSubmit(): void {
    if (this.resetPasswordForm.invalid) {
      this.markFormGroupTouched(this.resetPasswordForm);
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';
    this.successMessage = '';

    const resetPasswordData: ResetPasswordDto = {
      email: this.resetPasswordForm.value.email,
      token: this.resetPasswordForm.value.token,
      newPassword: this.resetPasswordForm.value.newPassword
    };

    this.authService.resetPassword(resetPasswordData).subscribe({
      next: (response) => {
        console.log('Password reset successful', response);
        this.isLoading = false;
        this.successMessage = response.message + ' Redirecting to login...';
        this.cdr.detectChanges();
        
        // Redirect to login after 2 seconds
        setTimeout(() => {
          this.router.navigate(['/login']);
        }, 2000);
      },
      error: (error) => {
        console.error('Password reset failed', error);
        console.error('Error status:', error.status);
        console.error('Error error:', error.error);
        this.isLoading = false;
        
        // Extract error message from backend response
        if (error.error?.message) {
          this.errorMessage = error.error.message;
        } else if (error.status === 400) {
          this.errorMessage = 'Invalid or expired reset token. Please request a new password reset link.';
        } else if (error.status === 0) {
          this.errorMessage = 'Cannot connect to server. Please check your internet connection.';
        } else {
          this.errorMessage = 'An error occurred. Please try again later.';
        }
        this.cdr.detectChanges();
      }
    });
  }

  private markFormGroupTouched(formGroup: FormGroup): void {
    Object.keys(formGroup.controls).forEach(key => {
      const control = formGroup.get(key);
      control?.markAsTouched();
    });
  }

  get email() { return this.resetPasswordForm.get('email'); }
  get token() { return this.resetPasswordForm.get('token'); }
  get newPassword() { return this.resetPasswordForm.get('newPassword'); }
  get confirmPassword() { return this.resetPasswordForm.get('confirmPassword'); }
}

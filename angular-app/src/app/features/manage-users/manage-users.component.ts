import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { CustomerService, CustomerAdminDto, CreateCustomerRequest } from '../../core/services/customer.service';
import { CustomerDto, UserRole } from '../../core/models/auth.models';
import { DialogService } from '../../shared/services/dialog.service';
import { NotificationService } from '../../shared/services/notification.service';

@Component({
  selector: 'app-manage-users',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './manage-users.component.html',
  styleUrls: ['./manage-users.component.css']
})
export class ManageUsersComponent implements OnInit {
  customers: CustomerDto[] = [];
  filteredCustomers: CustomerDto[] = [];
  isLoading = false;
  errorMessage = '';
  successMessage = '';
  
  // Form
  showForm = false;
  isEditMode = false;
  customerForm: FormGroup;
  selectedCustomerId: number | null = null;
  
  // Filter
  filterRole: 'all' | UserRole = 'all';
  searchTerm = '';
  
  UserRole = UserRole;

  constructor(
    private customerService: CustomerService,
    private fb: FormBuilder,
    private dialogService: DialogService,
    private notificationService: NotificationService,
    private cdr: ChangeDetectorRef
  ) {
    this.customerForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      firstName: ['', [Validators.required, Validators.minLength(2)]],
      lastName: ['', [Validators.required, Validators.minLength(2)]],
      phoneNumber: [''],
      role: [UserRole.Customer, [Validators.required]],
      isActive: [true, [Validators.required]],
      password: ['', [Validators.required, Validators.minLength(8)]]
    });
  }

  ngOnInit(): void {
    this.loadCustomers();
  }

  loadCustomers(): void {
    this.isLoading = true;
    this.errorMessage = '';
    
    console.log('Loading customers from API...');
    this.customerService.getAllCustomers().subscribe({
      next: (customers) => {
        console.log('Customers loaded successfully:', customers);
        this.customers = customers;
        this.applyFilters();
        this.isLoading = false;
        this.cdr.detectChanges();
        console.log('isLoading set to false. Should show table now.');
      },
      error: (error) => {
        console.error('Error loading customers:', error);
        console.error('Error status:', error.status);
        console.error('Error message:', error.message);
        
        if (error.status === 0) {
          this.errorMessage = 'Cannot connect to server. Please ensure the backend is running.';
        } else if (error.status === 401) {
          this.errorMessage = 'Unauthorized. Please login as an admin.';
        } else if (error.status === 403) {
          this.errorMessage = 'Access forbidden. Admin role required.';
        } else {
          this.errorMessage = error.error?.message || 'Failed to load customers. Please try again.';
        }
        
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  applyFilters(): void {
    console.log('applyFilters called. Total customers:', this.customers.length);
    console.log('Filter role:', this.filterRole);
    console.log('Search term:', this.searchTerm);
    
    let filtered = [...this.customers];
    
    // Filter by role
    if (this.filterRole !== 'all') {
      filtered = filtered.filter(c => c.role === this.filterRole);
      console.log('After role filter:', filtered.length);
    }
    
    // Filter by search term
    if (this.searchTerm.trim()) {
      const term = this.searchTerm.toLowerCase();
      filtered = filtered.filter(c =>
        c.email.toLowerCase().includes(term) ||
        c.firstName.toLowerCase().includes(term) ||
        c.lastName.toLowerCase().includes(term)
      );
      console.log('After search filter:', filtered.length);
    }
    
    this.filteredCustomers = filtered;
    console.log('Final filteredCustomers:', this.filteredCustomers.length);
    console.log('isLoading:', this.isLoading);
  }

  onFilterRoleChange(role: 'all' | UserRole): void {
    this.filterRole = role;
    this.applyFilters();
  }

  onSearchChange(): void {
    this.applyFilters();
  }

  showCreateForm(): void {
    this.isEditMode = false;
    this.showForm = true;
    this.selectedCustomerId = null;
    this.customerForm.reset({
      role: UserRole.Customer,
      isActive: true
    });
    // Password required for create
    this.customerForm.get('password')?.setValidators([Validators.required, Validators.minLength(8)]);
    this.customerForm.get('password')?.updateValueAndValidity();
  }

  showEditForm(customer: CustomerDto): void {
    this.isEditMode = true;
    this.showForm = true;
    this.selectedCustomerId = customer.id;
    
    this.customerForm.patchValue({
      email: customer.email,
      firstName: customer.firstName,
      lastName: customer.lastName,
      phoneNumber: customer.phoneNumber,
      role: customer.role,
      isActive: customer.isActive
    });
    
    // Password not required for edit
    this.customerForm.get('password')?.clearValidators();
    this.customerForm.get('password')?.updateValueAndValidity();
  }

  hideForm(): void {
    this.showForm = false;
    this.customerForm.reset();
    this.selectedCustomerId = null;
    this.errorMessage = '';
    this.successMessage = '';
  }

  onSubmit(): void {
    if (this.customerForm.invalid) {
      this.markFormGroupTouched(this.customerForm);
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';
    this.successMessage = '';

    if (this.isEditMode && this.selectedCustomerId) {
      // Update existing customer
      const adminDto: CustomerAdminDto = {
        email: this.customerForm.value.email,
        firstName: this.customerForm.value.firstName,
        lastName: this.customerForm.value.lastName,
        phoneNumber: this.customerForm.value.phoneNumber,
        role: this.customerForm.value.role,
        isActive: this.customerForm.value.isActive
      };

      this.customerService.updateCustomer(this.selectedCustomerId, adminDto).subscribe({
        next: (response) => {
          this.successMessage = 'Customer updated successfully!';
          this.isLoading = false;
          this.loadCustomers();
          setTimeout(() => this.hideForm(), 2000);
        },
        error: (error) => {
          console.error('Error updating customer', error);
          this.errorMessage = error.error?.message || 'Failed to update customer. Please try again.';
          this.isLoading = false;
        }
      });
    } else {
      // Create new customer
      const request: CreateCustomerRequest = {
        customer: {
          email: this.customerForm.value.email,
          firstName: this.customerForm.value.firstName,
          lastName: this.customerForm.value.lastName,
          phoneNumber: this.customerForm.value.phoneNumber,
          role: this.customerForm.value.role,
          isActive: this.customerForm.value.isActive
        },
        password: this.customerForm.value.password
      };

      this.customerService.createCustomer(request).subscribe({
        next: (customer) => {
          this.successMessage = 'Customer created successfully!';
          this.isLoading = false;
          this.loadCustomers();
          setTimeout(() => this.hideForm(), 2000);
        },
        error: (error) => {
          console.error('Error creating customer', error);
          this.errorMessage = error.error?.message || 'Failed to create customer. Please try again.';
          this.isLoading = false;
        }
      });
    }
  }

  deleteCustomer(customer: CustomerDto): void {
    const fullName = `${customer.firstName} ${customer.lastName}`;
    
    this.dialogService.confirmDelete('User', fullName).subscribe(confirmed => {
      if (!confirmed) {
        return;
      }

      this.isLoading = true;
      this.errorMessage = '';
      this.successMessage = '';

      this.customerService.deleteCustomer(customer.id).subscribe({
        next: (response) => {
          this.notificationService.deleted('User', fullName);
          // Remove the deleted customer from the arrays immediately for instant UI update
          this.customers = this.customers.filter(c => c.id !== customer.id);
          this.filteredCustomers = this.filteredCustomers.filter(c => c.id !== customer.id);
          this.isLoading = false;
          // Also reload to ensure data consistency with backend
          this.loadCustomers();
        },
        error: (error) => {
          console.error('Error deleting customer', error);
          const errorMessage = error.error?.message || error.statusText || 'Unknown error';
          this.notificationService.saveError('delete', errorMessage);
          this.isLoading = false;
        }
      });
    });
  }

  getRoleName(role: UserRole): string {
    return role === UserRole.Admin ? 'Admin' : 'Customer';
  }

  private markFormGroupTouched(formGroup: FormGroup): void {
    Object.keys(formGroup.controls).forEach(key => {
      const control = formGroup.get(key);
      control?.markAsTouched();
    });
  }

  // Getters for form controls
  get email() { return this.customerForm.get('email'); }
  get firstName() { return this.customerForm.get('firstName'); }
  get lastName() { return this.customerForm.get('lastName'); }
  get phoneNumber() { return this.customerForm.get('phoneNumber'); }
  get role() { return this.customerForm.get('role'); }
  get isActive() { return this.customerForm.get('isActive'); }
  get password() { return this.customerForm.get('password'); }
}

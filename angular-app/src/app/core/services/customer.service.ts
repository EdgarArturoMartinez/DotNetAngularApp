import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CustomerDto, UserRole } from '../models/auth.models';

export interface CustomerAdminDto {
  email: string;
  firstName: string;
  lastName: string;
  phoneNumber?: string;
  role: UserRole;
  isActive: boolean;
}

export interface CreateCustomerRequest {
  customer: CustomerAdminDto;
  password: string;
}

@Injectable({
  providedIn: 'root'
})
export class CustomerService {
  private apiUrl = `${environment.apiURL}/api/customers`;

  constructor(private http: HttpClient) {}

  /**
   * Get all customers (Admin only)
   */
  getAllCustomers(): Observable<CustomerDto[]> {
    return this.http.get<CustomerDto[]>(this.apiUrl);
  }

  /**
   * Get customer by ID (Admin only)
   */
  getCustomerById(id: number): Observable<CustomerDto> {
    return this.http.get<CustomerDto>(`${this.apiUrl}/${id}`);
  }

  /**
   * Get customers by role (Admin only)
   */
  getCustomersByRole(role: UserRole): Observable<CustomerDto[]> {
    return this.http.get<CustomerDto[]>(`${this.apiUrl}/role/${role}`);
  }

  /**
   * Create new customer (Admin only)
   */
  createCustomer(request: CreateCustomerRequest): Observable<CustomerDto> {
    return this.http.post<CustomerDto>(this.apiUrl, request);
  }

  /**
   * Update customer (Admin only)
   */
  updateCustomer(id: number, customer: CustomerAdminDto): Observable<{ message: string }> {
    return this.http.put<{ message: string }>(`${this.apiUrl}/${id}`, customer);
  }

  /**
   * Delete customer (Admin only)
   */
  deleteCustomer(id: number): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(`${this.apiUrl}/${id}`);
  }
}

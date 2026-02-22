// Auth DTOs matching backend models
export interface CustomerRegisterDto {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  phoneNumber?: string;
}

export interface CustomerLoginDto {
  email: string;
  password: string;
}

export interface AuthResponseDto {
  token: string;
  customer: CustomerDto;
}

export interface CustomerDto {
  id: number;
  email: string;
  firstName: string;
  lastName: string;
  phoneNumber?: string;
  role: UserRole;
  streetAddress?: string;
  city?: string;
  stateProvince?: string;
  postalCode?: string;
  country?: string;
  isActive: boolean;
  emailConfirmed: boolean;
  createdAt: Date;
}

export interface CustomerUpdateDto {
  firstName: string;
  lastName: string;
  phoneNumber?: string;
  streetAddress?: string;
  city?: string;
  stateProvince?: string;
  postalCode?: string;
  country?: string;
}

export interface ChangePasswordDto {
  currentPassword: string;
  newPassword: string;
}

export interface ForgotPasswordDto {
  email: string;
}

export interface ResetPasswordDto {
  email: string;
  token: string;
  newPassword: string;
}

export enum UserRole {
  Customer = 0,
  Admin = 1
}

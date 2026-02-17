import { IEntity } from '../interfaces/entity.interface';

/**
 * VegCategory Entity
 * Represents a product category in the system
 */
export interface VegCategory extends IEntity {
  id?: number;
  idCategory?: number;
  categoryName: string;
  description?: string;
  createdAt?: string;
  updatedAt?: string;
  productCount?: number;
}

/**
 * VegProduct Entity
 * Represents a product in the system
 */
export interface VegProduct extends IEntity {
  id?: number;
  idVegproduct?: number;
  name: string;
  price: number;
  description?: string;
  idCategory?: number;
  vegCategory?: VegCategory;
  createdAt?: string;
  updatedAt?: string;
}

/**
 * DTO for creating/updating products
 * Used in forms to avoid direct manipulation of the full entity
 */
export interface VegProductCreateUpdateDto {
  name: string;
  price: number;
  description?: string;
  idCategory?: number | null;
}

/**
 * DTO for creating/updating categories
 */
export interface VegCategoryCreateUpdateDto {
  categoryName: string;
  description?: string;
}

/**
 * Paginated response wrapper
 * Use this for any endpoint that returns paginated data
 */
export interface PaginatedResponse<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}

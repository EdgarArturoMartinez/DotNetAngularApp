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
  stockQuantity?: number;
  netWeight?: number;
  idCategory?: number;
  idTypeWeight?: number;
  vegCategory?: VegCategoryBasic;
  vegTypeWeight?: VegTypeWeightBasic;
  createdAt?: string;
  updatedAt?: string;
}

/**
 * Basic VegCategory for nested references
 */
export interface VegCategoryBasic {
  idCategory: number;
  categoryName: string;
  description?: string;
}

/**
 * VegTypeWeight Entity
 * Represents a weight/measure type in the system
 */
export interface VegTypeWeight extends IEntity {
  idTypeWeight: number;
  name: string;
  abbreviationWeight: string;
  description?: string;
  isActive: boolean;
  createdAt?: string;
}

/**
 * Basic VegTypeWeight for nested references and dropdowns
 */
export interface VegTypeWeightBasic {
  idTypeWeight: number;
  name: string;
  abbreviationWeight: string;
}

/**
 * DTO for creating/updating products
 * Used in forms to avoid direct manipulation of the full entity
 */
export interface VegProductCreateUpdateDto {
  name: string;
  price: number;
  description?: string;
  stockQuantity?: number;
  netWeight?: number;
  idCategory?: number | null;
  idTypeWeight?: number | null;
}

/**
 * DTO for creating/updating categories
 */
export interface VegCategoryCreateUpdateDto {
  categoryName: string;
  description?: string;
}

/**
 * DTO for creating/updating weight types
 */
export interface VegTypeWeightCreateUpdateDto {
  name: string;
  abbreviationWeight: string;
  description?: string;
  isActive: boolean;
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

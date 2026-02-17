/**
 * Base interface for all entities in the application
 * Every entity must implement this interface to ensure consistency
 */
export interface IEntity {
  id?: number | string;
}

/**
 * Generic CRUD Service interface
 * Implement this interface for any entity service
 */
export interface ICrudService<T extends IEntity> {
  getAll(): any;
  getById(id: number | string): any;
  create(data: Partial<T>): any;
  update(id: number | string, data: Partial<T>): any;
  delete(id: number | string): any;
}

/**
 * API Response wrapper for consistent error handling
 */
export interface ApiResponse<T> {
  success: boolean;
  data?: T;
  error?: string;
  message?: string;
}

import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { IEntity, ICrudService } from '../interfaces/entity.interface';

/**
 * Abstract base service for all CRUD operations
 * Extend this service for any entity to avoid code repetition
 * 
 * Usage:
 * @Injectable({ providedIn: 'root' })
 * export class ProductService extends BaseCrudService<Product> {
 *   constructor(http: HttpClient) {
 *     super(http, 'https://localhost:7020/api/products');
 *   }
 * }
 */
export abstract class BaseCrudService<T extends IEntity> implements ICrudService<T> {
  protected http: HttpClient;
  protected apiUrl: string;

  constructor(http: HttpClient, apiUrl: string) {
    this.http = http;
    this.apiUrl = apiUrl;
  }

  /**
   * Get all entities
   */
  public getAll(): Observable<T[]> {
    console.log(`[${this.getEntityName()}] Fetching all items from ${this.apiUrl}`);
    return this.http.get<T[]>(this.apiUrl);
  }

  /**
   * Get entity by ID
   */
  public getById(id: number | string): Observable<T> {
    const url = `${this.apiUrl}/${id}`;
    console.log(`[${this.getEntityName()}] Fetching by ID: ${id}`);
    return this.http.get<T>(url);
  }

  /**
   * Create new entity
   */
  public create(data: Partial<T>): Observable<T> {
    console.log(`[${this.getEntityName()}] Creating:`, data);
    return this.http.post<T>(this.apiUrl, data);
  }

  /**
   * Update entity
   */
  public update(id: number | string, data: Partial<T>): Observable<void> {
    const url = `${this.apiUrl}/${id}`;
    console.log(`[${this.getEntityName()}] Updating ID ${id}:`, data);
    return this.http.put<void>(url, data);
  }

  /**
   * Delete entity
   */
  public delete(id: number | string): Observable<void> {
    const url = `${this.apiUrl}/${id}`;
    console.log(`[${this.getEntityName()}] Deleting ID: ${id}`);
    return this.http.delete<void>(url);
  }

  /**
   * Get service name for logging purposes
   * Override in child services if needed
   */
  protected getEntityName(): string {
    return this.constructor.name;
  }
}

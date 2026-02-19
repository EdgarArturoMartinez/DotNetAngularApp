import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { VegTypeWeight, VegTypeWeightBasic, VegTypeWeightCreateUpdateDto } from '../models/entities';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class VegTypeWeightService {
  private apiUrl = `${environment.apiURL}/api/vegtypeweights`;
  
  constructor(private http: HttpClient) { }
  
  /**
   * Get all weight types
   */
  getAll(): Observable<VegTypeWeight[]> {
    return this.http.get<VegTypeWeight[]>(this.apiUrl);
  }
  
  /**
   * Get only active weight types for dropdowns
   */
  getActiveTypes(): Observable<VegTypeWeightBasic[]> {
    return this.http.get<VegTypeWeightBasic[]>(`${this.apiUrl}/active`);
  }
  
  /**
   * Get weight type by ID
   */
  getById(id: number): Observable<VegTypeWeight> {
    return this.http.get<VegTypeWeight>(`${this.apiUrl}/${id}`);
  }
  
  /**
   * Create a new weight type
   */
  create(data: VegTypeWeightCreateUpdateDto): Observable<VegTypeWeight> {
    return this.http.post<VegTypeWeight>(this.apiUrl, data);
  }
  
  /**
   * Update an existing weight type
   */
  update(id: number, data: VegTypeWeightCreateUpdateDto): Observable<VegTypeWeight> {
    return this.http.put<VegTypeWeight>(`${this.apiUrl}/${id}`, data);
  }
  
  /**
   * Delete a weight type
   */
  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}

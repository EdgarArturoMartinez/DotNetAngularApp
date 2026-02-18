import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { VegProduct, VegProductCreateUpdateDto } from '../../../shared/models/entities';
import { environment } from '../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private apiUrl = `${environment.apiURL}/api/vegproducts`;
  
  constructor(private http: HttpClient) { }
  
  getAll(): Observable<VegProduct[]> {
    return this.http.get<VegProduct[]>(this.apiUrl);
  }
  
  getById(id: number): Observable<VegProduct> {
    return this.http.get<VegProduct>(`${this.apiUrl}/${id}`);
  }
  
  create(data: VegProductCreateUpdateDto): Observable<VegProduct> {
    return this.http.post<VegProduct>(this.apiUrl, data);
  }
  
  update(id: number, data: VegProductCreateUpdateDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, data);
  }
  
  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getByCategory(categoryId: number): Observable<VegProduct[]> {
    return this.http.get<VegProduct[]>(`${this.apiUrl}/category/${categoryId}`);
  }
}

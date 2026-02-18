import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { VegCategory, VegCategoryCreateUpdateDto } from '../../../shared/models/entities';
import { environment } from '../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {
  private apiUrl = `${environment.apiURL}/api/vegcategories`;
  
  constructor(private http: HttpClient) { }
  
  getAll(): Observable<VegCategory[]> {
    return this.http.get<VegCategory[]>(this.apiUrl);
  }
  
  getById(id: number): Observable<VegCategory> {
    return this.http.get<VegCategory>(`${this.apiUrl}/${id}`);
  }
  
  create(data: VegCategoryCreateUpdateDto): Observable<VegCategory> {
    return this.http.post<VegCategory>(this.apiUrl, data);
  }
  
  update(id: number, data: VegCategoryCreateUpdateDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, data);
  }
  
  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { VegCategory } from './vegcategory';
import { environment } from '../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class VegCategoryService {
  private apiUrl = `${environment.apiURL}/api/vegcategories`;
  
  constructor(private http: HttpClient) { }
  
  getVegcategories(): Observable<VegCategory[]> {
    return this.http.get<VegCategory[]>(this.apiUrl);
  }
  
  getVegcategoryById(id: number): Observable<VegCategory> {
    const url = `${this.apiUrl}/${id}`;
    console.log('VegCategoryService.getVegcategoryById - URL:', url);
    console.log('VegCategoryService.getVegcategoryById - ID:', id);
    return this.http.get<VegCategory>(url);
  }
  
  createVegcategory(data: VegCategory): Observable<VegCategory> {
    return this.http.post<VegCategory>(this.apiUrl, data);
  }
  
  updateVegcategory(id: number, data: VegCategory): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, data);
  }
  
  deleteVegcategory(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
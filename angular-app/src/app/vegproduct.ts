import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { VegProductCreation } from './vegproduct.models';
import { environment } from '../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class Vegproduct {
  constructor(){}

  private http = inject(HttpClient);
  private apiUrl = `${environment.apiURL}/api/vegproducts`; 

  public createVegproduct(vegProduct: VegProductCreation) {
    return this.http.post(this.apiUrl, vegProduct);
  }

  public getVegproducts() {
    return this.http.get<any[]>(this.apiUrl);
  }

  public deleteVegproduct(id: number) {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }

  public updateVegproduct(id: number, vegProduct: VegProductCreation) {
    return this.http.put(`${this.apiUrl}/${id}`, vegProduct);
  }
}
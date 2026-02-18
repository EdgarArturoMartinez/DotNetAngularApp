import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';

import { Vegproduct, VegProduct } from './vegproduct';
import { VegProductCreation } from './vegproduct.models';
import { environment } from '../environments/environment';

describe('Vegproduct', () => {
  let service: Vegproduct;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [Vegproduct]
    });
    service = TestBed.inject(Vegproduct);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should create product with description via POST', () => {
    const testProduct: VegProductCreation = {
      name: 'Test Product',
      price: 10.99,
      description: 'This is a test description that should be saved',
      stockQuantity: 50,
      idCategory: 1
    };

    service.createVegproduct(testProduct).subscribe(response => {
      expect(response).toBeTruthy();
    });

    const req = httpMock.expectOne(`${environment.apiURL}/api/vegproducts`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body.description).toBe('This is a test description that should be saved');
    expect(req.request.body.name).toBe('Test Product');
    expect(req.request.body.price).toBe(10.99);
    
    req.flush({ id: 1, ...testProduct });
  });

  it('should update product with description via PUT', () => {
    const productId = 1;
    const testProduct: VegProductCreation = {
      name: 'Updated Product',
      price: 15.99,
      description: 'Updated description text',
      stockQuantity: 75,
      idCategory: 2
    };

    service.updateVegproduct(productId, testProduct).subscribe(response => {
      expect(response).toBeTruthy();
    });

    const req = httpMock.expectOne(`${environment.apiURL}/api/vegproducts/${productId}`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body.description).toBe('Updated description text');
    expect(req.request.body.name).toBe('Updated Product');
    
    req.flush({ id: productId, ...testProduct });
  });

  it('should handle empty description when creating product', () => {
    const testProduct: VegProductCreation = {
      name: 'Test Product',
      price: 10.99,
      description: '',
      stockQuantity: 50
    };

    service.createVegproduct(testProduct).subscribe();

    const req = httpMock.expectOne(`${environment.apiURL}/api/vegproducts`);
    expect(req.request.body.description).toBe('');
    
    req.flush({ id: 1, ...testProduct });
  });

  it('should handle undefined description when creating product', () => {
    const testProduct: VegProductCreation = {
      name: 'Test Product',
      price: 10.99,
      stockQuantity: 50
      // description is undefined
    };

    service.createVegproduct(testProduct).subscribe();

    const req = httpMock.expectOne(`${environment.apiURL}/api/vegproducts`);
    // undefined should be preserved or handled by the backend
    expect(req.request.body.description).toBeUndefined();
    
    req.flush({ id: 1, ...testProduct });
  });

  it('should retrieve product with description', () => {
    const mockProduct: VegProduct = {
      id: 1,
      name: 'Test Product',
      price: 10.99,
      description: 'Retrieved description',
      stockQuantity: 50,
      idCategory: 1
    };

    service.getVegproductById(1).subscribe(product => {
      expect(product.description).toBe('Retrieved description');
      expect(product.name).toBe('Test Product');
    });

    const req = httpMock.expectOne(`${environment.apiURL}/api/vegproducts/1`);
    expect(req.request.method).toBe('GET');
    
    req.flush(mockProduct);
  });
});

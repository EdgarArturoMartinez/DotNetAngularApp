import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { provideRouter } from '@angular/router';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { vi } from 'vitest';
import { of } from 'rxjs';

import { CreateVegproduct } from './create-vegproduct';
import { Vegproduct } from '../vegproduct';
import { VegCategoryService } from '../vegcategory.service';
import { environment } from '../../environments/environment';

describe('CreateVegproduct', () => {
  let component: CreateVegproduct;
  let fixture: ComponentFixture<CreateVegproduct>;
  let httpMock: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        CreateVegproduct,
        HttpClientTestingModule,
        BrowserAnimationsModule
      ],
      providers: [
        Vegproduct,
        VegCategoryService,
        provideRouter([])
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateVegproduct);
    component = fixture.componentInstance;
    httpMock = TestBed.inject(HttpTestingController);
    await fixture.whenStable();
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have description field in the form', () => {
    expect(component.vegProductForm.get('description')).toBeTruthy();
  });

  it('should include description when creating a product', () => {
    const testProduct = {
      name: 'Test Product',
      price: '10.99',
      description: 'This is a test description',
      stockQuantity: 50,
      idCategory: null
    };

    component.vegProductForm.patchValue(testProduct);

    const createSpy = vi.spyOn(component.vegProduct, 'createVegproduct').mockImplementation((data: any) => {
      // Verify description is included in the request
      expect(data.description).toBe('This is a test description');
      expect(data.name).toBe('Test Product');
      expect(data.price).toBe(10.99);
      expect(data.stockQuantity).toBe(50);
      
      return of({ id: 1, ...data });
    });

    component.saveChanges();
    expect(createSpy).toHaveBeenCalled();
  });

  it('should send empty string for description when not provided', () => {
    const testProduct = {
      name: 'Test Product',
      price: '10.99',
      description: '',
      stockQuantity: 50,
      idCategory: null
    };

    component.vegProductForm.patchValue(testProduct);

    const createSpy = vi.spyOn(component.vegProduct, 'createVegproduct').mockImplementation((data: any) => {
      // Verify description is included even when empty
      expect(data.description).toBe('');
      
      return of({ id: 1, ...data });
    });

    component.saveChanges();
    expect(createSpy).toHaveBeenCalled();
  });

  it('should validate required fields but allow optional description', () => {
    // Set only required fields
    component.vegProductForm.patchValue({
      name: 'Test',
      price: '5.00',
      stockQuantity: 10,
      description: '' // Optional field
    });

    expect(component.vegProductForm.valid).toBeTruthy();
  });
});

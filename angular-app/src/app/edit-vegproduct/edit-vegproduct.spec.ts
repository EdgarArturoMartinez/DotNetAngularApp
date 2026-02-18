import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { provideRouter } from '@angular/router';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';
import { vi } from 'vitest';

import { EditVegproduct } from './edit-vegproduct';
import { Vegproduct } from '../vegproduct';
import { VegCategoryService } from '../vegcategory.service';

describe('EditVegproduct', () => {
  let component: EditVegproduct;
  let fixture: ComponentFixture<EditVegproduct>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        EditVegproduct,
        HttpClientTestingModule,
        BrowserAnimationsModule
      ],
      providers: [
        Vegproduct,
        VegCategoryService,
        provideRouter([]),
        {
          provide: ActivatedRoute,
          useValue: {
            paramMap: of(new Map([['id', '1']]))
          }
        }
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditVegproduct);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have description field in the form', () => {
    expect(component.vegProductForm.get('description')).toBeTruthy();
  });

  it('should load product with description and populate form', async () => {
    const mockProduct = {
      id: 1,
      name: 'Test Product',
      price: 10.99,
      description: 'Original description',
      stockQuantity: 50,
      idCategory: 1
    };

    vi.spyOn(component.vegProduct, 'getVegproductById').mockReturnValue(
      of(mockProduct)
    );

    component.productId = 1;
    component.loadProduct();

    await new Promise(resolve => setTimeout(resolve, 100));
    
    expect(component.vegProductForm.get('description')?.value).toBe('Original description');
  });

  it('should include description when updating a product', () => {
    const testProduct = {
      name: 'Updated Product',
      price: '15.99',
      description: 'Updated description text',
      stockQuantity: 75,
      idCategory: 2
    };

    component.productId = 1;
    component.vegProductForm.patchValue(testProduct);

    const updateSpy = vi.spyOn(component.vegProduct, 'updateVegproduct').mockImplementation((id: number, data: any) => {
      // Verify description is included and not empty string
      expect(data.description).toBe('Updated description text');
      expect(data.name).toBe('Updated Product');
      expect(data.price).toBe(15.99);
      expect(data.stockQuantity).toBe(75);
      expect(id).toBe(1);
      
      return of({ id, ...data });
    });

    component.saveChanges();
    expect(updateSpy).toHaveBeenCalled();
  });

  it('should preserve empty description when updating', () => {
    const testProduct = {
      name: 'Product Without Description',
      price: '5.99',
      description: '',
      stockQuantity: 10,
      idCategory: null
    };

    component.productId = 1;
    component.vegProductForm.patchValue(testProduct);

    const updateSpy = vi.spyOn(component.vegProduct, 'updateVegproduct').mockImplementation((id: number, data: any) => {
      // Verify description is empty string, not hardcoded
      expect(data.description).toBe('');
      
      return of({ id, ...data });
    });

    component.saveChanges();
    expect(updateSpy).toHaveBeenCalled();
  });

  it('should handle undefined description gracefully', async () => {
    const mockProduct = {
      id: 1,
      name: 'Test Product',
      price: 10.99,
      stockQuantity: 50,
      idCategory: 1
      // description is undefined
    };

    vi.spyOn(component.vegProduct, 'getVegproductById').mockReturnValue(
      of(mockProduct)
    );

    component.productId = 1;
    component.loadProduct();

    await new Promise(resolve => setTimeout(resolve, 100));
    
    // Should default to empty string
    expect(component.vegProductForm.get('description')?.value).toBe('');
  });
});

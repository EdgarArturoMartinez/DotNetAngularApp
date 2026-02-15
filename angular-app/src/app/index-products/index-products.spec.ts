import { ComponentFixture, TestBed } from '@angular/core/testing';

import { IndexProducts } from './index-products';

describe('IndexProducts', () => {
  let component: IndexProducts;
  let fixture: ComponentFixture<IndexProducts>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [IndexProducts]
    })
    .compileComponents();

    fixture = TestBed.createComponent(IndexProducts);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

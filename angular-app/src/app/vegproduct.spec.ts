import { TestBed } from '@angular/core/testing';

import { Vegproduct } from './vegproduct';

describe('Vegproduct', () => {
  let service: Vegproduct;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Vegproduct);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

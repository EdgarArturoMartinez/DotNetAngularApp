import { TestBed } from '@angular/core/testing';

import { VegCategoryService } from './vegcategory.service';

describe('VegCategoryService', () => {
  let service: VegCategoryService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(VegCategoryService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

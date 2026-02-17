import { ComponentFixture, TestBed } from '@angular/core/testing';

import { IndexVegcategories } from './index-vegcategories';

describe('IndexVegcategories', () => {
  let component: IndexVegcategories;
  let fixture: ComponentFixture<IndexVegcategories>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [IndexVegcategories]
    })
    .compileComponents();

    fixture = TestBed.createComponent(IndexVegcategories);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

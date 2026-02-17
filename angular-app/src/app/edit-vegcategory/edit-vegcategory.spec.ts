import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditVegcategory } from './edit-vegcategory';

describe('EditVegcategory', () => {
  let component: EditVegcategory;
  let fixture: ComponentFixture<EditVegcategory>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditVegcategory]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditVegcategory);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

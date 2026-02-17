import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateVegcategory } from './create-vegcategory';

describe('CreateVegcategory', () => {
  let component: CreateVegcategory;
  let fixture: ComponentFixture<CreateVegcategory>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateVegcategory]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateVegcategory);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

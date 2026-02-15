import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateVegproduct } from './create-vegproduct';

describe('CreateVegproduct', () => {
  let component: CreateVegproduct;
  let fixture: ComponentFixture<CreateVegproduct>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateVegproduct]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateVegproduct);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

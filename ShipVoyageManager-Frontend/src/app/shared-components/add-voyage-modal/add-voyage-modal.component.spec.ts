import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddVoyageModalComponent } from './add-voyage-modal.component';

describe('AddVoyageModalComponent', () => {
  let component: AddVoyageModalComponent;
  let fixture: ComponentFixture<AddVoyageModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddVoyageModalComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddVoyageModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

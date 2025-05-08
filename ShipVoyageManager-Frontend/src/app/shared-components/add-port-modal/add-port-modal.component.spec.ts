import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddPortModalComponent } from './add-port-modal.component';

describe('AddPortModalComponent', () => {
  let component: AddPortModalComponent;
  let fixture: ComponentFixture<AddPortModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddPortModalComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddPortModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditPortModalComponent } from './edit-port-modal.component';

describe('EditPortModalComponent', () => {
  let component: EditPortModalComponent;
  let fixture: ComponentFixture<EditPortModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditPortModalComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditPortModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

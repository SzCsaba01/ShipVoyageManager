import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditVoyageModalComponent } from './edit-voyage-modal.component';

describe('EditVoyageModalComponent', () => {
  let component: EditVoyageModalComponent;
  let fixture: ComponentFixture<EditVoyageModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditVoyageModalComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditVoyageModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

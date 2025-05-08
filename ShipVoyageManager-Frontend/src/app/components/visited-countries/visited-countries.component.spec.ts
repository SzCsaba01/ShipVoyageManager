import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VisitedCountriesComponent } from './visited-countries.component';

describe('VisitedCountriesComponent', () => {
  let component: VisitedCountriesComponent;
  let fixture: ComponentFixture<VisitedCountriesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [VisitedCountriesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(VisitedCountriesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

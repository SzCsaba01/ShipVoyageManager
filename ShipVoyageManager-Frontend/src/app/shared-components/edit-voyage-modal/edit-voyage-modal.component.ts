import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { SelfUnsubscriberBase } from '../../utils/SelfUnsubscribeBase';
import { CommonModule } from '@angular/common';
import { formModulesUtil } from '../../shared-modules/form-module.util';
import { angularMaterialModulesUtil } from '../../shared-modules/angular-material-modules.util';
import { IVoyage } from '../../models/voyage/voyage.model';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { IPort } from '../../models/port/port.model';
import { IShip } from '../../models/ship/ship.model';
import { LoadingService } from '../../services/loading.service';
import { PortService } from '../../services/port.service';
import { ShipService } from '../../services/ship.service';
import { VoyageService } from '../../services/voyage.service';
import { takeUntil } from 'rxjs';
import { IGetShipsOutOfDateRange } from '../../models/ship/getShipsOutOfDateRange.model';
import { Store } from '@ngrx/store';
import { userRoles } from '../../constants/user-roles';
import { selectUserRole } from '../../state/authentication/auth.selector';
import { provideNativeDateAdapter } from '@angular/material/core';

@Component({
  selector: 'app-edit-voyage-modal',
  standalone: true,
  providers: [provideNativeDateAdapter()],
  imports: [CommonModule, formModulesUtil(), angularMaterialModulesUtil()],
  templateUrl: './edit-voyage-modal.component.html',
  styleUrl: './edit-voyage-modal.component.scss'
})
export class EditVoyageModalComponent extends SelfUnsubscriberBase implements OnInit {
  @Input() voyage: IVoyage = {} as IVoyage;
  @Output() onEditVoyage: EventEmitter<void> = new EventEmitter<void>();
  @Output() onCloseModal: EventEmitter<void> = new EventEmitter<void>();

  ships: IShip[] = [];
  ports: IPort[] = [];

  voyageForm: FormGroup = {} as FormGroup;
  
  startTime: FormControl = {} as FormControl;
  endTime: FormControl = {} as FormControl;
  shipId: FormControl = {} as FormControl;
  arrivalPortId: FormControl = {} as FormControl;
  departurePortId: FormControl = {} as FormControl;

  isAdmin = false;

  constructor(
    private shipService: ShipService,
    private loadingService: LoadingService,
    private portService: PortService,
    private voyageService: VoyageService,
    private formBuilder: FormBuilder,
    private store: Store,
  ) {
    super();
  }


  ngOnInit(): void {
    this.store.select(selectUserRole).subscribe(role => {
        this.isAdmin = role === userRoles.admin;
        this.initializeForm();
        this.loadData();
      });
  }

  initializeForm(): void {
    this.startTime = new FormControl({ value: this.voyage.startTime, disabled: !this.isAdmin }, []);
    this.endTime = new FormControl({ value: this.voyage.endTime, disabled: !this.isAdmin }, []);
    this.shipId = new FormControl({ value: this.voyage.shipId, disabled: !this.isAdmin }, []);
    this.arrivalPortId = new FormControl({ value: this.voyage.arrivalPortId, disabled: !this.isAdmin }, []);
    this.departurePortId = new FormControl({ value: this.voyage.departurePortId, disabled: !this.isAdmin }, []);

    this.voyageForm = this.formBuilder.group({
      startTime: this.startTime,
      endTime: this.endTime,
      shipId: this.shipId,
      arrivalPortId: this.arrivalPortId,
      departurePortId: this.departurePortId,
    }, { validators: [this.samePortValidator()] });

    this.startTime.valueChanges
    .pipe(takeUntil(this.ngUnsubscribe))
    .subscribe(() => this.checkDatesAndEnableShip());

    this.endTime.valueChanges
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(() => this.checkDatesAndEnableShip());
  }

  private checkDatesAndEnableShip(): void {
    const start = this.startTime.value;
    const end = this.endTime.value;
  
    if (start && end) {
      this.shipId.enable();
      this.loadShips();
    } else {
      this.shipId.disable();
      this.ships = [];
    }
  }  

  private samePortValidator() {
    return (form: FormGroup) => {
      const arrival = form.get('arrivalPort')?.value;
      const departure = form.get('departurePort')?.value;
      return arrival && departure && arrival === departure
        ? { samePort: true }
        : null;
    };
  }  

  private loadData(): void {
    this.portService.getAllPorts()
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe((ports: IPort[]) => {
        this.ports = ports;
      });
    this.loadShips();
  }

  loadShips(): void {
    if (this.startTime.value && this.endTime.value) {
      const requestBody = {
        startDate: this.startTime.value,
        endDate: this.endTime.value
      } as IGetShipsOutOfDateRange;
      this.shipService.getShipsOutOfDateRange(requestBody)
        .pipe(takeUntil(this.ngUnsubscribe))
        .subscribe((ships: IShip[]) => {
          this.ships = ships;

          const currentShip = {
            id: this.voyage.shipId,
            name: this.voyage.shipName,
          } as IShip;
  
          this.ships.push(currentShip);
        });
    }
  }

  onSave(voyageData: IVoyage): void {
    if (this.voyageForm.valid && this.isAdmin) {
      this.loadingService.show();
      voyageData.id = this.voyage.id;
      this.voyageService.updateVoyage(voyageData)
        .pipe(takeUntil(this.ngUnsubscribe))
        .subscribe(() => {
          this.onEditVoyage.emit();
          this.loadingService.hide();
        });
    }
  }

  onClose(): void {
    this.onCloseModal.emit();
  }

}

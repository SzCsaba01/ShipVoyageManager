import { CommonModule } from '@angular/common';
import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { angularMaterialModulesUtil } from '../../shared-modules/angular-material-modules.util';
import { formModulesUtil } from '../../shared-modules/form-module.util';
import { SelfUnsubscriberBase } from '../../utils/SelfUnsubscribeBase';
import { LoadingService } from '../../services/loading.service';
import { PortService } from '../../services/port.service';
import { ShipService } from '../../services/ship.service';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { IShip } from '../../models/ship/ship.model';
import { IPort } from '../../models/port/port.model';
import { IGetShipsOutOfDateRange } from '../../models/ship/getShipsOutOfDateRange.model';
import { takeUntil } from 'rxjs';
import { VoyageService } from '../../services/voyage.service';
import { provideNativeDateAdapter } from '@angular/material/core';
import { IVoyage } from '../../models/voyage/voyage.model';

@Component({
  selector: 'app-add-voyage-modal',
  standalone: true,
  providers: [provideNativeDateAdapter()],
  imports: [CommonModule, formModulesUtil(), angularMaterialModulesUtil()],
  templateUrl: './add-voyage-modal.component.html',
  styleUrl: './add-voyage-modal.component.scss'
})
export class AddVoyageModalComponent extends SelfUnsubscriberBase implements OnInit {
  @Output() onAddVoyage: EventEmitter<void> = new EventEmitter<void>();
  @Output() onCloseModal: EventEmitter<void> = new EventEmitter<void>();

  addVoyageForm: FormGroup = {} as FormGroup;

  ships: IShip[] = [];
  ports: IPort[] = [];

  startTime: FormControl = {} as FormControl;
  endTime: FormControl = {} as FormControl;
  shipId: FormControl = {} as FormControl;
  arrivalPortId: FormControl = {} as FormControl;
  departurePortId: FormControl = {} as FormControl;

  constructor(
    private shipService: ShipService,
    private loadingService: LoadingService,
    private portService: PortService,
    private voyageService: VoyageService,
    private formBuilder: FormBuilder,
  ) {
    super();
  }

  ngOnInit(): void {
      this.initializeForm();
      this.loadData();
  }

  private initializeForm(): void {
    this.startTime = new FormControl('', []);
    this.endTime = new FormControl('', []);
    this.shipId = new FormControl({value:'', disabled: true}, []);
    this.arrivalPortId = new FormControl('', []);
    this.departurePortId = new FormControl('', []);

    this.addVoyageForm = this.formBuilder.group({
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
    this.loadingService.show();
    this.portService.getAllPorts()
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe((ports: IPort[]) => {
        this.ports = ports;
        this.loadingService.hide();
      });
  }

  loadShips(): void {
    const requestBody = {
      startDate: this.startTime.value,
      endDate: this.endTime.value
    } as IGetShipsOutOfDateRange;
    this.shipService.getShipsOutOfDateRange(requestBody)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe((ships: IShip[]) => {
        this.ships = ships;
      });
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

  onSubmit(voyageData: IVoyage): void {
    this.loadingService.show();
    this.voyageService.addVoyage(voyageData)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(() => {
        this.loadingService.hide();
        this.onAddVoyage.emit();
      });
  }

  onClose(): void {
    this.onCloseModal.emit();
  }
}

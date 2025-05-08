import { CommonModule } from '@angular/common';
import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { formModulesUtil } from '../../shared-modules/form-module.util';
import { angularMaterialModulesUtil } from '../../shared-modules/angular-material-modules.util';
import { SelfUnsubscriberBase } from '../../utils/SelfUnsubscribeBase';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { LoadingService } from '../../services/loading.service';
import { ShipService } from '../../services/ship.service';
import { takeUntil } from 'rxjs';

@Component({
  selector: 'app-add-ship-modal',
  standalone: true,
  imports: [CommonModule, formModulesUtil(), angularMaterialModulesUtil()],
  templateUrl: './add-ship-modal.component.html',
  styleUrl: './add-ship-modal.component.scss'
})
export class AddShipModalComponent extends SelfUnsubscriberBase implements OnInit {
  @Output() onAddShip: EventEmitter<void> = new EventEmitter<void>();
  @Output() onCloseModal: EventEmitter<void> = new EventEmitter<void>();

  addShipForm: FormGroup = {} as FormGroup;
  
  name: FormControl = {} as FormControl;
  maxSpeed: FormControl = {} as FormControl;

  constructor(
    private formBuilder: FormBuilder,
    private shipService: ShipService,
    private loadingService: LoadingService,
  ) {
    super();
  }
  
  ngOnInit(): void {
    this.initializeForm();
  }

  private initializeForm(): void {
    this.name = new FormControl('', [Validators.required, Validators.maxLength(100)]);
    this.maxSpeed = new FormControl('', [Validators.required, Validators.min(0)]);

    this.addShipForm = this.formBuilder.group({
      name: this.name,
      maxSpeed: this.maxSpeed,
    });
  }

  onSubmit(shipData: any): void {
    this.loadingService.show();
    this.shipService.addShip(shipData)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(() => {
        this.loadingService.hide();
        this.onAddShip.emit();
      });
  }

  onClose(): void {
    this.onCloseModal.emit();
  }
}

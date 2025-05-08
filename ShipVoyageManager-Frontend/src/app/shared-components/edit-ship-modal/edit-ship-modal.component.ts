import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { SelfUnsubscriberBase } from '../../utils/SelfUnsubscribeBase';
import { CommonModule } from '@angular/common';
import { angularMaterialModulesUtil } from '../../shared-modules/angular-material-modules.util';
import { formModulesUtil } from '../../shared-modules/form-module.util';
import { IShip } from '../../models/ship/ship.model';
import { Store } from '@ngrx/store';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { ShipService } from '../../services/ship.service';
import { userRoles } from '../../constants/user-roles';
import { selectUserRole } from '../../state/authentication/auth.selector';
import { takeUntil } from 'rxjs';
import { LoadingService } from '../../services/loading.service';

@Component({
  selector: 'app-edit-ship-modal',
  imports: [CommonModule, formModulesUtil(), angularMaterialModulesUtil()],
  templateUrl: './edit-ship-modal.component.html',
  styleUrl: './edit-ship-modal.component.scss'
})
export class EditShipModalComponent extends SelfUnsubscriberBase implements OnInit {
  @Input() ship: IShip = {} as IShip;
  @Output() onEditShip: EventEmitter<void> = new EventEmitter<void>();
  @Output() onCloseModal: EventEmitter<void> = new EventEmitter<void>();

  shipForm: FormGroup = {} as FormGroup;

  name: FormControl = {} as FormControl;
  maxSpeed: FormControl = {} as FormControl;

  isAdmin = false;

  constructor(
    private shipService: ShipService,
    private loadingService: LoadingService,
    private formBuilder: FormBuilder,
    private store: Store
  ) {
    super();
  }

  ngOnInit(): void {
    this.store.select(selectUserRole).subscribe(role => {
      this.isAdmin = role === userRoles.admin;
      this.initializeForm();
    });
  }

  private initializeForm(): void {
    this.name = new FormControl({ value: this.ship.name, disabled: !this.isAdmin }, []);
    this.maxSpeed = new FormControl({ value: this.ship.maxSpeed, disabled: !this.isAdmin }, []);

    this.shipForm = this.formBuilder.group({
      name: this.name,
      maxSpeed: this.maxSpeed,
    });
  }

  onSave(shipData: IShip): void {
    if (this.shipForm.valid && this.isAdmin) {
      this.loadingService.show();
      shipData.id = this.ship.id;
      this.shipService.updateShip(shipData)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(() => {
        this.onEditShip.emit();
        this.loadingService.hide();
      });
    }
  }

  onClose(): void {
    this.onCloseModal.emit();
  }
}

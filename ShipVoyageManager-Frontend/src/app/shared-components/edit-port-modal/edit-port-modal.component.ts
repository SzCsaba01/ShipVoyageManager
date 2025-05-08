import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { Store } from '@ngrx/store';
import { IPort } from '../../models/port/port.model';
import { selectUserRole } from '../../state/authentication/auth.selector';
import { userRoles } from '../../constants/user-roles';
import { SelfUnsubscriberBase } from '../../utils/SelfUnsubscribeBase';
import { PortService } from '../../services/port.service';
import { takeUntil } from 'rxjs';
import { CommonModule } from '@angular/common';
import { formModulesUtil } from '../../shared-modules/form-module.util';
import { angularMaterialModulesUtil } from '../../shared-modules/angular-material-modules.util';
import { LoadingService } from '../../services/loading.service';
import { provideNativeDateAdapter } from '@angular/material/core';

@Component({
  selector: 'app-edit-port-modal',
  standalone: true,
  providers: [provideNativeDateAdapter()],
  imports: [CommonModule, formModulesUtil(), angularMaterialModulesUtil()],
  templateUrl: './edit-port-modal.component.html',
  styleUrl: './edit-port-modal.component.scss'
})
export class EditPortModalComponent extends SelfUnsubscriberBase  implements OnInit {
  @Input() port: IPort = {} as IPort;
  @Output() onEditPort: EventEmitter<void> = new EventEmitter<void>();
  @Output() onCloseModal: EventEmitter<void> = new EventEmitter<void>();

  portForm: FormGroup = {} as FormGroup;

  name: FormControl = {} as FormControl;
  countryName: FormControl = {} as FormControl;

  isAdmin = false;

  constructor(
    private portService: PortService,
    private formBuilder: FormBuilder, 
    private loadingService: LoadingService,
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
    this.name = new FormControl({value :this.port.name, disabled: !this.isAdmin}, [Validators.required, Validators.maxLength(100)]);
    this.countryName = new FormControl({value:this.port.countryName, disabled: !this.isAdmin}, [Validators.required, Validators.maxLength(100)]);

    this.portForm = this.formBuilder.group({
      name: this.name,
      countryName: this.countryName,
    });
  }

  onSave(portData: IPort): void {
    if (this.portForm.valid && this.isAdmin) {
      this.loadingService.show();
      portData.id = this.port.id;
      this.portService.updatePort(portData)
        .pipe(takeUntil(this.ngUnsubscribe))
        .subscribe(() => {
          this.onEditPort.emit();
          this.loadingService.hide();
        })
    }
  }

  onClose(): void {
    this.onCloseModal.emit();
  }
}

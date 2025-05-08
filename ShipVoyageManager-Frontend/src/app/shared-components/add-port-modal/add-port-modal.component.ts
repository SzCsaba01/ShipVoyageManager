import { CommonModule } from '@angular/common';
import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { formModulesUtil } from '../../shared-modules/form-module.util';
import { angularMaterialModulesUtil } from '../../shared-modules/angular-material-modules.util';
import { SelfUnsubscriberBase } from '../../utils/SelfUnsubscribeBase';
import { LoadingService } from '../../services/loading.service';
import { PortService } from '../../services/port.service';
import { IPort } from '../../models/port/port.model';
import { takeUntil } from 'rxjs';

@Component({
  selector: 'app-add-port-modal',
  standalone: true,
  imports: [CommonModule, formModulesUtil(), angularMaterialModulesUtil()],
  templateUrl: './add-port-modal.component.html',
  styleUrl: './add-port-modal.component.scss'
})
export class AddPortModalComponent extends SelfUnsubscriberBase implements OnInit {
  @Output() onAddPort: EventEmitter<void> = new EventEmitter<void>();
  @Output() onCloseModal: EventEmitter<void> = new EventEmitter<void>();

  addPortForm: FormGroup = {} as FormGroup;

  name: FormControl = {} as FormControl;
  countryName: FormControl = {} as FormControl;

  constructor(
    private formBuilder: FormBuilder,
    private portService: PortService,
    private loadingService: LoadingService,
  ) {
    super();
  }

  ngOnInit(): void {
    this.initializeForm();
  }

  private initializeForm(): void {
    this.name = new FormControl('', [Validators.required, Validators.maxLength(100)]);
    this.countryName = new FormControl('', [Validators.required, Validators.maxLength(100)]);

    this.addPortForm = this.formBuilder.group({
      name: this.name,
      countryName: this.countryName,
    });
  }

  onSubmit(portData: IPort): void {
    this.loadingService.show();
    this.portService.addPort(portData)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(() => {
        this.loadingService.hide();
        this.onAddPort.emit();
      });
  }

  onClose(): void {
    this.onCloseModal.emit();
  }
}

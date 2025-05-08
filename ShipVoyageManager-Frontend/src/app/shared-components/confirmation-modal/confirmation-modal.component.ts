import { Component, EventEmitter, Input, Output } from '@angular/core';
import { angularMaterialModulesUtil } from '../../shared-modules/angular-material-modules.util';

@Component({
  selector: 'app-confirmation-modal',
  imports: [angularMaterialModulesUtil()],
  templateUrl: './confirmation-modal.component.html',
  styleUrl: './confirmation-modal.component.scss'
})
export class ConfirmationModalComponent {
  @Input() title: string = 'Confirmation';
  @Input() message: string = 'Are you sure you want to proceed?';

  @Output() confirm: EventEmitter<void> = new EventEmitter<void>();
  @Output() cancel: EventEmitter<void> = new EventEmitter<void>();

  onConfirm() {
    this.confirm.emit();
  }

  onCancel() {
    this.cancel.emit();
  }
}

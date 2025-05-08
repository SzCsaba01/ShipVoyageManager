import { Component } from '@angular/core';
import { SelfUnsubscriberBase } from '../../utils/SelfUnsubscribeBase';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { angularMaterialModulesUtil } from '../../shared-modules/angular-material-modules.util';
import { formModulesUtil } from '../../shared-modules/form-module.util';
import { takeUntil } from 'rxjs';
import { LoadingService } from '../../services/loading.service';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-forgot-password',
  imports: [
    CommonModule,
    RouterModule,
    formModulesUtil(),
    angularMaterialModulesUtil(),
  ],
  templateUrl: './forgot-password.component.html',
  styleUrl: './forgot-password.component.scss'
})
export class ForgotPasswordComponent extends SelfUnsubscriberBase {
  emailFormGroup: FormGroup = {} as FormGroup;
  email = {} as FormControl;
  
  constructor(
    private userService: UserService,
    private loadingService: LoadingService,
    private router: Router
  ) {
    super();
  }

  ngOnInit(): void {
    this.initializeForm();
  }

  private initializeForm(): void {
    this.email = new FormControl('', [
      Validators.required,
      Validators.email,
      Validators.maxLength(50),
    ]);
    this.emailFormGroup = new FormGroup({
      email: this.email,
    });
  }

  sendResetPasswordEmail(email: string): void {
    this.loadingService.show();
    this.userService
      .sendResetPasswordEmail(email)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(() => {
        this.loadingService.hide();
        this.router.navigate(['/login']);
      });
  }

  onBackButtonClicked(): void {
    this.router.navigate(['/login']);
  }
}

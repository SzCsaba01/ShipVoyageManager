import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { formModulesUtil } from '../../shared-modules/form-module.util';
import { SelfUnsubscriberBase } from '../../utils/SelfUnsubscribeBase';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { takeUntil } from 'rxjs';
import { passwordFormat } from '../../formats/formats';
import { UserService } from '../../services/user.service';
import { IUserChangePassword } from '../../models/user/userChangePassword.model';
import { angularMaterialModulesUtil } from '../../shared-modules/angular-material-modules.util';

@Component({
  selector: 'app-change-password',
  standalone: true,
  imports: [CommonModule, RouterModule, formModulesUtil(), angularMaterialModulesUtil()],
  templateUrl: './change-password.component.html',
  styleUrl: './change-password.component.scss'
})
export class ChangePasswordComponent extends SelfUnsubscriberBase implements OnInit {

  private token: string = '';
  
  changePasswordForm: FormGroup = {} as FormGroup;
  newPassword: FormControl = {} as FormControl;
  repeatNewPassword: FormControl = {} as FormControl;

  constructor(
    private formBuilder: FormBuilder,
    private route: Router,
    private userService: UserService,
    private activatedRoute: ActivatedRoute
  ) {
    super();
  }

  ngOnInit(): void {
    this.token = this.activatedRoute.snapshot.params['token'];
    this.initializeForm();
    this.userService.verifyIfResetPasswordTokenExists(this.token)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(result => {
        if (!result) {
          this.route.navigate(['/page_not_found']);
        }
      });
  }

  changePassword(userChangePassword: IUserChangePassword): void {
    userChangePassword.resetPasswordToken = this.token;

    this.userService.changePassword(userChangePassword)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(() => {
        this.route.navigate(['/login']);
      });
  }

  private initializeForm(): void {
    this.newPassword = new FormControl('', [Validators.required, Validators.minLength(5), Validators.maxLength(30), Validators.pattern(passwordFormat)]);
    this.repeatNewPassword = new FormControl('', [Validators.required, Validators.minLength(5), Validators.maxLength(30), Validators.pattern(passwordFormat)]);

    this.changePasswordForm = this.formBuilder.group({
      newPassword: this.newPassword,
      repeatNewPassword: this.repeatNewPassword
    });
  }

  onBackButtonClicked(): void {
    this.route.navigate(['']);
  }

}

import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { angularMaterialModulesUtil } from '../../shared-modules/angular-material-modules.util';
import { formModulesUtil } from '../../shared-modules/form-module.util';
import { SelfUnsubscriberBase } from '../../utils/SelfUnsubscribeBase';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { UserService } from '../../services/user.service';
import { passwordFormat, usernameFormat } from '../../formats/formats';
import { IUserRegistration } from '../../models/user/userRegistration.model';
import { LoadingService } from '../../services/loading.service';
import { takeUntil } from 'rxjs';

@Component({
  selector: 'app-registration',
  standalone: true,
  imports: [CommonModule, RouterModule, formModulesUtil(), angularMaterialModulesUtil()],
  templateUrl: './registration.component.html',
  styleUrl: './registration.component.scss'
})
export class RegistrationComponent extends SelfUnsubscriberBase implements OnInit {
  registrationForm: FormGroup = {} as FormGroup;

  username: FormControl = {} as FormControl;
  email: FormControl = {} as FormControl;
  password: FormControl = {} as FormControl;
  repeatPassword: FormControl = {} as FormControl;

  constructor(
    private formBuilder: FormBuilder,
    private userService: UserService,
    private router: Router,
    private loadingService: LoadingService,
  ) {
    super();
  }


  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm(): void {
    this.username = new FormControl('', [Validators.required, Validators.pattern(usernameFormat)]);
    this.email = new FormControl('', [Validators.required, Validators.email, Validators.maxLength(50)]);
    this.password = new FormControl('', [Validators.required, Validators.minLength(8), Validators.maxLength(30), Validators.pattern(passwordFormat)]);
    this.repeatPassword = new FormControl('', [Validators.required, Validators.minLength(8), Validators.maxLength(30), Validators.pattern(passwordFormat)]);

    this.registrationForm = this.formBuilder.group({
      username: this.username,
      email: this.email,
      password: this.password,
      repeatPassword: this.repeatPassword,
    });
  }

  onRegister(registrationData :IUserRegistration): void {
    if (this.registrationForm.valid) {
      this.userService.register(registrationData)
        .pipe(takeUntil(this.ngUnsubscribe))
        .subscribe(() => {
          this.router.navigate(['/login']);
          this.loadingService.hide();
        },(error) => {
          this.loadingService.hide();
        });
    }
  }

  onBack(): void {
    this.router.navigate(['/login']);
  }
}

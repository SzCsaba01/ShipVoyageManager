import { Component, OnInit } from '@angular/core';
import { SelfUnsubscriberBase } from '../../utils/SelfUnsubscribeBase';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { formModulesUtil } from '../../shared-modules/form-module.util';
import { angularMaterialModulesUtil } from '../../shared-modules/angular-material-modules.util';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { takeUntil } from 'rxjs';
import { AuthenticationService } from '../../services/authentication.service';
import { LoadingService } from '../../services/loading.service';
import { IAuthenticationRequest } from '../../models/authentication/authenticationRequest.model';
import { AuthState } from '../../state/authentication/auth.reducer';
import { Store } from '@ngrx/store';
import { loginSuccess } from '../../state/authentication/auth.actions';

@Component({
  selector: 'app-login',
  imports: [CommonModule, RouterModule, formModulesUtil(), angularMaterialModulesUtil()],
  standalone: true,
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent extends SelfUnsubscriberBase implements OnInit {
  loginForm: FormGroup = {} as FormGroup;
  userCredential: FormControl = {} as FormControl;
  password: FormControl = {} as FormControl;

  constructor(
    private formBuilder: FormBuilder,
    private router: Router,
    private authenticationService: AuthenticationService,
    private loadingService: LoadingService,
    private store: Store<{ auth: AuthState }>,
  ) {
    super();
  }

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm(): void {
    this.userCredential = new FormControl('', [Validators.required]);
    this.password = new FormControl('', [Validators.required]);

    this.loginForm = this.formBuilder.group({
      userCredential: this.userCredential,
      password: this.password,
    });
  }

  onLogin(authenticationRequest: IAuthenticationRequest): void {
    this.loadingService.show();
    this.authenticationService
      .login(authenticationRequest)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe((response) => {
        this.loadingService.hide();
        if (response) {
          this.store.dispatch(loginSuccess({ role: response }));
          this.router.navigate(['/home']);
        }
      });
  }
}

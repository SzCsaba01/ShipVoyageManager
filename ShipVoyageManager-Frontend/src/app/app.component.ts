import { Component, OnInit } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { ErrorMessageComponent } from "./shared-components/error-message/error-message.component";
import { LoaderComponent } from "./shared-components/loader/loader.component";
import { SelfUnsubscriberBase } from './utils/SelfUnsubscribeBase';
import { AuthenticationService } from './services/authentication.service';
import { takeUntil } from 'rxjs';
import { loginSuccess } from './state/authentication/auth.actions';
import { AuthState } from './state/authentication/auth.reducer';
import { Store } from '@ngrx/store';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, ErrorMessageComponent, LoaderComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent extends SelfUnsubscriberBase implements OnInit {
  title = 'ShipVoyageManager-Frontend';

  constructor(
    private authService: AuthenticationService,
    private store: Store<{auth: AuthState}>,
    private route: Router,
  ) { 
    super();
  }

  ngOnInit(): void {
    this.authService.isAuthenticated()
     .pipe(takeUntil(this.ngUnsubscribe))
     .subscribe((role: string) => {
        if (!role) {
          this.route.navigate(['login']);
          return;
        }

        this.store.dispatch(loginSuccess({ role }));
        this.route.navigate(['/home']);
     });

  }
}

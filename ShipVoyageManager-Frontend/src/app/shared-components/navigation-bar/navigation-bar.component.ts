import { Component, OnInit } from '@angular/core';
import { SelfUnsubscriberBase } from '../../utils/SelfUnsubscribeBase';
import { AuthenticationService } from '../../services/authentication.service';
import { Observable, takeUntil } from 'rxjs';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { angularMaterialModulesUtil } from '../../shared-modules/angular-material-modules.util';
import { AuthState } from '../../state/authentication/auth.reducer';
import { Store } from '@ngrx/store';
import { selectUserRole } from '../../state/authentication/auth.selector';
import { userRoles } from '../../constants/user-roles';

@Component({
  selector: 'app-navigation-bar',
  standalone: true,
  imports: [CommonModule, RouterModule, angularMaterialModulesUtil()],
  templateUrl: './navigation-bar.component.html',
  styleUrl: './navigation-bar.component.scss'
})
export class NavigationBarComponent extends SelfUnsubscriberBase implements OnInit {
  userRole$: Observable<string | null> = {} as Observable<string | null>;

  userRoles = userRoles;

  constructor(
    private router: Router,
    private authenticationService: AuthenticationService,
    private store: Store<{ auth: AuthState }>,
  ) {
    super();
  }

  ngOnInit(): void {
    this.userRole$ = this.store.select(selectUserRole);
  }

  onLogout() {
    this.authenticationService.logout()
    .pipe(takeUntil(this.ngUnsubscribe))
    .subscribe(() => {
      this.router.navigate(['/login']);
    });
  }

  navigateTo(route: string) {
    this.router.navigate([`/${route}`]);
  }
}

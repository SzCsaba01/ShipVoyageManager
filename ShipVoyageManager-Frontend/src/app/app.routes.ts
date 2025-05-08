import { Routes } from '@angular/router';
import { HomeLayoutComponent } from './layouts/home-layout/home-layout.component';
import { HomeComponent } from './components/home/home.component';
import { AuthGuard } from './helpers/auth.guard';
import { ManageUsersComponent } from './components/manage-users/manage-users.component';
import { RoleGuard } from './helpers/role.guard';
import { LandingPageLayoutComponent } from './layouts/landing-page-layout/landing-page-layout.component';
import { LoginComponent } from './components/login/login.component';
import { RegistrationComponent } from './components/registration/registration.component';
import { ForgotPasswordComponent } from './components/forgot-password/forgot-password.component';
import { ChangePasswordComponent } from './components/change-password/change-password.component';
import { VerifyEmailComponent } from './components/verify-email/verify-email.component';
import { PageNotFoundComponent } from './components/page-not-found/page-not-found.component';
import { ShipsComponent } from './components/ships/ships.component';
import { VoyagesComponent } from './components/voyages/voyages.component';
import { PortsComponent } from './components/ports/ports.component';
import { VisitedCountriesComponent } from './components/visited-countries/visited-countries.component';

export const routes: Routes = [
    {
        path: 'home',
        component: HomeLayoutComponent,
        children: [
            {
                path: '',
                component: HomeComponent,
                canActivate: [AuthGuard],
            },
            {
                path: 'manage-users',
                component: ManageUsersComponent,
                canActivate: [RoleGuard],
                data: {
                    expectedRoles: ['Admin'],
                }
            },
            {
                path: 'ships',
                component: ShipsComponent,
                canActivate: [AuthGuard],
            },
            {
                path: 'voyages',
                component: VoyagesComponent,
                canActivate: [AuthGuard],
            },
            {
                path: 'ports',
                component: PortsComponent,
                canActivate: [AuthGuard],
            }, 
            {
                path: 'visited-countries',
                component: VisitedCountriesComponent,
                canActivate: [AuthGuard],
            }
        ]
    },
    {
        path: '',
        redirectTo: 'login',
        pathMatch: 'full'
    },
    {
        path: '',
        component: LandingPageLayoutComponent,
        children: [
            {
                path: 'login',
                component: LoginComponent,
            },
            {
                path: 'registration',
                component: RegistrationComponent,
            },
            {
                path: 'forgot-password',
                component: ForgotPasswordComponent,
            },
            {
                path: 'change-password/:token',
                component: ChangePasswordComponent,
            },
            {
                path: 'verify-email/:token',
                component: VerifyEmailComponent,
            },
        ]
    },
    {
        path: 'page-not-found',
        component: PageNotFoundComponent,
    },
    {
        path: '**',
        redirectTo: 'page-not-found',
        pathMatch: 'full'
    }
];

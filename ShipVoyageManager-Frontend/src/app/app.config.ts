import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';

import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { routes } from './app.routes';
import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { provideClientHydration } from '@angular/platform-browser';
import { ErrorInterceptor } from './helpers/error.interceptor';
import { HttpRequestInterceptor } from './helpers/http-request-interceptor';
import { provideStore } from '@ngrx/store';
import { authReducer } from './state/authentication/auth.reducer';
import { provideCharts, withDefaultRegisterables } from 'ng2-charts';

export const appConfig: ApplicationConfig = {
  providers: [
    provideStore({
      auth: authReducer,
    }),
    provideRouter(routes),
    provideClientHydration(),
    provideHttpClient(withFetch(), withInterceptors([HttpRequestInterceptor, ErrorInterceptor])),
    provideAnimationsAsync(), provideCharts(withDefaultRegisterables()),
],
};

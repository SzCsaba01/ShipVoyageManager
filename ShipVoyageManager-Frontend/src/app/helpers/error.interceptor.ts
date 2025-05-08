import { HttpInterceptorFn, HttpRequest, HttpHandlerFn, HttpEvent, HttpStatusCode, HttpErrorResponse } from "@angular/common/http";
import { inject } from "@angular/core";
import { Router } from "@angular/router";
import { Observable, tap, catchError, throwError } from "rxjs";
import { AuthenticationService } from "../services/authentication.service";
import { ErrorHandlerService } from "../services/error-handler.service";
import { LoadingService } from "../services/loading.service";

export const ErrorInterceptor: HttpInterceptorFn = (
    req: HttpRequest<any>,
    next: HttpHandlerFn
  ): Observable<HttpEvent<any>> => {
    const errorHandlerService = inject(ErrorHandlerService);
    const authenticationService = inject(AuthenticationService);
    const loadingService = inject(LoadingService);
    const router = inject(Router);
    return next(req).pipe(
      tap((event: any) => {
        if (
          event.status == HttpStatusCode.Ok &&
          event.body &&
          event.body.message
        ) {
          errorHandlerService.showMessage(event.body.message, 'success');
        }
      }),
      catchError((error: HttpErrorResponse) => {
        let errorMessage = {} as { message: string };
        loadingService.hide();
        if (error.status == HttpStatusCode.Unauthorized) {
          authenticationService.logout();
          router.navigate(['']);
        } else {
          try {
            const errorMessageObject =
              error.error instanceof Object
                ? error.error
                : JSON.parse(error.error);
            if (
              errorMessageObject.message != null &&
              errorMessageObject.message != '' &&
              errorMessageObject.message != undefined &&
              errorMessageObject.message.length > 0
            ) {
              errorMessage.message = errorMessageObject.message;
            } else {
              errorMessage.message = error.statusText;
            }
          } catch {
            errorMessage.message = error.statusText;
          }
          errorHandlerService.showMessage(errorMessage.message, 'error');
        }
        return throwError(() => error);
      })
    );
  };
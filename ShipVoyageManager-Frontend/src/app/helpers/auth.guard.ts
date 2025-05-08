import { inject } from "@angular/core";
import { CanActivateFn, Router } from "@angular/router";
import { AuthenticationService } from "../services/authentication.service";
import { Store } from "@ngrx/store";
import { map } from "rxjs";
import { AuthState } from "../state/authentication/auth.reducer";
import { selectUserRole } from "../state/authentication/auth.selector";

export const AuthGuard : CanActivateFn = () => {
  const store = inject(Store<{ auth: AuthState }>);
  const router = inject(Router);

  return store.select(selectUserRole).pipe(
    map(role => {
      if (role) {
        return true;
      } else {
        router.navigate(['login']);
        return false;
      }
    })
  );
}
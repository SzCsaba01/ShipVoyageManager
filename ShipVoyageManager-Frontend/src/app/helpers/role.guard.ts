import { CanActivateFn, ActivatedRouteSnapshot, Router } from "@angular/router";
import { AuthenticationService } from "../services/authentication.service";
import { inject } from "@angular/core";
import { Store } from "@ngrx/store";
import { selectUserRole } from "../state/authentication/auth.selector";
import { map } from "rxjs";

export const RoleGuard: CanActivateFn = (route) => {
  const store = inject(Store);
  const router = inject(Router);
  
  return store.select(selectUserRole).pipe(
    map((role) => {
      const expectedRole = route.data['expectedRoles'];

      if (!role) {
        return false;
      }

      if (!expectedRole.includes(role)) {
        router.navigate(['**']); 
        return false;
      }

      return true;
    })
  );
};
import { createAction, props } from "@ngrx/store";

export const loginSuccess = createAction(
    '[Auth] Login Success',
    props<{ role: string }>()
);

export const logoutSuccess = createAction(
    '[Auth] Logout'
);


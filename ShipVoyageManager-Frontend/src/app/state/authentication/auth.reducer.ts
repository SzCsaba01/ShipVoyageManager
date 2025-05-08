import { createReducer, on } from "@ngrx/store";
import { loginSuccess, logoutSuccess } from "./auth.actions";

export interface AuthState {
    role: string | null;
}

export const initialState: AuthState = {
    role: null,
};

export const authReducer = createReducer(
    initialState,
    on(loginSuccess, (state, { role }) => ({ ...state, role })),
    on(logoutSuccess, (state) => ({ ...state, role: null }))
);
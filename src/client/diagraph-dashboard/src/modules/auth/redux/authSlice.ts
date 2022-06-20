import { createSlice, Middleware, MiddlewareAPI } from '@reduxjs/toolkit';

export interface AuthState {
    authenticated: boolean;
}

const authJson = localStorage.getItem('auth');
const initialState: AuthState = authJson
    ? JSON.parse(authJson)
    : { authenticated: false } as AuthState;

export const authMiddleware: Middleware = ({ getState }: MiddlewareAPI) => (next) => (action) => {
    if (action.type?.startsWith('auth/')) {
        localStorage.setItem(
            'auth',
            JSON.stringify(getState().auth)
        )
    }

    return next(action);
};

export const authSlice = createSlice({
    name: 'auth',
    initialState,
    reducers: {
        login: (state) => {
            state.authenticated = true;
        },
        logout: (state) => {
            state.authenticated = false;
        }
    }
});

export const authReducer = authSlice.reducer;
export const { login, logout } = authSlice.actions;
export const logoutActionType = authSlice.actions.logout.type;
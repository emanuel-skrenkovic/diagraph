import { createSlice } from '@reduxjs/toolkit';

export interface AuthState {
    authenticated: boolean;
}

const initialState: AuthState = {
    authenticated: false
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
import { createSlice, PayloadAction } from '@reduxjs/toolkit';

export interface GoogleIntegrationState {
    idempotencyKey?: string;
}

const initialState: GoogleIntegrationState = {
    idempotencyKey: undefined
};

export const googleIntegrationSlice = createSlice({
    name: 'googleIntegration',
    initialState,
    reducers: {
        setKey: (state, action: PayloadAction<string>) => {
            state.idempotencyKey = action.payload;
        }
    }
});

export const { setKey } = googleIntegrationSlice.actions;
export const googleIntegrationReducer = googleIntegrationSlice.reducer;
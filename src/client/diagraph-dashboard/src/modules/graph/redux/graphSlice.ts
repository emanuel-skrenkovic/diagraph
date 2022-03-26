import { createSlice, PayloadAction } from '@reduxjs/toolkit';

export interface GraphState {
    events: any[]
}

const initialState: GraphState = {
    events: []
};

export const graphSlice = createSlice({
    name: 'graph',
    initialState,
    reducers: {
        setEvents: (state, action: PayloadAction<any[]>) => {
            state.events = action.payload
        }
    }
});

export const { setEvents } = graphSlice.actions;
export const graphReducer = graphSlice.reducer;
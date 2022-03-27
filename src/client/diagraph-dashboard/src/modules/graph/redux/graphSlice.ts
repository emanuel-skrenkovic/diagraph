import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { GlucoseMeasurement } from 'types';

export interface GraphState {
    events: any[];
    data: GlucoseMeasurement[];
}

const initialState: GraphState = {
    events: [],
    data: []
};

export const graphSlice = createSlice({
    name: 'graph',
    initialState,
    reducers: {
        setEvents: (state, action: PayloadAction<any[]>) => {
            state.events = action.payload;
        },
        setData: (state, action: PayloadAction<GlucoseMeasurement[]>) => {
            state.data = action.payload;
        }
    }
});

export const { setEvents, setData } = graphSlice.actions;
export const graphReducer = graphSlice.reducer;
import { createSlice, PayloadAction } from '@reduxjs/toolkit';

import { GlucoseMeasurement } from 'types';
import { toLocalISODateString } from 'modules/common';

export interface GraphState {
    events: any[];
    data: GlucoseMeasurement[];
    dateRange: { from: string, to: string },
}

function graphDateLimits() {
    const today = new Date();
    today.setHours(0, 0, 0, 0);

    const tomorrow = new Date(today);
    tomorrow.setDate(tomorrow.getDate() + 1);
    tomorrow.setHours(0, 0, 0, 0);

    return { today, tomorrow };
}

const { today, tomorrow } = graphDateLimits();

const initialState: GraphState = {
    events: [],
    data: [],
    dateRange: { from: toLocalISODateString(today), to: toLocalISODateString(tomorrow) }
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
        },
        setDateRange: (state, action: PayloadAction<{ from: string, to: string }>) => {
            state.dateRange = action.payload;
        }
    }
});

export const { setEvents, setData, setDateRange } = graphSlice.actions;
export const graphReducer = graphSlice.reducer;
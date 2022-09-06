import { createSlice, PayloadAction } from '@reduxjs/toolkit';

import { GlucoseMeasurement } from 'diagraph/app/types';

export interface GraphState {
    events:          any[];
    measurements:    GlucoseMeasurement[];
    dateRange:       { from: string, to: string },
    selectedEventId: number | undefined;
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
    measurements: [],
    dateRange: { from: new Date(today).toISOString(), to: new Date(tomorrow).toISOString() },
    selectedEventId: undefined
};

export const graphSlice = createSlice({
    name: 'graph',
    initialState,
    reducers: {
        setEvents: (state, action: PayloadAction<any[]>) => {
            state.events = action.payload;
        },
        setData: (state, action: PayloadAction<GlucoseMeasurement[]>) => {
            state.measurements = action.payload;
        },
        setDateRange: (state, action: PayloadAction<{ from: string, to: string }>) => {
            state.dateRange = action.payload;
        },
        setSelectedEventId: (state, action: PayloadAction<number | undefined>) => {
            state.selectedEventId = action.payload;
        }
    }
});

export const { setEvents, setData, setDateRange, setSelectedEventId } = graphSlice.actions;
export const graphReducer = graphSlice.reducer;

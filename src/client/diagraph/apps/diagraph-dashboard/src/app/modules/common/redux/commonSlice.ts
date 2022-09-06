import { createSlice, PayloadAction } from '@reduxjs/toolkit';

import { EventTag } from 'diagraph/app/types';

export interface SharedState {
    tags: EventTag[];
    shownToasts: string[];
}

const initialState: SharedState = {
    tags: [],
    shownToasts: []
};

export const sharedSlice = createSlice({
    name: 'shared',
    initialState,
    reducers: {
        setTags: (state, action: PayloadAction<EventTag[]>) => {
            state.tags = action.payload;
        },
        showToast: (state, action: PayloadAction<string>) => {
            state.shownToasts.push(action.payload)
        },
        removeToast: (state, action: PayloadAction<string>) => {
            const index = state.shownToasts.indexOf(action.payload);
            if (index > -1) state.shownToasts.splice(index, 1);
        }
    }
});

export const { setTags, showToast, removeToast } = sharedSlice.actions;
export const sharedReducer = sharedSlice.reducer;

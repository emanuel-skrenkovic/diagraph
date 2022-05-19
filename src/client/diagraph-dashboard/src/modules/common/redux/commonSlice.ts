import { createSlice, PayloadAction } from '@reduxjs/toolkit';

import { EventTag } from 'types';

export interface SharedState {
    tags: EventTag[];
}

const initialState: SharedState = {
    tags: []
};

export const sharedSlice = createSlice({
    name: 'shared',
    initialState,
    reducers: {
        setTags: (state, action: PayloadAction<EventTag[]>) => {
            state.tags = action.payload;
        }
    }
});

export const { setTags } = sharedSlice.actions;
export const sharedReducer = sharedSlice.reducer;
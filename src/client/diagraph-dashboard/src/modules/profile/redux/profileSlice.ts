import { createSlice, PayloadAction } from '@reduxjs/toolkit';

export interface Profile {
    showLowLimit: boolean;
    showHighLimit: boolean;
    showAverage: boolean;
}

export interface ProfileState {
    profile: Profile
}

const initialState: ProfileState = {
    profile: {
        showLowLimit:  false,
        showHighLimit: false,
        showAverage:   false
    }
};

export const profileSlice = createSlice({
    name: 'profile',
    initialState,
    reducers: {
        setProfile: (state, action: PayloadAction<Profile>) => {
            state.profile = action.payload;
        },
    }
});

export const { setProfile } = profileSlice.actions;
export const profileReducer = profileSlice.reducer;
import { createSlice, PayloadAction } from '@reduxjs/toolkit';

export interface Profile {
    showLowLimit: boolean;
    showHighLimit: boolean;
    showAverage: boolean;
    googleIntegration: boolean;
    googleTaskList: string;
}

export const defaultProfile: Profile = {
    showLowLimit:      false,
    showHighLimit:     false,
    showAverage:       false,
    googleIntegration: false,
    googleTaskList:    ''
}

export interface ProfileState {
    profile: Profile
}

const initialState: ProfileState = {
    profile: defaultProfile
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
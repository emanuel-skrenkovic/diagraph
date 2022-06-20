import { useState, useEffect } from 'react';

import { Profile } from 'modules/profile';
import { useAppSelector } from 'modules/common';
import { useUpdateProfileMutation } from 'services';

export const useProfile =
    (): [Profile, (profile: Profile) => void, { isLoading: boolean, isSuccess: boolean }] => {
        const userProfile: Profile  = useAppSelector(state => state.profile.profile);
        const [profile, setProfile] = useState(userProfile);

        const [updateProfile, { isLoading, isSuccess }] = useUpdateProfileMutation(undefined);

        const setUserProfile = (newProfile: Profile) => {
            updateProfile(newProfile);
            setProfile(newProfile);
        }

        useEffect(() => {
            setProfile(userProfile);
        }, [userProfile]);

        return [profile, setUserProfile, { isLoading, isSuccess }];
};
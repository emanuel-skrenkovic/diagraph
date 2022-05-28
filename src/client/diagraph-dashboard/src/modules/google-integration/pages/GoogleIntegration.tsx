import React, { useState, useEffect } from 'react';

import { Loader } from 'modules/common';
import { useProfile } from 'modules/profile';
import { useGoogleIntegrationQuery} from 'services';

export const GoogleIntegration: React.FC = () => {
    const profileActions = useProfile();

    const [requestedIntegration, setRequestedIntegration] = useState(false)
    const { data, isLoading, isSuccess } =  useGoogleIntegrationQuery(
        encodeURIComponent('http://localhost:3000/integrations/google/confirm'),
        { skip: !requestedIntegration }
    );

    useEffect(() => {
        if (data) window.location.replace(data.redirectUri);
    }, [isSuccess]);

    {
        const [profile, _, { isLoading }] = profileActions;
        if (isLoading) return <Loader />;

        if (profile.googleIntegration) {
            return <span>Google already integrated.</span>
        }
    }

    function onClickIntegrate() {
        setRequestedIntegration(true)
    }

    if (isLoading) return <Loader />;

    return (
        <>
            <button onClick={onClickIntegrate}>Add Google integration</button>
        </>
    )
};
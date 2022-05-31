import React, { useEffect } from 'react';
import { useSelector } from 'react-redux';
import { useNavigate } from 'react-router-dom';

import { RootState } from 'store';
import { useQuery, Loader } from 'modules/common';
import { useGoogleIntegrationConfirmMutation } from 'services';

// This page does nothing except pass on the Google integration parameters
// back to the API.
export const GoogleIntegrationConfirm: React.FC = () => {
    const profile = useSelector((state: RootState) => state.profile.profile);
    const authorizationCode = useQuery('code');
    const [googleIntegrationConfirm, { isLoading, isSuccess }] = useGoogleIntegrationConfirmMutation();

    const navigate = useNavigate();
    useEffect(() => {
        if (isSuccess) {
            navigate('/');
            return;
        }

        googleIntegrationConfirm({
            code: authorizationCode,
            redirectUri: 'http://localhost:3000/integrations/google/confirm',
            scopes: ['asdf', 'fdsa'] // WTF is this?
        });
    }, [profile, isSuccess]);

    if (profile.googleIntegration) navigate('/');
    if (!authorizationCode)        return <span>Invalid URL: no authorization code.</span>
    if (isLoading)                 return <Loader />;

    return null;
};
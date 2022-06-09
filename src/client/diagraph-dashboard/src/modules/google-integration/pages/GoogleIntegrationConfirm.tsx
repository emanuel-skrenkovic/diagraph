import React, { useEffect } from 'react';
import { Navigate } from 'react-router-dom';

import { useQuery, Loader } from 'modules/common';
import { useGoogleIntegrationConfirmMutation } from 'services';

// This page does nothing except pass on the Google integration parameters
// back to the API.
export const GoogleIntegrationConfirm: React.FC = () => {
    const idempotencyKey    = useQuery('state');
    const authorizationCode = useQuery('code');

    const [googleIntegrationConfirm, { isLoading, isSuccess }] = useGoogleIntegrationConfirmMutation();

    useEffect(() => {
        if (isLoading || isSuccess) return;

        googleIntegrationConfirm({
            code:           authorizationCode,
            redirectUri:    'http://localhost:3000/integrations/google/confirm',
            scopes:         ['asdf', 'fdsa'], // WTF is this?
            idempotencyKey: idempotencyKey
        });
    }, []);

    if (isLoading)          return <Loader />;
    if (!authorizationCode) return <span>Invalid URL: no authorization code.</span>

    return <Navigate to="/" />
};
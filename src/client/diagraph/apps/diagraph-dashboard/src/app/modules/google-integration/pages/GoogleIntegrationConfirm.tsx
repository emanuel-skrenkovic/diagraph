import React, { useEffect } from 'react';
import { Navigate } from 'react-router-dom';

import { useQuery, Loader } from 'diagraph/app/modules/common';
import { useGoogleIntegrationConfirmMutation } from 'diagraph/app/services';

// This page does nothing except pass on the Google integration parameters
// back to the API.
export const GoogleIntegrationConfirm = () => {
    const idempotencyKey    = useQuery('state');
    const authorizationCode = useQuery('code');
    const scope             = useQuery('scope');

    const [googleIntegrationConfirm, { isLoading, isSuccess }] = useGoogleIntegrationConfirmMutation();

    useEffect(() => {
        if (isLoading || isSuccess) return;

        googleIntegrationConfirm({
            code:           authorizationCode,
            redirectUri:    'http://localhost:3000/integrations/google/confirm',
            scope:          scope ? decodeURI(scope).split(' ') : null,
            idempotencyKey: idempotencyKey
        });
    }, []);

    if (isLoading)          return <Loader />;
    if (!authorizationCode) return <span>Invalid URL: no authorization code.</span>

    return <Navigate to="/" />
};

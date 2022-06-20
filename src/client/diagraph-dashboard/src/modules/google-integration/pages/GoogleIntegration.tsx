import React, { useState, useEffect } from 'react';
import { useDispatch } from 'react-redux';

import { Button, Centered } from 'styles';
import { useGoogleIntegrationQuery } from 'services';
import { Loader, useAppSelector } from 'modules/common';
import { GoogleIntegrationSettings, setKey } from 'modules/google-integration';

export const GoogleIntegration = () => {
    const googleIntegration = useAppSelector(state => state.profile.profile.googleIntegration);
    const idempotencyKey    = useAppSelector(state => state.googleIntegration.idempotencyKey);

    const dispatch = useDispatch();
    useEffect(() => {
        if (!googleIntegration && !idempotencyKey) dispatch(setKey(window.crypto.randomUUID()));
    }, [googleIntegration]);

    const [requestedIntegration, setRequestedIntegration] = useState(false)

    const { isLoading, data } = useGoogleIntegrationQuery({
            redirect: encodeURIComponent('http://localhost:3000/integrations/google/confirm'), // TODO: url from where?
            state: idempotencyKey
        },
        { skip: !requestedIntegration }
    );

    if (isLoading) return <Loader />;
    if (requestedIntegration && data?.redirectUri) window.location.replace(data.redirectUri)

    return (
        <Centered>
            {googleIntegration
                ? <GoogleIntegrationSettings />
                : <Button onClick={() => setRequestedIntegration(true)}>
                    Add Google integration
                  </Button>
            }
        </Centered>
    );
};
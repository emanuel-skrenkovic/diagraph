import React, { useEffect } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';

import { useProfile } from 'modules/profile';
import { Loader } from 'modules/common';
import { useGoogleIntegrationConfirmMutation} from 'services';

export const GoogleIntegrationConfirm: React.FC = () => {
    const profileActions = useProfile();

    const location = useLocation();
    const query = new URLSearchParams(location.search);
    const authorizationCode = query.get('code');

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
            scopes: ['asdf', 'fdsa']
        });
    }, [isSuccess]);

    {
        const [profile, _, { isLoading }] = profileActions;
        if (isLoading) return <Loader />;

        if (profile.googleIntegration) {
            navigate('/');
        }
    }

    if (!authorizationCode) console.error('TODO');
    if (isLoading) return <Loader />;

    return <>Hi</>
};
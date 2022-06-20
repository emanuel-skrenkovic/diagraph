import React from 'react';
import { Navigate } from 'react-router-dom';

import { useAppSelector} from 'modules/common';

export function withAuth<P>(Component: React.ComponentType<P>) {
    return (props: P) => {
        const isAuthenticated  = useAppSelector(state => state.auth.authenticated);
        return isAuthenticated
            ? <Component {...props} />
            : <Navigate to={'/login'} />;
    };
}
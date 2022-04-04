import React from 'react';
import { Route, Navigate, RouteProps } from 'react-router-dom';

export interface ProtectedRouteProps extends RouteProps {
    allowExpression: boolean;
    fallbackRoute: string;
}

export const ProtectedRoute: React.FC<ProtectedRouteProps> =({ allowExpression, fallbackRoute, ...rest }) => {
    return (
        allowExpression ? <Route {...rest} /> : <Navigate to={fallbackRoute} />
    );
};
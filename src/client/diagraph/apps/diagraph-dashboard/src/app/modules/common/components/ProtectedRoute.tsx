import React from 'react';
import { Navigate } from 'react-router-dom';

export type ProtectedRouteProps = {
    condition: boolean
    fallback: string;
    children?: React.ReactNode;
}

export const ProtectedRoute = ({ condition, fallback, children }: ProtectedRouteProps) => {
    if (condition) return <>{children}</>;
    return <Navigate to={fallback} />
};
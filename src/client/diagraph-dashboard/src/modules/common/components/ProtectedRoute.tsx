import React from 'react';
import { Navigate } from 'react-router-dom';

export interface ProtectedRouteProps {
    condition: boolean
    fallback: string;
}

export const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ condition, fallback, children }) => {
    if (condition) return <>{children}</>;
    return <Navigate to={fallback} />
};
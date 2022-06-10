import React from 'react';
import { useSelector } from 'react-redux';
import { Navigate, Link } from 'react-router-dom';

import { RootState } from 'store';
import { Loader } from 'modules/common';
import { LoginForm } from 'modules/auth';
import { useGetSessionQuery, useLoginMutation } from 'services';

import 'App.css';

export const Login = () => {
    const authenticated = useSelector((state: RootState) => state.auth.authenticated);

    const { isLoading: isSessionLoading } = useGetSessionQuery(undefined);
    const [login, { isLoading, isError }] = useLoginMutation();

    const onClickLogin = (email: string, password: string) => login({email, password});

    if (authenticated)                 return <Navigate to="/" />;
    if (isLoading || isSessionLoading) return <Loader />;

    return (
        <div className="container vertical">
            <div className="container" style={{marginTop: "5%"}}>
                <LoginForm onSubmit={onClickLogin} />
            </div>
            <div className="container item">
                <span>Don't have an account</span>
                <Link to="/register">Register</Link>
            </div>
            {isError && <span>Login failed</span>}
        </div>
    );
};
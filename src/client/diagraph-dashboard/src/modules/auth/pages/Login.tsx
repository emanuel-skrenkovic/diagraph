import React from 'react';
import { useSelector } from 'react-redux';
import { RootState } from 'store';
import { Navigate, Link } from 'react-router-dom';

import { Loader } from 'modules/common';
import { useLoginMutation } from 'services';
import { LoginForm } from 'modules/auth';

import 'App.css';

export const Login = () => {
    const authenticated = useSelector((state: RootState) => state.auth.authenticated);
    const [login, { isLoading, isError }] = useLoginMutation();

    const onClickLogin = (email: string, password: string) => login({email, password});

    if (isLoading) return <Loader />;
    if (authenticated) return <Navigate to="/" />

    return (
        <div className="container horizontal">
            <LoginForm onSubmit={onClickLogin} />
            <div className="container item">
                <span>Don't have an account</span>
                <Link to="/register">Register</Link>
            </div>
            {isError && <span>Login failed</span>}
        </div>
    );
};
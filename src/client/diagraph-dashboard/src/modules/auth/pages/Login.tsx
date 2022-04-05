import React, { useEffect } from 'react';
import { useSelector } from 'react-redux';
import { useNavigate, Link } from 'react-router-dom';

import { RootState } from 'store';
import { Loader } from 'modules/common';
import { LoginForm } from 'modules/auth';
import { useLoginMutation } from 'services';

import 'App.css';

export const Login = () => {
    const navigate = useNavigate();
    const authenticated = useSelector((state: RootState) => state.auth.authenticated);

    const [login, { isLoading, isError }] = useLoginMutation();

    const onClickLogin = (email: string, password: string) => login({email, password});

    useEffect(() => {
        if (authenticated) navigate(-1);
    }, [authenticated]);

    if (isLoading) return <Loader />;

    return (
        <div className="container horizontal">
            <div className="container">
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
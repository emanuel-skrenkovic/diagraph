import React from 'react';
import { Navigate, Link } from 'react-router-dom';

import { Container } from 'styles';
import { LoginForm } from 'modules/auth';
import { Loader, useAppSelector } from 'modules/common';
import { useGetSessionQuery, useLoginMutation } from 'services';

export const Login = () => {
    const authenticated = useAppSelector(state => state.auth.authenticated);

    const { isLoading: isSessionLoading } = useGetSessionQuery(undefined);
    const [login, { isLoading, isError }] = useLoginMutation();

    const onClickLogin = (email: string, password: string) => login({email, password});

    if (authenticated)                 return <Navigate to="/" />;
    if (isLoading || isSessionLoading) return <Loader />;

    return (
        <Container vertical>
            <LoginForm onSubmit={onClickLogin} />
            <span>Don't have an account?</span>
            <Link to="/register">Register</Link>
            {isError && <span>Login failed</span>}
        </Container>
    );
};
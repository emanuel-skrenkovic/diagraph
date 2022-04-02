import React from 'react';
import { useDispatch } from 'react-redux';
import { useNavigate, Link } from 'react-router-dom';

import { setAuthenticated } from 'modules/auth';
import { useLoginMutation } from 'services';
import { LoginForm } from 'modules/auth';

import 'App.css';

export const Login = () => {
    const dispatch = useDispatch();
    const navigate = useNavigate();

    const [login, _] = useLoginMutation();

    const onClickLogin = (email: string, password: string) => {
        login({email, password});
        dispatch(setAuthenticated(true));
        navigate('/');
    }

    return (
        <div className="container horizontal">
            <LoginForm onSubmit={onClickLogin} />
            <div className="container item">
                <span>Don't have an account</span>
                <Link to="/register">Register</Link>
            </div>
        </div>
    );
};
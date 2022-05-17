import React from 'react';
import { Link } from 'react-router-dom';

import { Loader } from 'modules/common';
import { RegisterForm } from 'modules/auth';
import { useRegisterMutation } from 'services';

export const Register = () => {
    const [register, { isLoading, isSuccess }] = useRegisterMutation();

    if (isLoading) return <Loader />;

    return (
        <div className="container vertical">
            <div className="container">
                <RegisterForm onSubmit={(email, password) => register({ email, password })} />
            </div>
            {isSuccess &&
                <div className="item">
                    <span>{isSuccess && 'Thank you for registering, an email has been sent to your account for confirmation.'}</span>
                    <br/>
                    <Link to="/login">Log in.</Link>
                </div>
            }
        </div>
    );
};
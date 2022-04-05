import React from 'react';
import { Link } from 'react-router-dom';

import { Loader } from 'modules/common';
import { RegisterForm } from 'modules/auth';
import { useRegisterMutation } from 'services';

export const Register = () => {
    const [register, { isLoading, isSuccess }] = useRegisterMutation();

    if (isLoading) return <Loader />;

    return (
        <div className="container horizontal">
            <RegisterForm onSubmit={(email, password) => register({ email, password })} />
            {isSuccess &&
                <div className="item">
                    <span>{isSuccess && 'Account successfully created. Please'}</span>
                    <Link to="/login">log in.</Link>
                </div>
            }
        </div>
    );
};
import React from 'react';

import { RegisterForm } from 'modules/auth';
import { useRegisterMutation } from 'services';

export const Register = () => {
    const [register, _] = useRegisterMutation();

    return (
        <RegisterForm onSubmit={(email, password) => register({ email, password })} />
    );
};
import React, { FormEvent } from 'react';

import { useValidation } from 'modules/common';

import 'App.css';

const emailValidation = (email: string | undefined): [boolean, string] => {
    if (!email) return [false, 'Email must not be empty.'];

    return [true, ''];
}

const passwordValidation = (password: string | undefined): [boolean, string] => {
    if (!password) return [false, 'Password must not be empty.'];

    return [true, ''];
};

export interface LoginFormProps {
    onSubmit: (email: string, password: string) => void;
}

export const LoginForm: React.FC<LoginFormProps> = ({ onSubmit }) => {
    const [email,
           setEmail,
           emailError,
           validateEmail] = useValidation<string>(emailValidation, '');
    const [password,
           setPassword,
           passwordError,
           validatePassword] = useValidation<string>(passwordValidation, '');

    const onClickSubmit = (e: FormEvent<HTMLButtonElement>) => {
        e.preventDefault();

        const emailValid    = validateEmail();
        const passwordValid = validatePassword();

        if (!emailValid || !passwordValid) return;

        onSubmit(email!, password!);
    }

    return (
        <form className="container vertical box">
            <label htmlFor="emailInput">Email:</label>
            <div className="item">
                <input className={emailError && 'input invalid'}
                       id="emailInput"
                       type="text"
                       value={email}
                       onChange={e => setEmail(e.currentTarget.value)} />
            </div>
            <span className="input label">{emailError}</span>
            <label htmlFor="passwordInput">Password:</label>
            <div className="item">
                <input className={passwordError && 'input invalid'}
                       id="passwordInput"
                       type="password"
                       value={password}
                       onChange={e => setPassword(e.currentTarget.value)} />
            </div>
            <span className="input label">{passwordError}</span>
            <button className="button blue centered"
                    type="submit"
                    disabled={!!emailError || !!passwordError}
                    onClick={onClickSubmit}>Log in</button>
        </form>
    )
};

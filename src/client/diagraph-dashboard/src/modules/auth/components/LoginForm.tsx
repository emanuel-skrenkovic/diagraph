import React, { useState, ChangeEvent, FormEvent } from 'react';

import 'App.css';

export interface LoginFormProps {
    onSubmit: (email: string, password: string) => void;
}

export const LoginForm: React.FC<LoginFormProps> = ({ onSubmit }) => {
    const [email, setEmail]                 = useState('');
    const [emailError, setEmailError]       = useState('');

    const [password, setPassword]           = useState('');
    const [passwordError, setPasswordError] = useState('');

    const onClickSubmit = (e: FormEvent<HTMLButtonElement>) => {
        e.preventDefault();

        if (!email)    setEmailError('Email must not be empty.');
        if (!password) setPasswordError('Password must not be empty.');

        if (!email || !password) return;

        onSubmit(email, password);
    }

    const onChangeEmail = (e: ChangeEvent<HTMLInputElement>) => {
        if (!e.currentTarget.value) setEmailError('Email must not be empty.');
        else                        setEmailError('');

        setEmail(e.currentTarget.value);
    };

    const onChangePassword = (e: ChangeEvent<HTMLInputElement>) => {
        if (!e.currentTarget.value) setPasswordError('Password must not be empty.');
        else                        setPasswordError('');

        setPassword(e.currentTarget.value);
    }

    return (
        <form className="container vertical box">
            <label htmlFor="emailInput">Email:</label>
            <div className="item">
                <input className={emailError && 'input invalid'}
                       id="emailInput"
                       type="text"
                       value={email}
                       onChange={onChangeEmail} />
            </div>
            <span className="input label">{emailError}</span>
            <label htmlFor="passwordInput">Password:</label>
            <div className="item">
                <input className={passwordError && 'input invalid'}
                       id="passwordInput"
                       type="password"
                       value={password}
                       onChange={onChangePassword} />
            </div>
            <span className="input label">{passwordError}</span>
            <button className="button blue centered"
                    type="submit"
                    disabled={!!emailError || !!passwordError}
                    onClick={onClickSubmit}>Log in</button>
        </form>
    )
};

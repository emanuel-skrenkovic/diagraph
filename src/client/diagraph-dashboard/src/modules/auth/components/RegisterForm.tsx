import React, { useState, FormEvent, ChangeEvent } from 'react';

import 'App.css';

export interface RegisterFormProps {
    onSubmit: (email: string, password: string) => void;
}

export const RegisterForm: React.FC<RegisterFormProps> = ({ onSubmit }) => {
    const [email, setEmail]                     = useState('');
    const [emailError, setEmailError]           = useState('');

    const [password, setPassword]               = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    const [passwordError, setPasswordError]     = useState('');

    const onClickSubmit = (e: FormEvent<HTMLButtonElement>) => {
        e.preventDefault();

        if (!email)    setEmailError('Email must not be empty.');
        if (!password) setPasswordError('Password must not be empty.');

        onSubmit(email, password);
    };

    const onChangeConfirmPassword = (e: ChangeEvent<HTMLInputElement>) => {
        if (password !== e.currentTarget.value) setPasswordError('Password fields do not match.');
        else if (passwordError)                 setPasswordError('');

        setConfirmPassword(e.currentTarget.value)
    };

    const onChangeEmail = (e: ChangeEvent<HTMLInputElement>) => {
        if (!e.currentTarget.value) setEmailError('Email must not be empty.');
        else                        setEmailError('');

        setEmail(e.currentTarget.value);
    };

    const onChangePassword = (e: ChangeEvent<HTMLInputElement>) => {
        if (!e.currentTarget.value) setPasswordError('Password must not be empty.');
        else                        setPasswordError('');

        setPassword(e.currentTarget.value);
    };

    return (
        <>
            <form className="container horizontal box">
                <label htmlFor="emailInput">Email:</label>
                <div className="item">
                    <input className={emailError && 'input invalid'}
                           id="emailInput"
                           type="text"
                           value={email}
                           onChange={onChangeEmail} />
                </div>
                <span className="input label">{emailError ?? ' '}</span>
                <label htmlFor="passwordInput">Password:</label>
                <div className="item">
                    <input className={passwordError && 'input invalid'}
                           id="passwordInput"
                           type="password"
                           value={password}
                           onChange={onChangePassword} />
                </div>
                <span className="input label">{passwordError}</span>
                <label htmlFor="confirmPasswordInput">Confirm password:</label>
                <div className="item">
                    <input id="confirmPasswordInput"
                           type="password"
                           value={confirmPassword}
                           onChange={onChangeConfirmPassword} />
                </div>
                <button className="button blue"
                        type="submit"
                        disabled={!!emailError || !!passwordError}
                        onClick={onClickSubmit}>Register</button>
            </form>
        </>

    )
};
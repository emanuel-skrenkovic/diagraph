import React, { FormEvent } from 'react';

import { useValidation } from 'modules/common';

import 'App.css';

function passwordValidation(password: string | undefined): [boolean, string] {
    if (!password) return [false, 'Password must not be empty.'];
    return [true, ''];
}

function emailValidation(email: string | undefined): [boolean, string] {
    if (!email) return [false, 'Email must not be empty'];
    return [true, ''];
}

export interface RegisterFormProps {
    onSubmit: (email: string, password: string) => void;
}

export const RegisterForm: React.FC<RegisterFormProps> = ({ onSubmit }) => {
    function confirmPasswordValidation(confirmPassword: string | undefined): [boolean, string] {
        if (password !== confirmPassword) return [false, 'Password fields do not match.'];
        return [true, ''];
    }

    const [password, setPassword, passwordError, validatePassword]
        = useValidation<string>(passwordValidation, '');
    const [confirmPassword, setConfirmPassword, confirmPasswordError, validateConfirmPassword]
        = useValidation<string>(confirmPasswordValidation, '');
    const [email, setEmail, emailError, validateEmail]
        = useValidation<string>(emailValidation, '');

    const onClickSubmit = (e: FormEvent<HTMLButtonElement>) => {
        e.preventDefault();

        const validEmail        = validateEmail();
        const validPassword     = validatePassword();
        const passwordConfirmed = validateConfirmPassword();

        if (!validEmail || !validPassword || !passwordConfirmed) return;

        onSubmit(email!, password!);
    };

    return (
        <>
            <form className="container vertical box">
                <label htmlFor="emailInput">Email:</label>
                <div className="item">
                    <input className={emailError && 'input invalid'}
                           id="emailInput"
                           type="text"
                           value={email}
                           onChange={e => setEmail(e.currentTarget.value)} />
                </div>
                <span className="input label">{emailError ?? ' '}</span>
                <label htmlFor="passwordInput">Password:</label>
                <div className="item">
                    <input className={passwordError && 'input invalid'}
                           id="passwordInput"
                           type="password"
                           value={password}
                           onChange={e => setPassword(e.currentTarget.value)} />
                </div>
                <span className="input label">{confirmPasswordError || passwordError}</span>
                <label htmlFor="confirmPasswordInput">Confirm password:</label>
                <div className="item">
                    <input id="confirmPasswordInput"
                           type="password"
                           value={confirmPassword}
                           onChange={e => setConfirmPassword(e.currentTarget.value)} />
                </div>
                <button className="button blue centered"
                        type="submit"
                        disabled={!!emailError || !!passwordError}
                        onClick={onClickSubmit}>Register</button>
            </form>
        </>

    )
};
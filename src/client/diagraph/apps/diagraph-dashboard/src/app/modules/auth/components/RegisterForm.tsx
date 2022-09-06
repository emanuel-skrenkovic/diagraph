import React, { FormEvent } from 'react';

import { PrimaryButton, Centered, Container, Input, Item } from 'diagraph/styles';
import { useValidation } from 'diagraph/app/modules/common';

function passwordValidation(password: string | undefined): [boolean, string] {
    if (!password) return [false, 'Password must not be empty.'];
    return [true, ''];
}

function emailValidation(email: string | undefined): [boolean, string] {
    if (!email) return [false, 'Email must not be empty'];
    return [true, ''];
}

export type RegisterFormProps = {
    onSubmit: (email: string, password: string) => void;
}

export const RegisterForm = ({ onSubmit }: RegisterFormProps) => {
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
        <Container vertical>
            <form>
                <label htmlFor="emailInput">Email:</label>
                <Item>
                    <Input className={emailError && 'input invalid'}
                           id="emailInput"
                           type="text"
                           value={email}
                           onChange={e => setEmail(e.currentTarget.value)} />
                </Item>
                <span className="input label">{emailError ?? ' '}</span>
                <label htmlFor="passwordInput">Password:</label>
                <Item>
                    <Input className={passwordError && 'input invalid'}
                           id="passwordInput"
                           type="password"
                           value={password}
                           onChange={e => setPassword(e.currentTarget.value)} />
                </Item>
                <span className="input label">{confirmPasswordError || passwordError}</span>
                <label htmlFor="confirmPasswordInput">Confirm password:</label>
                <Item>
                    <Input id="confirmPasswordInput"
                           type="password"
                           value={confirmPassword}
                           onChange={e => setConfirmPassword(e.currentTarget.value)} />
                </Item>
                <Centered as={PrimaryButton}
                          type="submit"
                          disabled={!!emailError || !!passwordError}
                          onClick={onClickSubmit}>
                    Register
                </Centered>
            </form>
        </Container>
    )
};

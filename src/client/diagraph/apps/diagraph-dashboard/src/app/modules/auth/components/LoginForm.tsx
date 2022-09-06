import React, { FormEvent } from 'react';

import { Box, PrimaryButton, Centered, Container, Input, Item } from 'diagraph/styles';
import { useValidation } from 'diagraph/app/modules/common';

const emailValidation = (email: string | undefined): [boolean, string] => {
    if (!email) return [false, 'Email must not be empty.'];

    return [true, ''];
}

const passwordValidation = (password: string | undefined): [boolean, string] => {
    if (!password) return [false, 'Password must not be empty.'];

    return [true, ''];
};

export type LoginFormProps = {
    onSubmit: (email: string, password: string) => void;
}

export const LoginForm = ({ onSubmit }: LoginFormProps) => {
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
        <Container vertical>
            <Box>
                <form>
                    <label htmlFor="emailInput">Email:</label>
                    <Item>
                        <Input className={emailError && 'input invalid'}
                               id="emailInput"
                               type="text"
                               value={email}
                               onChange={e => setEmail(e.currentTarget.value)} />
                    </Item>
                    <span className="input label">{emailError}</span>
                    <label htmlFor="passwordInput">Password:</label>
                    <Item>
                        <Input className={passwordError && 'input invalid'}
                               id="passwordInput"
                               type="password"
                               value={password}
                               onChange={e => setPassword(e.currentTarget.value)} />
                    </Item>
                    <span className="input label">{passwordError}</span>
                    <Centered as={PrimaryButton}
                              type="submit"
                              disabled={!!emailError || !!passwordError}
                              onClick={onClickSubmit}>
                        Log in
                    </Centered>
                </form>
            </Box>
        </Container>
    )
};

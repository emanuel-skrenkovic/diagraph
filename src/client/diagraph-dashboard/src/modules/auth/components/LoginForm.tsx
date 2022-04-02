import React, { useState, FormEvent } from 'react';

export interface LoginFormProps {
    onSubmit: (email: string, password: string) => void;
}

export const LoginForm: React.FC<LoginFormProps> = ({ onSubmit }) => {
    const [email, setEmail]       = useState('');
    const [password, setPassword] = useState('');

    const onClickSubmit = (e: FormEvent<HTMLButtonElement>) => {
        e.preventDefault();
        onSubmit(email, password);
    }

    return (
        <form className="container horizontal box">
            <label htmlFor="emailInput">Email:</label>
            <div className="item">
                <input id="emailInput"
                       type="text"
                       value={email}
                       onChange={e => setEmail(e.currentTarget.value)} />
            </div>
            <label htmlFor="passwordInput">Password:</label>
            <div className="item">
                <input id="passwordInput"
                       type="password"
                       value={password}
                       onChange={e => setPassword(e.currentTarget.value)} />
            </div>
            <button className="button"
                    type="submit"
                    onClick={onClickSubmit}>Log in</button>
        </form>
    )
};

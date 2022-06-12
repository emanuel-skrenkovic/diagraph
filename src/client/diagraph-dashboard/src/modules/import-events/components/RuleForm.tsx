import React, { useState, useEffect, FormEvent } from 'react';

import { BlueButton, Container, Input } from 'styles';
import { Rule } from 'types';

export interface RuleFormProps {
    value?: Rule
    onSubmit: (rule: Rule) => void;
    disabled?: boolean;
    buttonText?: string;
}

export const RuleForm: React.FC<RuleFormProps> = ({ value, onSubmit, disabled, buttonText }) => {
    let initialField      = '';
    let initialExpression = '';
    if (value) {
        const parts = value.expression.split('=');

        initialField      = parts[0].trim();
        initialExpression = parts[1].trim();
    }

    const [field, setField]           = useState<string | undefined>(initialField);
    const [expression, setExpression] = useState<string | undefined>(initialExpression);

    useEffect(() => {
        setField(initialField);
        setExpression(initialExpression);
    }, [initialField, initialExpression]);

    function onClickSubmit(e: FormEvent<HTMLButtonElement>) {
        e.preventDefault();

        if (field && expression) {
            onSubmit({ expression: `${field} = ${expression}` });
        }
    }

    return (
        <>
            <Container style={{width:"fit-content"}}>
                <select value={field}
                        onChange={e => setField(e.currentTarget.value)}
                        disabled={disabled ?? false}>
                    <option />
                    <option>occurredAtUtc</option>
                    <option>text</option>
                </select>
                <span>=</span>
                <Input type="text"
                       value={expression}
                       onChange={e => setExpression(e.currentTarget.value)}
                       disabled={disabled ?? false} />
            </Container>
            {!disabled &&
                <BlueButton onClick={onClickSubmit}
                        disabled={disabled ?? false}>
                    {buttonText ?? 'Save'}
                </BlueButton>
            }
        </>
    );
};
import React, { useState, useEffect, FormEvent } from 'react';

import { PrimaryButton, Container, Input } from 'styles';
import { Rule } from 'types';
import { Options } from 'modules/common';

export type RuleFormProps = {
    value?: Rule;
    onSubmit: (rule: Rule) => void;
    disabled?: boolean;
    buttonText?: string;
}

const ALLOWED_FIELDS = ['occurredAtUtc', 'text'];

export const RuleForm = ({ value, onSubmit, disabled, buttonText }: RuleFormProps) => {
    let initialField      = '';
    let initialExpression = '';

    if (value?.expression) {
        const parts       = value.expression.split('=');
        initialField      = parts[0].trim();
        initialExpression = parts[1].trim();
    }

    const [field, setField]           = useState<string | undefined>(initialField);
    const [expression, setExpression] = useState<string | undefined>(initialExpression);

    useEffect(() => {
        setField(initialField);
        setExpression(initialExpression);
    }, [value, initialField, initialExpression]);

    const onClickSubmit = (e: FormEvent<HTMLButtonElement>) => {
        e.preventDefault();
        if (field && expression) onSubmit({ expression: `${field} = ${expression}` });
    }

    return (
        <Container style={{width:"fit-content"}}>
            <select value={field}
                    onChange={e => setField(e.currentTarget.value)}
                    disabled={disabled}>
                <Options elements={ALLOWED_FIELDS} value={v => v} />
            </select>
            <span>=</span>
            <Input type="text"
                   value={expression}
                   onChange={e => setExpression(e.currentTarget.value)}
                   disabled={disabled ?? false} />
            {!disabled &&
                <PrimaryButton onClick={onClickSubmit} disabled={disabled ?? false}>
                    {buttonText ?? 'Save'}
                </PrimaryButton>
            }
        </Container>
    );
};
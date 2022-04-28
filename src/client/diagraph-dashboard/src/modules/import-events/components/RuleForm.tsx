import React, { useState, ChangeEvent, FormEvent } from 'react';

import { Rule } from 'modules/import-events';

export interface RuleFormProps {
    initial?: Rule
    onSubmit: (rule: Rule) => void;
}

const EMPTY_RULE = { expression: '' };

export const RuleForm: React.FC<RuleFormProps> = ({ initial, onSubmit }) => {
    const [rule, setRule] = useState<Rule>(initial ? initial :  EMPTY_RULE);

    function onClickSubmit(e: FormEvent<HTMLButtonElement>) {
        e.preventDefault();

        if (rule) onSubmit(rule);
        setRule(EMPTY_RULE)
    }

    function onExpressionChange(e: ChangeEvent<HTMLInputElement>) {
        setRule({ ...rule, expression: e.currentTarget.value });
    }

    // TODO: split inputs into multiple:
    // 1. field
    // 2. selector for every '+'
    // 3. ...

    return (
        <>
            <input type="text"
                   value={rule.expression}
                   onChange={onExpressionChange}/>
            <button className="button blue" type="submit" onClick={onClickSubmit}>Submit</button>
        </>
    );
};
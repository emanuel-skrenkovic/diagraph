import React, { useState, FormEvent } from 'react';

import { For } from 'modules/common';
import { Rule, RuleForm, TemplateHeaderMapping } from 'modules/import-events';

export interface HeaderMappingFormProps {
    value?: TemplateHeaderMapping;
    onSubmit: (template: TemplateHeaderMapping) => void;
}

const DEFAULT_MAPPING = {
    header: '',
    rules: [],
    tags: []
};

export const HeaderMappingForm: React.FC<HeaderMappingFormProps> = ({ value, onSubmit }) => {
    const [tags, setTags] = useState<string[]>([]);
    const [editingRuleId, setEditingRuleId] = useState<string | undefined>(undefined);
    const [template, setTemplate] = useState<TemplateHeaderMapping>(value ?? DEFAULT_MAPPING);

    function onClickSubmit(e: FormEvent<HTMLButtonElement>) {
        e.preventDefault();
        onSubmit(template);
        setTemplate(DEFAULT_MAPPING);
    }

    function renderRule(rule: Rule) {
        if (editingRuleId == rule.expression) {
            return (
                <RuleForm initial={rule}
                          onSubmit={r => {
                              rule.expression = r.expression;
                              setEditingRuleId(undefined);
                          }} />
            );
        } else {
            return (
                <tr>
                    <td>{rule.expression}</td>
                    <td>
                        <button onClick={() => setEditingRuleId(rule.expression)}>
                            Edit
                        </button>
                    </td>
                    <td>
                        <button onClick={() => setTemplate({
                            ...template,
                            rules: template.rules.filter(r => r !== rule)
                        })}>
                            Remove
                        </button>
                    </td>
                </tr>
            );
        }
    }

    return (
        <form className="container horizontal box">
            <div className="container horizontal">
                <div className="item">
                    <label htmlFor="headerName">Header</label>
                    <input id="headerName"
                           type="text"
                           value={template.header}
                           onChange={e => setTemplate({ ...template, header: e.currentTarget.value })}/>
                </div>
                <div className="item">
                    <label>Rules</label>
                    <table>
                        <thead>
                        <tr>
                            <th></th>
                            <th></th>
                        </tr>
                        </thead>
                        <tbody>
                        <For each={template.rules} onEach={renderRule} />
                        </tbody>
                    </table>
                    {!editingRuleId &&
                        <RuleForm onSubmit={r => setTemplate({
                            ...template,
                            rules: [...template.rules, r]
                        })} />
                    }
                </div>
                <div className="item container horizontal">
                    <label htmlFor="selectTags">Tags</label>
                    <select id="selectTags"
                            style={{width: "20%"}}
                            multiple
                            onChange={e => console.log(e.currentTarget.value)}>
                        <For each={tags} onEach={t => <option key={t}>{t}</option>} />
                    </select>
                </div>
            </div>
            <button className="button blue" onClick={onClickSubmit}>Save Mapping</button>
        </form>
    )
};
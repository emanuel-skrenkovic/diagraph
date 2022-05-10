import React, { useState, FormEvent } from 'react';

import { RuleForm } from 'modules/import-events';
import { For, Tag } from 'modules/common';
import { EventTag, Rule, TemplateHeaderMapping } from 'types';

export interface HeaderMappingFormProps {
    value?: TemplateHeaderMapping;
    onSubmit: (template: TemplateHeaderMapping) => void;
    tags: EventTag[];
}

const DEFAULT_MAPPING = {
    header: '',
    rules: [],
    tags: []
};

export const HeaderMappingForm: React.FC<HeaderMappingFormProps> = ({ value, onSubmit, tags }) => {
    const [selectedTags, setSelectedTags] = useState<EventTag[]>(tags ?? []);

    const [editingRuleId, setEditingRuleId] = useState<string | undefined>(undefined);
    const [template, setTemplate] = useState<TemplateHeaderMapping>(value ?? DEFAULT_MAPPING);

    function onClickSubmit(e: FormEvent<HTMLButtonElement>) {
        e.preventDefault();
        onSubmit(template);
        setTemplate(DEFAULT_MAPPING);
    }

    function setTemplateTags(tags: EventTag[]) {
        setTemplate({
            ...template,
            tags
        });
    }

    function setTagSelection(tagName: string) {
        console.log(tagName);
        // setSelectedTags([]);
    }

    function renderRule(rule: Rule, index: number) {
        if (editingRuleId === rule.expression) {
            return (
                <RuleForm key={index}
                          initial={rule}
                          onSubmit={r => {
                              rule.expression = r.expression;
                              setEditingRuleId(undefined);
                          }} />
            );
        } else {
            return (
                <tr key={index}>
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
                    <select multiple onChange={e => setTagSelection(e.currentTarget.value)}>
                        <For each={tags} onEach={(tag, index) => (
                            <option key={index}>{tag.name}</option>
                        )} />
                    </select>
                    <button className="button blue"
                            onClick={e => {
                                e.preventDefault();
                                setTemplateTags([...template.tags, ...selectedTags]);
                                setSelectedTags([]); }}>
                        Add
                    </button>
                </div>
                <h3>Tags</h3>
                <For each={template.tags ?? []} onEach={(tag, index) => (
                    <div className="container" key={index}>
                        <Tag value={tag.name} onChange={changed => {
                            const updated = [...template.tags];
                            updated[index] = { name: changed } as EventTag;

                            setTemplateTags(updated);
                        }} />
                        <button className="button blue"
                                onClick={e => {
                                    e.preventDefault();
                                    const updated = [...template.tags];
                                    updated.splice(index, 1);

                                    setTemplateTags(updated);
                                }}>
                            X
                        </button>
                    </div>
                )} />
                <button className="button blue" onClick={(e) => {
                    e.preventDefault();
                    setTemplateTags([...template.tags, { name: '' } as EventTag]);
                }}>
                    +
                </button>
            </div>
            <button className="button blue" onClick={onClickSubmit}>Save Mapping</button>
        </form>
    )
};
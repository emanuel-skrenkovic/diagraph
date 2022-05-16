// noinspection JSDeprecatedSymbols

import React, { useState, FormEvent } from 'react';

import { For, Tag, MultiSelectForm } from 'modules/common';
import { RuleForm } from 'modules/import-events';
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
    const [editingRuleId, setEditingRuleId] = useState<number>(-1);
    const [template, setTemplate] = useState<TemplateHeaderMapping>(value ?? DEFAULT_MAPPING);

    function onClickSubmit(e: FormEvent<HTMLButtonElement>) {
        e.preventDefault();
        onSubmit(template);
        setTemplate(DEFAULT_MAPPING);
    }

    function addSelectedTags(selectedTags: EventTag[]) {
        const tagsToAdd = selectedTags.filter(
            t => !template.tags.map(t => t.name).includes(t.name)
        );
        setTemplateTags([...template.tags, ...tagsToAdd]);
    }

    function setTemplateTags(tags: EventTag[]) {
        setTemplate({
            ...template,
            tags
        });
    }

    function onSaveRule(rule: Rule, index: number) {
        const newRules = [...template.rules];
        newRules[index] = rule

        setTemplateRules(newRules);
        setEditingRuleId(-1);
    }

    function onAddRule(rule: Rule) {
        setTemplateRules([...template.rules, rule]);
        setEditingRuleId(-1);
    }

    function setTemplateRules(rules: Rule[]) {
        setTemplate({
            ...template,
            rules
        })
    }

    function onTagChanged(newValue: string, index: number) {
        const updated = [...template.tags];
        updated[index] = { name: newValue } as EventTag;

        setTemplateTags(updated);
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
                    <For each={template.rules} onEach={(rule, index) => (
                        <div className="container" key={index}>
                            <RuleForm value={rule}
                                      onSubmit={newValue => onSaveRule(newValue, index)}
                                      disabled={editingRuleId !== index}
                                      buttonText="Save" />
                            <button className="button blue" onClick={e => {
                                e.preventDefault();
                                setEditingRuleId(editingRuleId === index ? -1 : index);
                            }}>
                                {editingRuleId === index ? 'Close' : 'Edit'}
                            </button>
                            <button className="button blue"
                                    onClick={e => {
                                        e.preventDefault();
                                        const updated = [...template.rules];
                                        updated.splice(index, 1);

                                        setTemplateRules(updated);
                                    }}>
                                Remove
                            </button>
                        </div>
                    )} />
                    {editingRuleId !== -1 && <button className="button blue">New</button>}
                    {editingRuleId === -1 && <RuleForm key={-1} onSubmit={onAddRule} />}
                </div>
                <div className="item container horizontal">
                    <label htmlFor="selectTags">Tags</label>
                    <MultiSelectForm options={tags}
                                     keySelector={(t: EventTag) => t.name}
                                     onAdd={tags => addSelectedTags(tags)} />
                </div>
                <h3>Tags</h3>
                <For each={template.tags ?? []} onEach={(tag, index) => (
                    <div className="container" key={index}>
                        <Tag value={tag.name} onChange={newValue => onTagChanged(newValue, index)} />
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
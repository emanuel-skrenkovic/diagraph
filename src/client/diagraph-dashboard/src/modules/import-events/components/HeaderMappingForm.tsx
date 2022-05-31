import React, { useState, useEffect, FormEvent } from 'react';

import { RuleForm } from 'modules/import-events';
import { EventTag, Rule, TemplateHeaderMapping } from 'types';
import { Box, Container, Item, For, TagSelector } from 'modules/common';

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

export const HeaderMappingForm: React.FC<HeaderMappingFormProps> = ({ value, onSubmit }) => {
    const [editingRuleId, setEditingRuleId] = useState<number>(-1);
    const [template, setTemplate] = useState<TemplateHeaderMapping>(value ?? DEFAULT_MAPPING);

    useEffect(() => setTemplate(value ?? DEFAULT_MAPPING), [value]);

    function onClickSubmit(e: FormEvent<HTMLButtonElement>) {
        e.preventDefault();
        onSubmit(template);
        setTemplate(DEFAULT_MAPPING);
    }

    function setTemplateTags(tags: EventTag[]) {
        setTemplate({ ...template, tags });
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
        setTemplate({ ...template, rules })
    }

    return (
        <Container vertical>
        <Box>
            <Container vertical>
                <Item>
                    <label htmlFor="headerName">Header</label>
                    <input id="headerName"
                           type="text"
                           value={template.header}
                           onChange={e => setTemplate({ ...template, header: e.currentTarget.value })}/>
                </Item>
                <Item>
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
                </Item>
                <TagSelector initialSelectedTags={template.tags} onChange={setTemplateTags} />
            </Container>
            <button className="button blue" onClick={onClickSubmit}>Save Mapping</button>
        </Box>
        </Container>
    )
};
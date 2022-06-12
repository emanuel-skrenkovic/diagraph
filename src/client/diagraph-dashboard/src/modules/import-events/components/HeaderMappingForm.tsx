import React, { useState, useEffect, FormEvent } from 'react';

import { RedButton, BlueButton, Centered, Container, Input, Item } from 'styles';
import { RuleForm } from 'modules/import-events';
import { EventTag, Rule, TemplateHeaderMapping } from 'types';
import { For, TagSelector } from 'modules/common';

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

    function removeTemplateRule(e: FormEvent<HTMLButtonElement>, index: number) {
        e.preventDefault();
        const updated = [...template.rules];
        updated.splice(index, 1);

        setTemplateRules(updated);
    }

    return (
        <Container vertical>
            <Item>
                <label htmlFor="headerName">Header</label>
                <Input id="headerName"
                       type="text"
                       value={template.header}
                       onChange={e => setTemplate({ ...template, header: e.currentTarget.value })}/>
            </Item>
            <Item>
                <label>Rules</label>
                <For each={template.rules} onEach={(rule, index) => (
                    <Container key={index}>
                        <RuleForm value={rule}
                                  onSubmit={newValue => onSaveRule(newValue, index)}
                                  disabled={editingRuleId !== index} />
                        <BlueButton onClick={() => setEditingRuleId(editingRuleId === index ? -1 : index)}>
                            {editingRuleId === index ? 'Close' : 'Edit'}
                        </BlueButton>
                        {editingRuleId !== index && (
                            <RedButton onClick={e => removeTemplateRule(e, index)}>
                                Remove
                            </RedButton>
                        )}
                    </Container>
                )} />
                {editingRuleId !== -1 && <BlueButton>New</BlueButton>}
                {editingRuleId === -1 && <RuleForm key={-1} onSubmit={onAddRule} />}
            </Item>

            <TagSelector initialSelectedTags={template.tags} onChange={setTemplateTags} />
            <Centered>
                <BlueButton onClick={onClickSubmit}>Save Mapping</BlueButton>
            </Centered>
        </Container>
    )
};
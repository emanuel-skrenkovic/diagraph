import React, { useState, useEffect, FormEvent } from 'react';

import { DangerButton, PrimaryButton, Centered, Container, Input, Item } from 'diagraph/styles';
import { RuleForm } from 'diagraph/app/modules/import-events';
import { For, TagSelector } from 'diagraph/app/modules/common';
import { EventTag, Rule, TemplateHeaderMapping } from 'diagraph/app/types';

const DEFAULT_MAPPING = {
    header: '',
    rules: [],
    tags: []
};

const DEFAULT_RULE: Rule = { expression: '' };

export type HeaderMappingFormProps = {
    value?: TemplateHeaderMapping;
    onSubmit: (template: TemplateHeaderMapping) => void;
    tags: EventTag[];
}

export const HeaderMappingForm = ({ value, onSubmit }: HeaderMappingFormProps) => {
    const [editingRuleId, setEditingRuleId] = useState<number>(-1);
    const [template, setTemplate]           = useState<TemplateHeaderMapping>(value ?? DEFAULT_MAPPING);

    useEffect(() => setTemplate(value ?? DEFAULT_MAPPING), [value]);

    const onClickSubmit = (e: FormEvent<HTMLButtonElement>) => {
        e.preventDefault();
        onSubmit(template);
        setTemplate(DEFAULT_MAPPING);
    }

    const onSaveRule = (rule: Rule, index: number) => {
        const newRules = [...template.rules];
        newRules[index] = rule

        setEditingRuleId(-1);
        setTemplateRules(newRules);
    }

    const onAddRule = (rule: Rule) => {
        setEditingRuleId(-1);
        setTemplateRules([...template.rules, rule]);
    }

    const removeTemplateRule = (e: FormEvent<HTMLButtonElement>, index: number) => {
        e.preventDefault();
        const updated = [...template.rules];
        updated.splice(index, 1);

        setTemplateRules(updated);
    }

    const setTemplateRules = (rules: Rule[]) => setTemplate({ ...template, rules });
    const setTemplateTags  = (tags: EventTag[]) => setTemplate({ ...template, tags });
    const setEditingRule   = (index: number) => setEditingRuleId(editingRuleId === index ? -1 : index);

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
                        <PrimaryButton onClick={() => setEditingRule(index)}>
                            {editingRuleId === index ? 'Close' : 'Edit'}
                        </PrimaryButton>
                        {editingRuleId !== index && (
                            <DangerButton onClick={e => removeTemplateRule(e, index)}>
                                Remove
                            </DangerButton>
                        )}
                    </Container>
                )} />
                {editingRuleId === -1 && <RuleForm value={DEFAULT_RULE} key={-1} onSubmit={onAddRule} />}
            </Item>
            <TagSelector initialSelectedTags={template.tags} onChange={setTemplateTags} />
            <Centered>
                <PrimaryButton onClick={onClickSubmit}>Save Mapping</PrimaryButton>
            </Centered>
        </Container>
    )
};

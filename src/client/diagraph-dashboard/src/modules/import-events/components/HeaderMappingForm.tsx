import React, { useState, useEffect, FormEvent } from 'react';

import { BlueButton, Box, Container, Input } from 'styles';
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

    return (
        <Container vertical>
        <Box>
            <Container vertical>
                <label htmlFor="headerName">Header</label>
                <Input id="headerName"
                       type="text"
                       value={template.header}
                       onChange={e => setTemplate({ ...template, header: e.currentTarget.value })}/>
                <label>Rules</label>
                <For each={template.rules} onEach={(rule, index) => (
                    <Container key={index}>
                        <RuleForm value={rule}
                                  onSubmit={newValue => onSaveRule(newValue, index)}
                                  disabled={editingRuleId !== index}
                                  buttonText="Save" />
                        <BlueButton onClick={e => {
                            e.preventDefault();
                            setEditingRuleId(editingRuleId === index ? -1 : index);
                        }}>
                            {editingRuleId === index ? 'Close' : 'Edit'}
                        </BlueButton>
                        <BlueButton onClick={e => {
                            e.preventDefault();
                            const updated = [...template.rules];
                            updated.splice(index, 1);

                            setTemplateRules(updated);
                        }}>
                            Remove
                        </BlueButton>
                    </Container>
                )} />
                {editingRuleId !== -1 && <BlueButton>New</BlueButton>}
                {editingRuleId === -1 && <RuleForm key={-1} onSubmit={onAddRule} />}
                <TagSelector initialSelectedTags={template.tags} onChange={setTemplateTags} />
            </Container>
            <BlueButton onClick={onClickSubmit}>Save Mapping</BlueButton>
        </Box>
        </Container>
    )
};
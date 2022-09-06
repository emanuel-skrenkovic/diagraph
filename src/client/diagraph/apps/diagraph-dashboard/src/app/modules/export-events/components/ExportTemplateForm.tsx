import React, { useState, useEffect } from 'react';

import { Item, Divider, Box, Centered, PrimaryButton, Button, Container, Input } from 'diagraph/styles';
import { ExportTemplate } from 'diagraph/app/types';
import { MultiSelectForm } from 'diagraph/app/modules/common';

export type ExportTemplateFormProps = {
    value: ExportTemplate;
    onSubmit: (exportTemplate: ExportTemplate) => void;
    submitButtonText?: string;
};

export const ExportTemplateForm = ({ value, onSubmit, submitButtonText }: ExportTemplateFormProps) => {
    const [template, setTemplate]   = useState<ExportTemplate>(value);
    const [newHeader, setNewHeader] = useState('');

    useEffect(() => value && setTemplate(value), [value]);

    const onAddHeader = () => {
        if (template.data.headers.includes(newHeader)) return;
        setTemplateHeaders([...template.data.headers, newHeader]);
        setNewHeader('');
    };

    const onRemoveHeaderSelection = (selection: string[]) => {
        setTemplateHeaders(
            template.data.headers.filter(h => !selection.includes(h))
        );
    };

    const setTemplateHeaders = (headers: string[]) => setTemplate({ ...template, data: { headers } });

    return (
        <Box>
            <Container vertical>
                <label>Template name</label>
                <Input type="text" value={template.name}
                       onChange={e => setTemplate({...template, name: e.currentTarget.value})} />

                <label>Add template header</label>
                <Input type="text"
                       value={newHeader}
                       onChange={e => setNewHeader(e.currentTarget.value)} />
                <Button onClick={onAddHeader}>Add</Button>

                <MultiSelectForm options={template.data.headers}
                                 keySelector={h => h}
                                 onAdd={onRemoveHeaderSelection}
                                 buttonText="Remove" />
            </Container>

            <Centered>
                <PrimaryButton onClick={() => onSubmit(template)}>
                    {submitButtonText ?? 'Submit'}
                </PrimaryButton>
            </Centered>
        </Box>
    );
};

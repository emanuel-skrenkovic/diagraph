import React, { useState, useEffect, MouseEvent } from 'react';

import { Button, PrimaryButton, Box, Container, Input, Item, Title } from 'styles';
import { EventTag, ImportTemplate, TemplateHeaderMapping } from 'types';
import { HeaderMappingForm, HeaderMappingsTable } from 'modules/import-events';

export type ImportTemplateFormProps = {
    initial? : ImportTemplate;
    onSubmit: (t: ImportTemplate) => void;
    tags?: EventTag[];
}

const DEFAULT_MAPPING = {
    header: '',
    rules: [],
    tags: []
};

const DEFAULT_TEMPLATE = {
    name: '',
    data: { headerMappings: [] as TemplateHeaderMapping[] }
} as ImportTemplate;

export const ImportTemplateForm = ({ initial, onSubmit, tags }: ImportTemplateFormProps) => {
    const [template, setTemplate]             = useState(initial ?? DEFAULT_TEMPLATE);
    const [editingMapping, setEditingMapping] = useState<TemplateHeaderMapping | undefined>();

    useEffect(() => setTemplate(initial ?? DEFAULT_TEMPLATE), [initial]);

    const onClickSubmit = (e: MouseEvent<HTMLButtonElement>) => {
        e.preventDefault();
        onSubmit(template);
    }

    const setTemplateName = (name: string) => setTemplate({ ...template, name });

    const onSaveMapping = (t: TemplateHeaderMapping) => {
        const index = editingMapping && template.data.headerMappings
            ? template.data.headerMappings.indexOf(editingMapping)
            : -1;

        const initialMappings = template.data.headerMappings ?? [];
        const newMappings = [...initialMappings];
        if (index > -1) {
            newMappings[index] = t;
        } else {
            newMappings.push(t);
        }

        setTemplateMappings(newMappings);
        setEditingMapping(undefined);
    }

    const removeMapping = (mapping: TemplateHeaderMapping) => {
        setTemplateMappings(
            template
                .data
                .headerMappings
                .filter(m => m !== mapping)
        );
    }

    const setTemplateMappings = (newMappings: TemplateHeaderMapping[]) => {
        setTemplate({
            ...template,
            data: {
                ...template.data,
                headerMappings: newMappings
            }}
        );
    }

    return (
        <Container vertical>
            <Container>
                <label htmlFor="templateNameInput">Template Name</label>
                <Input id="templateNameInput"
                       type="text"
                       value={template.name}
                       onChange={e => setTemplateName(e.currentTarget.value)} />
                <PrimaryButton type="submit" onClick={onClickSubmit}>Save Template</PrimaryButton>
            </Container>
            <div style={{width:"10%", float:"right"}}>
                <PrimaryButton onClick={() => setEditingMapping(DEFAULT_MAPPING)}
                               disabled={!!editingMapping}>
                    New Mapping
                </PrimaryButton>
            </div>
            <Container>
                <Item style={{width:"100%"}}>
                    <Title>Mappings</Title>
                    <HeaderMappingsTable values={template.data.headerMappings ?? []}
                                         onEdit={setEditingMapping}
                                         onRemove={removeMapping} />
                </Item>
                {editingMapping &&
                    <Item as={Box}>
                        <Button onClick={() => setEditingMapping(undefined)}>Close</Button>
                        <HeaderMappingForm value={editingMapping}
                                           onSubmit={onSaveMapping}
                                           tags={tags ?? []} />
                    </Item>
                }
            </Container>
        </Container>
    );
};
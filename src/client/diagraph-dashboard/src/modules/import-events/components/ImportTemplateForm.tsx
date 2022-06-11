import React, { useState, MouseEvent } from 'react';

import { BlueButton, Container, Input, Item } from 'styles';
import { For } from 'modules/common';
import { HeaderMappingForm } from 'modules/import-events';
import { EventTag, ImportTemplate, TemplateHeaderMapping } from 'types';

export interface ImportTemplateFormProps {
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
    data: {
        headerMappings: [] as TemplateHeaderMapping[]
    }
} as ImportTemplate;

export const ImportTemplateForm: React.FC<ImportTemplateFormProps> = ({ initial, onSubmit, tags }) => {
    const [template, setTemplate]                         = useState(initial ?? DEFAULT_TEMPLATE);
    const [editingHeaderMapping, setEditingHeaderMapping] = useState<TemplateHeaderMapping | undefined>();

    function onClickSubmit(e: MouseEvent<HTMLButtonElement>) {
        e.preventDefault();
        onSubmit(template);
    }

    function setTemplateName(name: string) {
        setTemplate({
            ...template,
            name
        });
    }

    function onSaveMapping(t: TemplateHeaderMapping) {
        const index = editingHeaderMapping ?
            template.data.headerMappings.indexOf(editingHeaderMapping)
            : -1;

        const newMappings = [...template.data.headerMappings];
        if (index > -1) {
            newMappings[index] = t;
        } else {
            newMappings.push(t);
        }

        setTemplate({
            ...template,
            data: {
                ...template.data,
                headerMappings: newMappings
            }});
        setEditingHeaderMapping(undefined);
    }

    function removeHeaderMapping(mapping: TemplateHeaderMapping) {
        setTemplate({
            ...template,
            data: {
                ...template.data,
                headerMappings: template
                    .data
                    .headerMappings
                    .filter(m => m !== mapping)
            }
        });
    }

    return (
        <Container vertical>
            <Container>
                <label htmlFor="templateNameInput">Template Name</label>
                <Input id="templateNameInput"
                       type="text"
                       value={template.name}
                       onChange={e => setTemplateName(e.currentTarget.value)} />
                <BlueButton type="submit"
                            onClick={onClickSubmit}>
                    Save Template
                </BlueButton>
            </Container>
            <div style={{width:"10%", float:"right"}}>
                <BlueButton style={{width: "max-content", paddingLeft: "2em", paddingRight: "2em"}}
                        onClick={() => setEditingHeaderMapping(
                            !!editingHeaderMapping
                                ? undefined
                                : DEFAULT_MAPPING
                        )}>
                    {!!editingHeaderMapping ? 'Close' : 'New Mapping'}
                </BlueButton>
            </div>
            <Container>
                <Item style={{width:"100%"}}>
                    <h3>Mappings</h3>
                    <table>
                        <thead>
                        <tr>
                            <th>Header</th>
                            <th></th>
                        </tr>
                        </thead>
                        <tbody>
                        <For each={template.data.headerMappings} onEach={(mapping, index) => (
                            <tr key={index}>
                                <td>{mapping.header}</td>
                                <td>
                                    <BlueButton onClick={() => {setEditingHeaderMapping(mapping)}}>
                                        Edit
                                    </BlueButton>
                                    <BlueButton onClick={() => removeHeaderMapping(mapping)}>
                                        Remove
                                    </BlueButton>
                                </td>
                            </tr>
                        )} />
                        </tbody>
                    </table>
                </Item>
                {editingHeaderMapping &&
                    <Item as={HeaderMappingForm}
                          value={editingHeaderMapping}
                          onSubmit={onSaveMapping}
                          tags={tags ?? []} />
                }
            </Container>
        </Container>
    );
};
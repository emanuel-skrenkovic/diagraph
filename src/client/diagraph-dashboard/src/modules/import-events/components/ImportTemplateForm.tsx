import React, { useState, MouseEvent } from 'react';

import { For } from 'modules/common';
import { HeaderMappingForm, ImportTemplate, TemplateHeaderMapping } from 'modules/import-events';

export interface ImportTemplateFormProps {
    initial? : ImportTemplate;
    onSubmit: (t: ImportTemplate) => void;
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

export const ImportTemplateForm: React.FC<ImportTemplateFormProps> = ({ initial, onSubmit }) => {
    const [template, setTemplate] = useState(initial ?? DEFAULT_TEMPLATE);
    const [editingHeaderMapping, setEditingHeaderMapping] = useState<TemplateHeaderMapping | undefined>();

    function onClickSubmit(e: MouseEvent<HTMLButtonElement>) {
        e.preventDefault();
        onSubmit(template);
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

    return (
        <div className="container horizontal">
            <div className="container">
                <div className="item">
                    <label htmlFor="templateNameInput">Template Name</label>
                    <input id="templateNameInput"
                           type="text"
                           value={template.name}
                           onChange={e => setTemplate({
                               ...template,
                               name: e.currentTarget.value
                           })} />
                </div>
                <button className="button blue"
                        type="submit"
                        onClick={onClickSubmit}>
                    Save Template
                </button>
            </div>
            <div style={{width:"10%", float:"right"}}>
                <button className="button blue"
                        style={{width: "max-content", paddingLeft: "2em", paddingRight: "2em"}}
                        onClick={() => setEditingHeaderMapping(
                            !!editingHeaderMapping
                                ? undefined
                                : DEFAULT_MAPPING
                        )}>
                    {!!editingHeaderMapping ? 'Close' : 'New Mapping'}
                </button>
            </div>
            <div className="container">
                <div className="item" style={{width:"100%"}}>
                    <h3>Mappings</h3>
                    <table>
                        <thead>
                        <tr>
                            <th>Header</th>
                            <th></th>
                        </tr>
                        </thead>
                        <tbody>
                        <For each={template.data.headerMappings} onEach={m => (
                            <tr>
                                <td>{m.header}</td>
                                <td>
                                    <button className="button blue"
                                            onClick={() => {setEditingHeaderMapping(m)}}>
                                        Edit
                                    </button>
                                    <button className="button red"
                                            onClick={() =>
                                                setTemplate({
                                                    ...template,
                                                    data: {
                                                        ...template.data,
                                                        headerMappings: template
                                                            .data
                                                            .headerMappings
                                                            .filter(map => map !== m)
                                                    }
                                                })}>
                                        Remove
                                    </button>
                                </td>
                            </tr>
                        )} />
                        </tbody>
                    </table>
                </div>
                <div className="item" style={{width:"100"}}>
                    {editingHeaderMapping &&
                        <HeaderMappingForm value={editingHeaderMapping} onSubmit={onSaveMapping}/>
                    }
                </div>
            </div>
        </div>
    );
};
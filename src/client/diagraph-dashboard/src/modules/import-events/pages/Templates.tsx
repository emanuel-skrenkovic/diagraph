import React, { useState, FormEvent } from 'react';

import { For } from 'modules/common';
import { useCreateImportTemplateMutation } from 'services';
import { HeaderMappingForm, TemplateHeaderMapping } from 'modules/import-events';

import 'App.css';

export const Templates = () => {
    const [templateName, setTemplateName] = useState('');
    const [mappings, setMappings]         = useState<TemplateHeaderMapping[]>([]);

    const [editingHeaderMapping, setEditingHeaderMapping] = useState<TemplateHeaderMapping | undefined>()

    const [createImportTemplate] = useCreateImportTemplateMutation();

    function onSubmit(e: FormEvent<HTMLButtonElement>) {
        e.preventDefault();
        const template = {
            name: templateName,
            data : {
                headerMappings: mappings
            }
        };
        createImportTemplate(template);
    }

    return (
        <div className="container horizontal">
            <div className="container">
                <div className="item">
                    <label htmlFor="templateNameInput">Template Name</label>
                    <input id="templateNameInput"
                           type="text"
                           value={templateName}
                           onChange={e => setTemplateName(e.currentTarget.value)} />
                </div>
                <button className="button blue"
                        type="submit"
                        onClick={onSubmit}>
                    Create Template
                </button>
            </div>
            <div style={{width:"10%", float:"right"}}>
                <button className="button blue"
                        style={{width: "max-content", paddingLeft: "2em", paddingRight: "2em"}}
                        onClick={() => setEditingHeaderMapping(
                            !!editingHeaderMapping
                                ? undefined
                                : {} as TemplateHeaderMapping
                        )}>
                    {!!editingHeaderMapping ? 'Close' : 'New Mapping'}
                </button>
            </div>
            <div className="container">
                <div className="item" style={{width:"40%"}}>
                    <h3>Mappings</h3>
                    <table>
                        <thead>
                        <tr>
                            <th>Header</th>
                            <th></th>
                        </tr>
                        </thead>
                        <tbody>
                        <For each={mappings} onEach={m => (
                            <tr>
                                <td>{m.header}</td>
                                <td>
                                    <button className="button blue"
                                            onClick={() => {}}>
                                        Edit
                                    </button>
                                    <button className="button red"
                                            onClick={() => setMappings(mappings.filter(map => map !== m))}>
                                        Remove
                                    </button>
                                </td>
                            </tr>
                        )} />
                        </tbody>
                    </table>
                </div>
                <div className="item" style={{width:"70", float:"right"}}>
                    {editingHeaderMapping &&
                        <HeaderMappingForm onSubmit={t => setMappings([...mappings, t])}/>}
                </div>
            </div>
        </div>
    )
};
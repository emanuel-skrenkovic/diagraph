import React, { useState, FormEvent } from 'react';

import { For } from 'modules/common';
import { useCreateImportTemplateMutation } from 'services';
import { HeaderMappingForm, TemplateHeaderMapping } from 'modules/import-events';

import 'App.css';

export const Templates = () => {
    const [templateName, setTemplateName] = useState('');
    const [mappings, setMappings] = useState<TemplateHeaderMapping[]>([]);
    const [showHeaderMappingForm, setShowHeaderMappingForm] = useState(false);

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
                <button className="button blue"
                        onClick={() => setShowHeaderMappingForm(!showHeaderMappingForm)}>
                    New Mapping
                </button>
            </table>
            {showHeaderMappingForm &&
                <HeaderMappingForm onSubmit={t => setMappings([...mappings, t])}/>}

        </div>
    )
};
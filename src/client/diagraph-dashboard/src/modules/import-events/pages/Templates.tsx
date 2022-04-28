import React, { useState, FormEvent } from 'react';

import { useCreateImportTemplateMutation } from 'services';
import { HeaderMappingForm, TemplateHeaderMapping } from 'modules/import-events';

import 'App.css';

export const Templates = () => {
    const [templateName, setTemplateName] = useState('');
    const [mappings, setMappings] = useState<TemplateHeaderMapping[]>([]);

    const [createImportTemplate] = useCreateImportTemplateMutation();

    function onSubmit(e: FormEvent<HTMLButtonElement>) {
        e.preventDefault();
        const template = {
            name: templateName,
            template : {
                headerMappings: mappings
            }
        };
        createImportTemplate(template);
    }

    return (
        <div className="container horizontal">
            <div className="container horizontal">
                <div className="item">
                    <label htmlFor="templateNameInput">Template Name</label>
                    <input id="templateNameInput"
                           type="text"
                           value={templateName}
                           onChange={e => setTemplateName(e.currentTarget.value)} />
                </div>
                <HeaderMappingForm onSubmit={t => setMappings([...mappings, t])}/>
                <button className="button blue"
                        type="submit"
                        onClick={onSubmit}>
                    Create Template
                </button>
            </div>
        </div>
    )
};
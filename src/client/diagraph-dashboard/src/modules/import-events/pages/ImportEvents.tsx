import React, { useState } from 'react';
import { useNavigate } from 'react-router';

import { useGetImportTemplatesQuery } from 'services';
import { FileUploadForm,  For, Loader } from 'modules/common';
import { TemplateMappingPreview } from 'modules/import-events';

import 'App.css';

export const ImportEvents = () => {
    const { data, isLoading, isError, error } = useGetImportTemplatesQuery(undefined);

    const navigate = useNavigate();

    const [file, setFile]                         = useState<File | undefined>();
    const [selectedTemplate, setSelectedTemplate] = useState<string | undefined>(undefined);
    const [showPreview, setShowPreview]           = useState(false);

    async function onCheckTemplateMapping() {
        if (!file) return;
        setShowPreview(true);
    }

    function onEditTemplate() {
        const templateId = data?.find(t => t.name === selectedTemplate)?.id;
        if (!templateId) return;
        navigate(`/templates/edit?template_id=${templateId}`)
    }

    function onNewTemplate() {
        navigate('/templates/add');
    }

    if (isLoading) return <Loader />
    if (isError) console.error(error); // TODO

    return (
        <div className="container horizontal">
            <div className="container">
                <div className="item">
                    <FileUploadForm onSubmit={() => console.log('Uploading...')}
                                    onSelect={setFile}/>
                </div>
                <div className="item container horizontal">
                    <label htmlFor="selectTemplate">Templates</label>
                    <select id="selectTemplate"
                            value={selectedTemplate}
                            onChange={e => setSelectedTemplate(e.currentTarget.value)}>
                        <option key={undefined}></option>
                        <For each={data ?? []} onEach={({ name, id }) => (
                            <option key={id}>
                                {name}
                            </option>
                        )} />
                    </select>
                    <div className="container">
                        <button className={`button blue item ${!!selectedTemplate ? '' : 'disabled'}`}
                                onClick={onEditTemplate}>
                            Edit Template
                        </button>
                        <button className="button blue item" onClick={onNewTemplate}>
                            New Template
                        </button>
                    </div>
                    <button className="button blue item"
                            onClick={onCheckTemplateMapping}>
                        Check template mapping
                    </button>
                </div>
            </div>
            {showPreview && file && selectedTemplate && (
                <div className="box">
                    <TemplateMappingPreview
                        csvFile={file}
                        template={selectedTemplate}
                    />
                </div>
            )}
        </div>
    )
}
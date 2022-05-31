import React, { useState } from 'react';
import { useNavigate } from 'react-router';

import { TemplateMappingPreview } from 'modules/import-events';
import { useImportEventsMutation, useGetImportTemplatesQuery } from 'services';
import { Box, Container, Item, FileUploadForm,  For, Loader } from 'modules/common';

import 'App.css';

export const ImportEvents = () => {
    const [importEvents]                      = useImportEventsMutation();
    const { data, isLoading, isError, error } = useGetImportTemplatesQuery(undefined);

    const navigate = useNavigate();

    const [file, setFile]                         = useState<File | undefined>();
    const [selectedTemplate, setSelectedTemplate] = useState<string | undefined>();
    const [showPreview, setShowPreview]           = useState(false);

    function onUpload() {
        importEvents({ file, templateName: selectedTemplate }) ;
    }

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
        <Container vertical>
            <Container>
                <Item>
                    <FileUploadForm onSubmit={onUpload}
                                    onSelect={setFile} />
                </Item>
                <Item>
                <Container vertical>
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
                    <Container>
                        <button className={`button blue item ${!!selectedTemplate ? '' : 'disabled'}`}
                                onClick={onEditTemplate}>
                            Edit Template
                        </button>
                        <button className="button blue item" onClick={onNewTemplate}>
                            New Template
                        </button>
                    </Container>
                    <button className="button blue item" onClick={onCheckTemplateMapping}>
                        Check template mapping
                    </button>
                </Container>
                </Item>
            </Container>
            {showPreview && file && selectedTemplate && (
                <Box>
                    <TemplateMappingPreview
                        csvFile={file}
                        template={selectedTemplate}
                    />
                </Box>
            )}
        </Container>
    )
}
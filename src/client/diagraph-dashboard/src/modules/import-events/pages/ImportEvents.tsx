import React, { useState } from 'react';
import { useNavigate } from 'react-router';

import { BlueButton, Container, Item } from 'styles';
import { TemplateMappingPreview } from 'modules/import-events';
import { useImportEventsMutation, useGetImportTemplatesQuery } from 'services';
import { FileUploadForm,  For, Loader } from 'modules/common';

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
        <Container>
            <Container vertical>
                <Container>
                    <Item>
                        <FileUploadForm onSubmit={onUpload}
                                        onSelect={setFile} />
                    </Item>
                    <Container as={Item} vertical>
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
                            <Item as={BlueButton} disabled={!selectedTemplate}
                                  onClick={onEditTemplate}>
                                Edit Template
                            </Item>
                            <Item as={BlueButton} onClick={onNewTemplate}>
                                New Template
                            </Item>
                        </Container>
                        <Item as={BlueButton} onClick={onCheckTemplateMapping}>
                            Check template mapping
                        </Item>
                    </Container>
                </Container>
                {showPreview && file && selectedTemplate && (
                    <Container vertical style={{marginLeft:"10%",marginRight:"10%"}}>
                        <TemplateMappingPreview
                            csvFile={file}
                            template={selectedTemplate} />
                    </Container>
                )}
            </Container>
        </Container>
    )
}
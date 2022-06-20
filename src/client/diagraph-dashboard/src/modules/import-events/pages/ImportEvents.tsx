import React, { useState } from 'react';
import { useNavigate } from 'react-router';

import { PrimaryButton, Container, Item } from 'styles';
import { TemplateMappingPreview } from 'modules/import-events';
import { useImportEventsMutation, useGetImportTemplatesQuery } from 'services';
import { FileUploadForm, Loader, Options } from 'modules/common';

export const ImportEvents = () => {
    const [importEvents, { isLoading: isImportEventsLoading }]    = useImportEventsMutation();
    const { data: templates, isLoading: isImportTemplateLoading } = useGetImportTemplatesQuery(undefined);

    const [file, setFile]                         = useState<File | undefined>();
    const [selectedTemplate, setSelectedTemplate] = useState<string | undefined>();
    const [showPreview, setShowPreview]           = useState(false);

    const navigate = useNavigate();

    const onUpload = () => importEvents({ file, templateName: selectedTemplate });
    const onNewTemplate = () => navigate('/templates/add');
    const onCheckTemplateMapping = () => { if (file) setShowPreview(true) };

    const onEditTemplate = () => {
        const templateId = templates?.find(t => t.name === selectedTemplate)?.id;
        if (!templateId) return;
        navigate(`/templates/edit?template_id=${templateId}`)
    }

    if (isImportEventsLoading || isImportTemplateLoading) return <Loader />

    const previewLoaded = selectedTemplate && showPreview;

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
                            <Options elements={templates ?? []} value={t => t.name} />
                        </select>
                        <Container>
                            <Item as={PrimaryButton} disabled={!selectedTemplate}
                                  onClick={onEditTemplate}>
                                Edit Template
                            </Item>
                            <Item as={PrimaryButton} onClick={onNewTemplate}>
                                New Template
                            </Item>
                        </Container>
                        <Item as={PrimaryButton} onClick={onCheckTemplateMapping}>
                            Check template mapping
                        </Item>
                    </Container>
                </Container>
                {previewLoaded && (
                    <Container vertical style={{marginLeft:"10%",marginRight:"10%"}}>
                        <TemplateMappingPreview
                            csvFile={file!}
                            template={selectedTemplate} />
                    </Container>
                )}
            </Container>
        </Container>
    )
}
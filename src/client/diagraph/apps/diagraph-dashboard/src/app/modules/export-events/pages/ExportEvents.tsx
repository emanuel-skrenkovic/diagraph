import React, { useState, useEffect } from 'react';
import { useDispatch } from 'react-redux';

import { DangerButton, Button, Container, Item } from 'diagraph/styles';
import { AppDispatch } from 'diagraph/store';
import { ExportTemplate } from 'diagraph/app/types';
import { useToast, Options, Loader } from 'diagraph/app/modules/common';
import { ExportTemplateForm } from 'diagraph/app/modules/export-events';
import {
    useGetExportTemplateQuery, useGetExportTemplatesQuery, useCreateExportTemplateMutation,
    useUpdateExportTemplateMutation, useDeleteExportTemplateMutation, diagraphApi
} from 'diagraph/app/services';

export const ExportEvents = () => {
    // TODO: clean this up. The names are unclear.
    const [selectedTemplateObj, setSelectedTemplateObj] = useState<ExportTemplate>(DEFAULT_EXPORT_TEMPLATE);
    const [selectedTemplate, setSelectedTemplate]       = useState<string | undefined>();

    const showToast = useToast();
    const dispatch = useDispatch<AppDispatch>();

    const [templates, setTemplates] = useState<ExportTemplate[]>([]);
    const {
        data:      loadedTemplates,
        isLoading: isLoadingTemplates,
        isSuccess: isTemplatesSuccess } = useGetExportTemplatesQuery(undefined);

    const selectedTemplateId: number | undefined = templates
        ?.find(t => t.name === selectedTemplate)
        ?.id;
    const {
        data:      loadedTemplate,
        isLoading: isLoadingTemplate,
        isSuccess: isTemplateSuccess
    } = useGetExportTemplateQuery(selectedTemplateId!, { skip: !selectedTemplateId });

    const [
        createExportTemplate,
        { isLoading: isCreateTemplateLoading, isSuccess: isCreateTemplateSuccess }
    ] = useCreateExportTemplateMutation();
    const [
        updateExportTemplate,
        { isLoading: isUpdateTemplateLoading, isSuccess: isUpdateTemplateSuccess }
    ] = useUpdateExportTemplateMutation();
    const [
        deleteExportTemplate,
        { isLoading: isDeleteTemplateLoading, isSuccess: isDeleteTemplateSuccess }
    ] = useDeleteExportTemplateMutation();

    useEffect(() => {
        if (loadedTemplates) setTemplates(loadedTemplates);

        if (loadedTemplate && selectedTemplate) setSelectedTemplateObj(loadedTemplate);
        else                                    setSelectedTemplateObj(DEFAULT_EXPORT_TEMPLATE);

        // TODO: isSuccess stays true which shows the toast every time
        // the selected template is changed.
        if (isCreateTemplateSuccess) showToast('Successfully created new template');
        if (isUpdateTemplateSuccess) showToast('Successfully updated template');
        if (isDeleteTemplateSuccess) showToast('Successfully deleted template');
    }, [isTemplatesSuccess, isTemplateSuccess, isCreateTemplateSuccess,
        isUpdateTemplateSuccess, isDeleteTemplateSuccess, loadedTemplate, loadedTemplates]);

    if (isLoadingTemplates) return <Loader />

    const onExportEvents = () => {
        if (!selectedTemplate) {
            showToast('Please select template before exporting data.');
            return;
        }
        // TODO: there is, most certainly, a way better way.
        window.location.href = `https://localhost:7053/events/data-export/csv?mergeSequential=true&template=${selectedTemplate}`;
    }

    const onSubmitTemplate = (template: ExportTemplate) => {
        if (template.id) updateExportTemplate(template);
        else             createExportTemplate(template);

        dispatch(diagraphApi.endpoints.getExportTemplates.initiate(undefined));
    };

    const onDeleteTemplate = () => {
        if (selectedTemplateId) deleteExportTemplate(selectedTemplateId);
    }

    const elements = templates?.map(t => t.name) ?? [];
    elements.sort();

    const templateFormLoading = isLoadingTemplate
        || isCreateTemplateLoading
        || isUpdateTemplateLoading
        || isDeleteTemplateLoading;

    return (
        <Container>
            <Item>
                <Container vertical>
                    <Item>
                        <select style={{minWidth:"12em"}} value={selectedTemplate}
                                onChange={e => setSelectedTemplate(e.currentTarget.value)}>
                            <Options elements={elements} value={e => e} />
                        </select>
                    </Item>
                    <Item style={{marginLeft:"25%"}}>
                        <Button style={{whiteSpace:"nowrap"}}
                                disabled={!selectedTemplate}
                                onClick={onExportEvents}>
                            Export Events
                        </Button>
                    </Item>
                </Container>
            </Item>

            <Item>
                {templateFormLoading
                    ? <Loader />
                    : <>
                          <ExportTemplateForm value={selectedTemplateObj}
                                              onSubmit={onSubmitTemplate}
                                              submitButtonText={selectedTemplate ? 'Update' : 'Create'} />
                          <DangerButton onClick={onDeleteTemplate}>Delete</DangerButton>
                      </>
                }
            </Item>
        </Container>
    );
};

const DEFAULT_EXPORT_TEMPLATE: ExportTemplate = {
    name: '',
    data: { headers: [] }
};

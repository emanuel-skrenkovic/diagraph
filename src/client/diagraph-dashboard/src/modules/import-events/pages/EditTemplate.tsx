import React from 'react';

import { Loader, useQuery } from 'modules/common';
import { ImportTemplateForm } from 'modules/import-events';
import { useGetImportTemplateQuery, useUpdateImportTemplateMutation, useGetTagsQuery } from 'services';

export const EditTemplate = () => {
    const id = useQuery('template_id');

    const { data: tags, isLoading: isTagsLoading }                     = useGetTagsQuery(undefined);
    const { data: importTemplate, isLoading: isImportTemplateLoading } = useGetImportTemplateQuery(id);

    const [updateImportTemplate,
          { isLoading: isUpdateTemplateLoading }] = useUpdateImportTemplateMutation();

    if (!id) return null;
    if (isTagsLoading || isImportTemplateLoading || isUpdateTemplateLoading) return <Loader />

    return <ImportTemplateForm initial={importTemplate}
                               onSubmit={template => updateImportTemplate({template, id})}
                               tags={tags ?? []} />;
};
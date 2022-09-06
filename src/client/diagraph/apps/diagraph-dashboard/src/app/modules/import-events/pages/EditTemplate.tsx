import React from 'react';

import { Loader, useQuery } from 'diagraph/app/modules/common';
import { ImportTemplateForm } from 'diagraph/app/modules/import-events';
import { useGetImportTemplateQuery, useUpdateImportTemplateMutation, useGetTagsQuery } from 'diagraph/app/services';

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

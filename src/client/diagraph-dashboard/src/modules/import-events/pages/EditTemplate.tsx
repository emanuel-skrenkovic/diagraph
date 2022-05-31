import React from 'react';
import { useSearchParams } from 'react-router-dom';

import { handleQuery, Loader } from 'modules/common';
import { ImportTemplateForm } from 'modules/import-events';
import { useGetImportTemplateQuery, useUpdateImportTemplateMutation, useGetTagsQuery } from 'services';

export const EditTemplate: React.FC = () => {
    const [searchParams] = useSearchParams();
    const id = searchParams.get('template_id');

    const getTags           = useGetTagsQuery(undefined);
    const getImportTemplate = useGetImportTemplateQuery(id);

    const [updateImportTemplate] = useUpdateImportTemplateMutation();

    if (!id)                            return null;
    if (handleQuery(getTags))           return <Loader />
    if (handleQuery(getImportTemplate)) return <Loader />

    return <ImportTemplateForm initial={getImportTemplate.data}
                               onSubmit={template => updateImportTemplate({template, id})}
                               tags={getTags?.data ?? []} />
};
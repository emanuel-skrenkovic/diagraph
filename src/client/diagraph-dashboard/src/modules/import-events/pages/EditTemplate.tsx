import React from 'react';
import { useSearchParams } from 'react-router-dom';

import { Loader } from 'modules/common';
import { ImportTemplateForm } from 'modules/import-events';
import { useGetImportTemplateQuery, useUpdateImportTemplateMutation, useGetTagsQuery } from 'services';

export const EditTemplate: React.FC = () => {
    const [searchParams] = useSearchParams();
    const id = searchParams.get('template_id');

    const getImportTemplate = useGetImportTemplateQuery(id);
    const getTags           = useGetTagsQuery(undefined);

    const [updateImportTemplate] = useUpdateImportTemplateMutation();

    if (!id) return null;

    {
        const { isLoading, isError, error } = getImportTemplate;
        if (isLoading) return <Loader />
        if (isError) console.error(error);
    }

    {
        const { isLoading, isError, error } = getTags;
        if (isLoading) return <Loader />
        if (isError) console.error(error);
    }

    return <ImportTemplateForm initial={getImportTemplate.data}
                               onSubmit={template => updateImportTemplate({template, id})}
                               tags={getTags?.data ?? []} />
};
import React from 'react';
import { useSearchParams } from 'react-router-dom';

import { Loader } from 'modules/common';
import { useGetImportTemplateQuery, useUpdateImportTemplateMutation } from 'services';
import { ImportTemplateForm } from 'modules/import-events';

export const EditTemplate: React.FC = () => {
    const [searchParams] = useSearchParams();
    const id = searchParams.get('template_id');

    const { data, isLoading, isError, error } = useGetImportTemplateQuery(id);
    const [updateImportTemplate] = useUpdateImportTemplateMutation();

    if (!id) return null;

    if (isLoading) return <Loader />
    if (isError)   console.error(error);

    return <ImportTemplateForm initial={data}
                               onSubmit={template => updateImportTemplate({template, id})}/>
};
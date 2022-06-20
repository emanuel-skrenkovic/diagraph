import React from 'react';
import { Navigate } from 'react-router-dom';

import { Loader } from 'modules/common';
import { ImportTemplateForm } from 'modules/import-events';
import { useCreateImportTemplateMutation, useGetTagsQuery } from 'services';

export const Templates = () => {
    const [
        createImportTemplate,
        { data: templateId, isSuccess, isLoading: isCreateTemplateLoading }
    ] = useCreateImportTemplateMutation();
    const { data, isLoading } = useGetTagsQuery(undefined);

    if (isSuccess) return <Navigate to={`/templates/edit?template_id=${templateId}`} />
    if (isLoading || isCreateTemplateLoading) return <Loader />

    return <ImportTemplateForm onSubmit={createImportTemplate} tags={data ?? []} />;
};
import React from 'react';
import { Navigate } from 'react-router-dom';

import { Loader } from 'diagraph/app/modules/common';
import { ImportTemplateForm } from 'diagraph/app/modules/import-events';
import { useCreateImportTemplateMutation, useGetTagsQuery } from 'diagraph/app/services';

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

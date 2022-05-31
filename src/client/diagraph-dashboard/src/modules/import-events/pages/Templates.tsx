import React from 'react';
import { Navigate } from 'react-router-dom';

import { Loader } from 'modules/common';
import { ImportTemplateForm } from 'modules/import-events';
import { useCreateImportTemplateMutation, useGetTagsQuery } from 'services';

import 'App.css';

export const Templates = () => {
    const [createImportTemplate, { data: templateId, isSuccess }] = useCreateImportTemplateMutation();
    const { data, isLoading, isError, error }                     = useGetTagsQuery(undefined);

    if (isSuccess) {
        return <Navigate to={`/templates/edit?template_id=${templateId}`} />
    }

    if (isLoading) return <Loader />
    if (isError)   console.error(error); // TODO

    return <ImportTemplateForm onSubmit={createImportTemplate}
                               tags={data ?? []} />;
};
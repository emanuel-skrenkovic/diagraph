import React from 'react';

import { Loader } from 'modules/common';
import { ImportTemplateForm } from 'modules/import-events';
import { useCreateImportTemplateMutation, useGetTagsQuery } from 'services';

import 'App.css';

export const Templates = () => {
    const [createImportTemplate] = useCreateImportTemplateMutation();
    const { data, isLoading, isError, error } = useGetTagsQuery(undefined);

    if (isLoading) return <Loader />
    if (isError) console.error(error); // TODO

    return <ImportTemplateForm onSubmit={createImportTemplate}
                               tags={data ?? []} />;
};
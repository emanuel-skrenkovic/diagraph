import React from 'react';

import { useCreateImportTemplateMutation } from 'services';
import { ImportTemplateForm } from 'modules/import-events';

import 'App.css';

export const Templates = () => {
    const [createImportTemplate] = useCreateImportTemplateMutation();
    return<ImportTemplateForm onSubmit={createImportTemplate} />;
};
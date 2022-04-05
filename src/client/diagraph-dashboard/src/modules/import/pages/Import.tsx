import React from 'react';

import { Loader } from 'modules/common';
import { FileUploadForm } from 'modules/import';
import { useImportDataMutation } from 'services';

export const Import = () => {
    const [importData, { isLoading }] = useImportDataMutation(undefined);
    return isLoading ? <Loader /> : <FileUploadForm onSubmit={importData} />
};
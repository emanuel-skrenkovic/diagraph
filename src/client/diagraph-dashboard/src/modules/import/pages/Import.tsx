import React from 'react';

import { Loader } from 'modules/common';
import { useImportDataMutation } from 'services';
import { FileUploadForm } from 'modules/common';

export const Import = () => {
    const [importData, { isLoading }] = useImportDataMutation(undefined);
    if (isLoading) return <Loader />
    return (
        <FileUploadForm onSubmit={importData} />
    );
};
import React from 'react';

import { Loader } from 'diagraph/app/modules/common';
import { useImportDataMutation } from 'diagraph/app/services';
import { FileUploadForm } from 'diagraph/app/modules/common';

export const Import = () => {
    const [importData, { isLoading }] = useImportDataMutation(undefined);
    if (isLoading) return <Loader />
    return <FileUploadForm onSubmit={importData} />;
};

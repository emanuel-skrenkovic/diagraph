import React from 'react';

import { Loader } from 'modules/common';
import { FileUploadForm } from 'modules/common';
import { useImportDataMutation } from 'services';

export const Import = () => {
    const [importData, { isLoading }] = useImportDataMutation(undefined);

    if (isLoading) return <Loader />

    return (
        <div className="container">
            <FileUploadForm onSubmit={importData} />
        </div>
    );
};
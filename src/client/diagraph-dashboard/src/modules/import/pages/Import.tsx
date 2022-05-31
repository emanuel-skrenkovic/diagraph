import React from 'react';

import { Loader } from 'modules/common';
import { useImportDataMutation } from 'services';
import { Container, FileUploadForm } from 'modules/common';

export const Import = () => {
    const [importData, { isLoading }] = useImportDataMutation(undefined);

    if (isLoading) return <Loader />

    return (
        <Container>
            <FileUploadForm onSubmit={importData} />
        </Container>
    );
};
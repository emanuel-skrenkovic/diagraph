import React from 'react';

import { Loader } from 'diagraph/app/modules/common';

export type WithLoadingProps = {
    isLoading: boolean;
}

export function withLoading<P>(Component: React.ComponentType<P & WithLoadingProps>) {
    return (props: P & WithLoadingProps) => {
        return props.isLoading
            ? <Loader/>
            : <Component {...props} />;
    };
}

import React from 'react';

import 'App.css';

export interface ContainerProps {
    vertical?: boolean;
}

export const Container: React.FC<ContainerProps> = ({ vertical, children }) =>
    <div className={`container ${vertical  && 'vertical'}`}>{children}</div>;
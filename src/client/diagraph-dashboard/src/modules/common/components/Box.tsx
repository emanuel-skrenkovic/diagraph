import React from 'react';

import 'App.css';

export interface BoxProps {
    children?: React.ReactNode;
}

export const Box: React.FC<BoxProps> = ({ children }) => <div className="box">{children}</div>;
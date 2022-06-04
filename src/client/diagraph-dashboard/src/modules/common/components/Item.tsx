import React from 'react';

import 'App.css';

export interface ItemProps {
    children?: React.ReactNode;
}

export const Item: React.FC<ItemProps> = ({ children }) => <div className="item">{children}</div>;
import React from 'react';

import { Container } from 'modules/common';

export interface NavigationBarProps {
    children?: React.ReactNode;
}

export const NavigationBar: React.FC<NavigationBarProps> = ({ children }) => <Container>{children}</Container>;
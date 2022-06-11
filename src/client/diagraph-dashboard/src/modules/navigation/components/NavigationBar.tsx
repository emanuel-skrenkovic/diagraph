import React from 'react';

import { Container } from 'styles';

export interface NavigationBarProps {
    children?: React.ReactNode;
}

export const NavigationBar: React.FC<NavigationBarProps> = ({ children }) => <Container>{children}</Container>;
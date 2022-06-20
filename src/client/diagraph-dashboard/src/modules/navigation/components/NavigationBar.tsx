import React from 'react';

import { Container } from 'styles';

export type NavigationBarProps = {
    children?: React.ReactNode;
}

export const NavigationBar = ({ children }: NavigationBarProps) =>
    <Container>{children}</Container>;
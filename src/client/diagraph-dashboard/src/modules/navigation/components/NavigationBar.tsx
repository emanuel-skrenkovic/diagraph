import React from 'react';

export interface NavigationBarProps {
    
}

export const NavigationBar: React.FC<NavigationBarProps> = ({ children }) => {
    return (
        <div className="container">
            {children}
        </div>
    );
};
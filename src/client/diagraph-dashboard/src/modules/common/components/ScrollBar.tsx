import React from 'react';

export interface ScrollBarProps {
    heightPx?: number;
    widthPx?: number;
}

export const ScrollBar: React.FC<ScrollBarProps> = ({ heightPx, widthPx, children }) => {
    return (
        <div className="scrollBar"
              style={{
                  maxHeight: heightPx,
                  maxWidth: widthPx,
                  overflow: "scroll"
              }}>
            {children}
        </div>
    );
}
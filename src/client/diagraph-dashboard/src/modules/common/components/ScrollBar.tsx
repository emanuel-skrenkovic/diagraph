import React from 'react';

export interface ScrollBarProps {
    heightPx?: number;
    widthPx?: number;
}

export const ScrollBar: React.FC<ScrollBarProps> = ({ heightPx, widthPx, children }) => {
    return (
        <div className="scrollBar"
              style={{
                  height: heightPx,
                  width: widthPx,
                  overflow: "scroll"
              }}>
            {children}
        </div>
    );
}
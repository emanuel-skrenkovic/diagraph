import React from 'react';

export function Loader() {
    return (
        <svg width="50px" height="100px" xmlns="http://www.w3.org/2000/svg">
            <rect x="0" y="0" width="12" height="100" fill="#006fda94" fillOpacity="0.58">
                <animate attributeName="height"
                         values="100;10;100"
                         begin="0s"
                         dur="1.2s"
                         repeatCount="indefinite" />
                <animate attributeName="y"
                         values="0;50;0"
                         begin="0s"
                         dur="1.2s"
                         repeatCount="indefinite" />
            </rect>
            <rect x="20" y="0" width="12" height="100" fill="#006fdad9" fillOpacity="0.58">
                <animate attributeName="height"
                         values="100;10;100"
                         begin="0.2s"
                         dur="1.2s"
                         repeatCount="indefinite" />
                <animate attributeName="y"
                         values="0;50;0"
                         begin="0.2s"
                         dur="1.2s"
                         repeatCount="indefinite" />
            </rect>
            <rect x="40" y="0" width="12" height="100" fill="#006fdad9" fillOpacity="0.58">
                <animate attributeName="height"
                         values="100;10;100"
                         begin="0.4s"
                         dur="1.2s"
                         repeatCount="indefinite" />
                <animate attributeName="y"
                         values="0;50;0"
                         begin="0.4s"
                         dur="1.2s"
                         repeatCount="indefinite" />
            </rect>
        </svg>
    );
}
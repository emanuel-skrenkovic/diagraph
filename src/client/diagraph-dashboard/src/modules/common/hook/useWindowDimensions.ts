import { useState, useEffect } from 'react';

function windowDimensions() {
    const height = window.innerHeight;
    const width = window.innerWidth;

    return { height, width };
}

export const useWindowDimensions = () => {
    const inWindow = typeof window === 'object';

    const { height, width }           = windowDimensions();
    const [dimensions, setDimensions] = useState({ height, width })

    useEffect(() => {
        if (!inWindow) return;

        window.addEventListener(
            'resize',
            () => setDimensions(windowDimensions())
        );
    }, [inWindow]);

    return dimensions;
}
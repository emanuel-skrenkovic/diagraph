import { useState, useEffect } from 'react';

function windowDimensions() {
    const height = window.innerHeight;
    const width = window.innerWidth;

    return { height, width };
}

export const useWindowDimensions = () => {
    const inWindow = typeof window !== undefined;

    const { height, width } = windowDimensions();

    const [dimensions, setDimensions] = useState({ height, width })
    useEffect(() => {
        if (!inWindow) return;

        function onResize() {
            setDimensions(windowDimensions());
        }

        window.addEventListener('resize', onResize);
        return () => window.removeEventListener('resize', onResize); // TODO: I don't get this part, research!
    }, [inWindow]);


    return dimensions;
}
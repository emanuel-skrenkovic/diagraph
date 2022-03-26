import React, { useEffect, useRef } from 'react';
import * as d3 from 'd3';
import { GlucoseMeasurement } from 'types';

import "App.css";

export interface GlucoseGraphProps {
    points: GlucoseMeasurement[];
}

export const GlucoseGraph :React.FC<GlucoseGraphProps> = ({ points }) => {
    const curveElemRef = useRef(null);
    const data: [number, number][] = [[0, 20], [150, 150], [300, 100], [450, 20], [600, 130]];

    useEffect(() => {
        const svg = d3.select(curveElemRef.current).append("svg").attr("width", 800).attr("height", 200);

        // prepare a helper function
        const curveFunc = d3.line()
            .curve(d3.curveLinear)              // This is where you define the type of curve. Try curveStep for instance.
            .x(d => d[0])
            .y(d => d[1]);

        // Add the path using this helper function
        svg.append('path')
            .attr('d', curveFunc(data))
            .attr('stroke', 'black')
            .attr('fill', 'none');
    }, []);

    return (
        <div className="item">
            <h2>Glucose graph</h2>
            <div ref={curveElemRef} />
        </div>
    );
}
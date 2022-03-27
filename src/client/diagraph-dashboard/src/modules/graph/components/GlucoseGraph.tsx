import React, { useEffect, useRef } from 'react';
import * as d3 from 'd3';
import { GlucoseMeasurement } from 'types';

import "App.css";

export interface GlucoseGraphProps {
    from: Date,
    to: Date,
    points: GlucoseMeasurement[];
}

export const GlucoseGraph :React.FC<GlucoseGraphProps> = ({ from, to, points }) => {
    const chartElemRef = useRef(null);

    let pointData: [number, number][] = points.map(p => {
        const dateString = new Date(p.takenAt).toISOString();
        const dateNumber = d3.timeParse('%Y-%m-%dT%H:%M:%S.%LZ')(dateString);
        return [Number(dateNumber), p.level]
    });

    useEffect(() => {
        const svg = d3.select(chartElemRef.current)
            .append("svg")
            .attr("width", 720)
            .attr("height", 200);

        const x = d3.scaleTime()
            .domain([from, to])
            .range([600, 0]);
        svg.append("g")
            .attr("transform", "translate(0," + 200 + ")")
            .call(d3.axisBottom(x))
            .text('Time');

        const formatPercent = d3.format("0");

        const y = d3.scaleLinear()
            .domain([0, 15])
            .range([150, 0]);

        svg.append("g")
            .call(d3.axisLeft(y).tickFormat(formatPercent));

        svg.append("path")
            .datum(pointData)
            .attr("fill", "none")
            .attr("stroke", "#69b3a2")
            .attr("stroke-width", 1.2)
            .attr("d", d3.line()
                .x(d => x(d[0]))
                .y((d) => y(d[1]))
            );
        // Add the points
        svg.append("g")
            .selectAll("dot")
            .data(pointData)
            .join("circle")
            .attr("cx", d => x(d[0]))
            .attr("cy", d => y(d[1]))
            .attr("r", 1);
            //.attr("fill", "#69b3a2")
    }, [from, to]);

    return (
        <div className="item">
            <h2>Glucose graph</h2>
            <div ref={chartElemRef} />
        </div>
    );
}
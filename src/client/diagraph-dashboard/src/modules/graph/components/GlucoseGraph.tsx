import React, { useEffect, useRef } from 'react';
import * as d3 from 'd3';
import { GlucoseMeasurement } from 'types';

import "App.css";

export interface GlucoseGraphProps {
    from: Date,
    to: Date,
    points: GlucoseMeasurement[];
}

const MARGIN = { top: 20, bottom: 20, left: 20, right: 20 };
const PADDING = 5;
const WIDTH = 750 - MARGIN.left - MARGIN.right;
const HEIGHT = 300 - MARGIN.top - MARGIN.bottom;

export const GlucoseGraph :React.FC<GlucoseGraphProps> = ({ from, to, points }) => {
    const chartElemRef = useRef<HTMLDivElement>(null);

    let pointData: [number, number][] = points.map(p => {
        const dateString = new Date(p.takenAt).toISOString();
        const dateNumber = d3.timeParse('%Y-%m-%dT%H:%M:%S.%LZ')(dateString);
        return [Number(dateNumber), p.level]
    });

    useEffect(() => {
        if (chartElemRef?.current) {
            chartElemRef.current.innerHTML = '';
        }

        const svg = d3.select(chartElemRef.current)
            .append("svg")
            .attr("width", WIDTH + MARGIN.left + MARGIN.right + PADDING)
            .attr("height", HEIGHT + MARGIN.top + MARGIN.bottom + PADDING);

        const x = d3.scaleTime()
            .domain([from, to])
            .range([0, WIDTH - 2 * PADDING - MARGIN.top - MARGIN.bottom]);
        svg.append("g")
            .attr("transform", `translate(${MARGIN.left + PADDING}, ${HEIGHT + 2 * PADDING})`)
            .call(d3.axisBottom(x).ticks(12));

        svg.append("text")
            .attr("transform", `translate(${WIDTH / 2}, ${HEIGHT + MARGIN.top + MARGIN.bottom + PADDING})`)
            .style("text-anchor", "middle")
            .text("Date");

        const yFormat = d3.format("1");

        const y = d3.scaleLinear()
            .domain([0, 15])
            .range([HEIGHT - 2 * PADDING - MARGIN.top - MARGIN.bottom, 0]);

        svg.append("g")
            .attr('class', 'y axis')
            .attr('transform', `translate (${MARGIN.left + PADDING}, ${PADDING * 2 + MARGIN.top + MARGIN.bottom})`)
            .call(d3.axisLeft(y).ticks(15).tickFormat(yFormat));

        svg.append("path")
            .datum(pointData)
            .attr('transform', `translate(${PADDING * 3 + MARGIN.left + MARGIN.right}, ${PADDING * 2 + MARGIN.bottom + MARGIN.top})`)
            .attr("fill", "none")
            .attr("stroke", "#69b3a2")
            .attr("stroke-width", 1.2)
            .attr("d", d3.line()
                .x(d => x(d[0]))
                .y((d) => y(d[1]))
            );

        svg.append("g")
            .selectAll("dot")
            .data(pointData)
            .join("circle")
            .attr('transform', `translate(${PADDING * 3 + MARGIN.left + MARGIN.right}, ${PADDING * 2 + MARGIN.bottom + MARGIN.top})`)
            .attr("cx", d => x(d[0]))
            .attr("cy", d => y(d[1]))
            .attr("r", 2.5)
            .on('mouseover', (d, i) => {
                // TODO
            });

    }, [from, to]);

    return (
        <div className="item">
            <h2>Glucose graph</h2>
            <div ref={chartElemRef} />
        </div>
    );
}
import React, { useState, useEffect, useRef } from 'react';
import * as d3 from 'd3';
import { Event, GlucoseMeasurement } from 'types';

import "App.css";

interface Selected {
    level: number;
    date: Date;
}

export interface GlucoseGraphProps {
    from: Date,
    to: Date,
    points: GlucoseMeasurement[];
    events: Event[];
}

const MARGIN = { top: 20, bottom: 20, left: 20, right: 20 };
const PADDING = 5;
const WIDTH = 750 - MARGIN.left - MARGIN.right;
const HEIGHT = 300 - MARGIN.top - MARGIN.bottom;

function parseNumber(date: Date) {
    const dateString = new Date(date).toISOString();
    return Number(d3.timeParse('%Y-%m-%dT%H:%M:%S.%LZ')(dateString));
}

function horizontalLine(
    svg: d3.Selection<SVGSVGElement, unknown, null, undefined>,
    axis: d3.ScaleLinear<number, number>,
    at: number,
    color: string) {
    return svg.append('line')
        .attr('x1', MARGIN.left + PADDING)
        .attr('y1', axis(at) + PADDING * 2 + MARGIN.top + MARGIN.bottom)
        .attr('x2', WIDTH - MARGIN.right - PADDING)
        .attr('y2', axis(at) + PADDING * 2 + MARGIN.top + MARGIN.bottom)
        .style('stroke-width', 1)
        .style('stroke', color)
        .style('fill', 'black');
}

export const GlucoseGraph :React.FC<GlucoseGraphProps> = ({ from, to, points, events }) => {
    const chartElemRef = useRef<HTMLDivElement>(null);

    const [selectedMeasurement, setSelectedMeasurement] = useState<Selected | undefined>(undefined);
    const [selectedEvent, setSelectedEvent]             = useState<Event | undefined>(undefined);

    const [showOptions, setShowOptions]     = useState(false);
    const [showLowLimit, setShowLowLimit]   = useState(false);
    const [showHighLimit, setShowHighLimit] = useState(false);
    const [showAverage, setShowAverage]     = useState(false);

    const pointData: [number, number][] = points.map(p => [parseNumber(p.takenAt), p.level]);
    const eventData: [number, Event][]  = events.map(e => [parseNumber(e.occurredAtUtc), e]);

    useEffect(() => {
        if (chartElemRef?.current) {
            chartElemRef.current.innerHTML = '';
        }

        if (pointData.length <= 0 && eventData.length <= 0) {
            return;
        }

        const svg = d3.select(chartElemRef.current)
            .append("svg")
            .attr("width", WIDTH + MARGIN.left + MARGIN.right + PADDING)
            .attr("height", HEIGHT + MARGIN.top + MARGIN.bottom + PADDING);

        const x = d3.scaleTime()
            .domain([from, to])
            .range([0, WIDTH - 2 * PADDING - MARGIN.top - MARGIN.bottom]);

        // X label
        {
            const moveX = MARGIN.left + PADDING;
            const moveY = HEIGHT + 2 * PADDING;

            svg.append("g")
                .attr('transform', `translate(${moveX}, ${moveY})`)
                .call(d3.axisBottom(x).ticks(12));

            svg.append("text")
                .attr("transform", `translate(${WIDTH / 2}, ${HEIGHT + MARGIN.top + MARGIN.bottom + PADDING})`)
                .style("text-anchor", "middle")
                .text("Date");
        }

        const maxMeasurement = pointData.length > 0
            ? Math.max.apply(null, pointData.map(p => p[1]))
            : 12;
        const maxValue = maxMeasurement + 3;

        const y = d3.scaleLinear()
            .domain([0, maxValue])
            .range([HEIGHT - 2 * PADDING - MARGIN.top - MARGIN.bottom, 0]);

        // Y label
        {
            const yFormat = d3.format("1");

            const moveX = MARGIN.left + PADDING;
            const moveY = PADDING * 2 + MARGIN.top + MARGIN.bottom;
            svg.append("g")
                .attr('class', 'y axis')
                .attr('transform', `translate (${moveX}, ${moveY})`)
                .call(d3.axisLeft(y).ticks(maxValue).tickFormat(yFormat));
        }

        // Measurements
        {
            const moveX = MARGIN.left + PADDING;
            const moveY = PADDING * 2 + MARGIN.bottom + MARGIN.top;

            svg.append("path")
                .datum(pointData)
                .attr('transform', `translate(${moveX}, ${moveY})`)
                .attr("fill", "none")
                .attr("stroke", "#69b3a2")
                .attr("stroke-width", 1.2)
                .attr("d", d3.line()
                    .x(d => x(d[0]))
                    .y((d) => y(d[1]))
                );

            const circleRSmall = 2.5;
            const circleRBig = 10;

            svg.append('g')
                .selectAll('dot')
                .data(pointData)
                .join('circle')
                .attr('transform', `translate(${moveX}, ${moveY})`)
                .attr("cx", d => x(d[0]))
                .attr("cy", d => y(d[1]))
                .attr('r', circleRSmall)
                .on('mouseover', function () {
                    d3.select(this).attr('r', circleRBig);
                })
                .on('mouseout', function () {
                    d3.select(this).attr('r', circleRSmall);
                })
                .on('click', (d, i) => {
                    setSelectedMeasurement({date: new Date(i[0]), level: i[1]});
                });
        }

        if (showLowLimit)  horizontalLine(svg, y, 3.9, 'red');
        if (showHighLimit) horizontalLine(svg, y, 10, 'red');


        if (showAverage) {
            const sum = pointData.reduce((a, b) => a + b[1], 0);
            const average = (sum / pointData.length) || 0;

            horizontalLine(svg, y, average, 'blue');
        }

        // Events
        for (const event of eventData) {
            svg.append('line')
                .attr('x1', x(event[0]))
                .attr('y1', MARGIN.top + 2 * PADDING + MARGIN.top)
                .attr('x2', x(event[0]))
                .attr('y2', HEIGHT + 2 * PADDING)
                .style('stroke-width', 2.5)
                .style('stroke', 'green')
                .style('fill', 'black')
                .on('mouseover', function() {
                    d3.select(this).style('stroke-width', 5.5);
                })
                .on('mouseout', function(){
                    d3.select(this).style('stroke-width', 2);
                })
                .on('click', () => setSelectedEvent(event[1]));
        }
    }, [events, points, showLowLimit, showHighLimit, showAverage]);

    return (
        <div className="item">
            <h2>Glucose graph</h2>
            <button className="button" onClick={() => setShowOptions(!showOptions)}>
                {showOptions ? 'Close Options' : 'Show Options'}
            </button>
            {showOptions && (
                <div className="container horizontal">
                    <label>High limit line</label>
                    <input type="checkbox"
                           defaultChecked={showHighLimit}
                           onChange={() => setShowHighLimit(!showHighLimit)} />
                    <label>Low limit line</label>
                    <input type="checkbox"
                           defaultChecked={showLowLimit}
                           onChange={() => setShowLowLimit(!showLowLimit)} />
                    <label>Average</label>
                    <input type="checkbox"
                           defaultChecked={showAverage}
                           onChange={() => setShowAverage(!showAverage)} />
                </div>
            )}
            <div ref={chartElemRef} />
            <div className="container">
                {selectedMeasurement && (
                    <div className="container horizontal box item">
                        <button className="button"
                                onClick={() => setSelectedMeasurement(undefined)}>
                            x
                        </button>
                        <label>Date: </label>
                        <input disabled value={selectedMeasurement!.date.toLocaleString()} />
                        <label>Glucose mmol/L</label>
                        <input disabled value={selectedMeasurement!.level} />
                    </div>
                )}
                {selectedEvent && (
                    <div className="container horizontal box item">
                        <button className="button"
                                onClick={() => setSelectedEvent(undefined)}>
                            x
                        </button>
                        <label>Date: </label>
                        <input disabled value={selectedEvent!.occurredAtUtc.toLocaleString()} />
                        <textarea disabled value={selectedEvent!.text} />
                    </div>
                )}
            </div>
        </div>
    );
}
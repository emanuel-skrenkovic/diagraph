import React, { useState, useEffect, useRef } from 'react';
import * as d3 from 'd3';
import { TimeChart } from 'services';
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

        const chart = new TimeChart(chartElemRef, HEIGHT, WIDTH, MARGIN, PADDING);

        const maxMeasurement = pointData.length > 0
            ? Math.max.apply(null, pointData.map(p => p[1]))
            : 12;
        const maxValue = maxMeasurement + 3;

        const minMeasurement = pointData.length > 0
            ? Math.min.apply(null, pointData.map(p => p[1]))
            : 0;
        const minValue = minMeasurement > 2 ? minMeasurement - 2 : 0;

        chart.xAxis([from, to])
            .xLabel('Date')
            .yAxis([minValue, maxValue]) // TODO: think about the min value change
            .yLabel(maxValue)
            .data(pointData, (data) =>
                setSelectedMeasurement({
                    date: new Date(data[0] as Date),
                    level: data[1] as number}));

        if (showLowLimit)  chart.horizontalLine(3.9, 'red');
        if (showHighLimit) chart.horizontalLine(10, 'red');

        if (showAverage) {
            const sum = pointData.reduce((a, b) => a + b[1], 0);
            const average = (sum / pointData.length) || 0;

            chart.horizontalLine(average, 'blue');
        }

        for (const event of eventData) {
            chart.verticalLine(event[0], 'green', () => setSelectedEvent(event[1]));
        }

        chart.draw();
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
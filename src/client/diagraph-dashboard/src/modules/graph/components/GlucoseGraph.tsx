import * as d3 from 'd3';
import React, { useState, useEffect, useRef } from 'react';

import { useProfile } from 'modules/profile';

import { TimeChart } from 'services';
import { Event, GlucoseMeasurement } from 'types';
import { useWindowDimensions } from 'modules/common';

import "App.css";

export interface GlucoseGraphProps {
    from: Date,
    to: Date,
    points: GlucoseMeasurement[];
    events: Event[];
    onClickEvent: (event: Event) => void;
    onClickMeasurement: (measurement: GlucoseMeasurement) => void;
}

const MARGIN = { top: 20, bottom: 20, left: 20, right: 20 };
const PADDING = 5;
const HEIGHT = 300 - MARGIN.top - MARGIN.bottom;

function parseNumber(date: Date) {
    const dateString = new Date(date).toISOString();
    return Number(d3.timeParse('%Y-%m-%dT%H:%M:%S.%LZ')(dateString));
}

function getMinMax(pointData: [number, number][]) {
    const maxMeasurement = pointData.length > 0
        ? Math.max.apply(null, pointData.map(p => p[1]))
        : 12;

    const minMeasurement = pointData.length > 0
        ? Math.min.apply(null, pointData.map(p => p[1]))
        : 0;

    return { min: minMeasurement, max: maxMeasurement };
}

export const GlucoseGraph :React.FC<GlucoseGraphProps> = ({ from,
                                                            to,
                                                            points,
                                                            events,
                                                            onClickEvent,
                                                            onClickMeasurement }) => {
    const chartElemRef = useRef<HTMLDivElement>(null);

    const { width } = useWindowDimensions();

    const [showOptions, setShowOptions] = useState(false);
    const [profile, setProfile]         = useProfile();

    const pointData: [number, number][] = points.map(p => [parseNumber(p.takenAt), p.level]);
    const eventData: [number, Event][]  = events.map(e => [parseNumber(e.occurredAtUtc), e]);

    const { showLowLimit, showHighLimit, showAverage } = profile;

    useEffect(() => {
        if (chartElemRef?.current) {
            chartElemRef.current.innerHTML = '';
        }

        const chart = new TimeChart(chartElemRef, HEIGHT, width * 0.7, MARGIN, PADDING);

        const { min, max } = getMinMax(pointData);
        const maxValue = max + 3;
        const minValue = min > 2 ? min - 2 : 0;

        chart.xAxis([from, to])
            .xLabel('Date')
            .yAxis([minValue, maxValue]) // TODO: think about the min value change
            .yLabel(maxValue)
            .data(pointData, (data) =>
                onClickMeasurement({
                    takenAt: new Date(data[0] as Date),
                    level: data[1] as number } as GlucoseMeasurement));

        if (showLowLimit)  chart.horizontalLine(3.9, 'red');
        if (showHighLimit) chart.horizontalLine(10, 'red');

        if (pointData.length > 0 && showAverage) {
            const sum = pointData.reduce((a, b) => a + b[1], 0);
            const average = (sum / pointData.length) || 0;

            chart.horizontalLine(average, 'blue');
        }

        for (const event of eventData) {
            chart.verticalLine(event[0], 'green', () => onClickEvent(event[1]));
        }

        chart.draw();
    }, [from, to, showLowLimit, showHighLimit, showAverage, events, points, profile]);

    return (
        <>
            <button className="button" onClick={() => setShowOptions(!showOptions)}>
                {showOptions ? 'Close Options' : 'Show Options'}
            </button>
            {showOptions && (
                <div className="container">
                    <label>High limit line</label>
                    <input type="checkbox"
                           defaultChecked={profile.showHighLimit}
                           onChange={() => setProfile({...profile, showHighLimit: !showHighLimit})} />
                    <label>Low limit line</label>
                    <input type="checkbox"
                           defaultChecked={profile.showLowLimit}
                           onChange={() => setProfile({...profile, showLowLimit: !showLowLimit})} />
                    <label>Average</label>
                    <input type="checkbox"
                           defaultChecked={profile.showAverage}
                           onChange={() => setProfile({...profile, showAverage: !showAverage})} />
                </div>
            )}
            <div ref={chartElemRef} />
        </>
    );
}
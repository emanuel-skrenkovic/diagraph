import * as d3 from 'd3';
import React, { useState, useEffect, useRef } from 'react';

import { Button, Container, Input } from 'styles';
import { TimeChart } from 'services';
import { useProfile } from 'modules/profile';
import { Event, GlucoseMeasurement } from 'types';
import { useWindowDimensions } from 'modules/common';

export type GlucoseGraphProps = {
    from: Date,
    to: Date,
    points: GlucoseMeasurement[];
    events: Event[];
    onClickEvent: (event: Event) => void;
    onClickMeasurement: (measurement: GlucoseMeasurement) => void;
}

export const GlucoseGraph = ({ from, to, points, events,
                               onClickEvent, onClickMeasurement }: GlucoseGraphProps) => {
    const chartElemRef = useRef<HTMLDivElement>(null);

    const { width } = useWindowDimensions();

    const [showOptions, setShowOptions] = useState(false);
    const [profile, setProfile]         = useProfile();

    const pointData: [number, number][] = points.map(p => [parseNumber(p.takenAt), p.level]);
    const eventData: EventInfo[] = events.map(e => ({
            start: parseNumber(e.occurredAtUtc),
            end:   e.endedAtUtc ? parseNumber(e.endedAtUtc) : undefined,
            event: e
        }));

    const { showLowLimit, showHighLimit, showAverage } = profile;

    useEffect(() => {
        if (chartElemRef?.current) {
            chartElemRef.current.innerHTML = '';
        }

        const chart = new TimeChart(chartElemRef, HEIGHT, width * 0.7, MARGIN, PADDING);

        const {min, max} = getMinMax(pointData);
        const maxValue   = max + 3;
        const minValue   = min > 2 ? min - 2 : 0;

        chart.xAxis([from, to])
            .xLabel('Date')
            .yAxis([minValue, maxValue]) // TODO: think about the min value change
            .yLabel(maxValue)
            .data(pointData, (data) =>
                onClickMeasurement({
                    takenAt: new Date(data[0] as Date),
                    level: data[1] as number
                } as GlucoseMeasurement));

        if (showLowLimit) chart.horizontalLine(3.9, 'red');
        if (showHighLimit) chart.horizontalLine(10, 'red');

        if (pointData.length > 0 && showAverage) {
            const sum     = pointData.reduce((a, b) => a + b[1], 0);
            const average = (sum / pointData.length) || 0;

            chart.horizontalLine(average, 'blue');
        }

        for (const data of eventData) {
            chart.verticalLine(data.start, 'green', () => onClickEvent(data.event));

            if (data.end) {
                chart.verticalLine(data.end, 'green', () => onClickEvent(data.event));

                for (let i = minValue + .2; i < maxValue; i++)
                    chart.line(i, i + .33, data.start, data.end, 'green', 'dashed')
           }
        }

        chart.draw();
    }, [from, to, showLowLimit, showHighLimit, showAverage, events, points, profile,
        eventData, onClickEvent, onClickMeasurement, pointData, width]);

    return (
        <>
            <Button onClick={() => setShowOptions(!showOptions)}>
                {showOptions ? 'Hide Options' : 'Show Options'}
            </Button>
            {showOptions && (
                <Container>
                    <label>High limit line</label>
                    <Input type="checkbox"
                           defaultChecked={profile.showHighLimit}
                           onChange={() => setProfile({...profile, showHighLimit: !showHighLimit})} />
                    <label>Low limit line</label>
                    <Input type="checkbox"
                           defaultChecked={profile.showLowLimit}
                           onChange={() => setProfile({...profile, showLowLimit: !showLowLimit})} />
                    <label>Average</label>
                    <Input type="checkbox"
                           defaultChecked={profile.showAverage}
                           onChange={() => setProfile({...profile, showAverage: !showAverage})} />
                </Container>
            )}
            <div ref={chartElemRef} />
        </>
    );
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

type EventInfo = {
    start: number;
    end: number | undefined;
    event: Event ;
}
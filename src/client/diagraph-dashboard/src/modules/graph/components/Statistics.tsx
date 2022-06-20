import React from 'react';

import { Box, Container, Item } from 'styles'
import { GlucoseMeasurement } from 'types';

export type StatisticsProps = {
    measurements: GlucoseMeasurement[];
}

export const Statistics = ({ measurements }: StatisticsProps) => {
    const levels = measurements.map(m => m.level);
    const n = levels.length;

    const sum = levels.reduce((a, b) => a + b, 0);
    const avg = (sum / n) || 0;

    const med = median(levels);

    const stdDev = Math.sqrt(
        levels.reduce((a, b) => a + Math.pow(b - avg, 2), 0)
        /
        (n - 1)
    );

    const percentDeviation = (stdDev / avg) * 100;

    return (
        <Container vertical as={Box}>
            <Item>
                <span>Average:</span>
                <span>{avg.toFixed(1)}</span>
            </Item>
            <Item>
                <span>Median:</span>
                <span>{med.toFixed(1)}</span>
            </Item>
            <Item>
                <span>Deviation:</span>
                <span>{percentDeviation.toFixed(1)}%</span>
            </Item>
        </Container>
    );
};

function median(numbers: number[]) {
    const sorted = numbers.sort();
    const middle = Math.floor(sorted.length / 2);

    if (numbers.length % 2 === 0) {
        return (sorted[middle-1] + sorted[middle]) / 2;
    }

    return sorted[middle];
}
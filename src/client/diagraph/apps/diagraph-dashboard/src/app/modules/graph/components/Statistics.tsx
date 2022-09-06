import React from 'react';

import { Box, Container, Item } from 'diagraph/styles'
import { GlucoseMeasurement } from 'diagraph/app/types';

export type StatisticsProps = {
    measurements: GlucoseMeasurement[];
}

export const Statistics = ({ measurements }: StatisticsProps) => {
    const levels = measurements.map(m => m.level);
    const n = levels.length;

    const sum = levels.reduce((a, b) => a + b, 0);
    const mean = (sum / n) || 0;

    const med = median(levels);

    const stdDev = Math.sqrt(
        levels.reduce((a, b) => a + Math.pow(b - mean, 2), 0)
        /
        (n - 1)
    );

    const percentDeviation = (stdDev / mean) * 100;

    return (
        <Container vertical as={Box}>
            <Item>
                <span>Mean:</span>
                <span>{mean.toFixed(1)}</span>
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

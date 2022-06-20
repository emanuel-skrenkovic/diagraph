import React from 'react';
import { Container, Input } from 'styles';

import { GlucoseMeasurement } from 'types';

export type GlucoseMeasurementProps = { value: GlucoseMeasurement };

export const GlucoseMeasurementView = ({ value }: GlucoseMeasurementProps) => (
    <Container vertical>
        <label>Date: </label>
        <Input disabled value={value.takenAt.toLocaleString()} />
        <label>Glucose mmol/L</label>
        <Input disabled value={value.level} />
    </Container>
);
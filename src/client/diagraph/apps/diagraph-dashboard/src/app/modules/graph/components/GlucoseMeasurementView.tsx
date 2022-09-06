import React from 'react';
import { Container, Input } from 'diagraph/styles';

import { GlucoseMeasurement } from 'diagraph/app/types';

export type GlucoseMeasurementProps = { value: GlucoseMeasurement };

export const GlucoseMeasurementView = ({ value }: GlucoseMeasurementProps) => (
    <Container vertical>
        <label>Date: </label>
        <Input disabled value={value.takenAt.toLocaleString()} />
        <label>Glucose mmol/L</label>
        <Input disabled value={value.level} />
    </Container>
);

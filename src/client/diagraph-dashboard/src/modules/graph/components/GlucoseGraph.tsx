import React from 'react';
import { GlucoseMeasurement } from 'types';

import "App.css";

export interface GlucoseGraphProps {
    points: GlucoseMeasurement[];
}

export const GlucoseGraph :React.FC<GlucoseGraphProps> = ({ points }) => {
    return <div className="item">Hello from the glucose graph!</div>
}
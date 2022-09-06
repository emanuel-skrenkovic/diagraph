import React from 'react';

import { Box, Container, Item, Divider } from 'diagraph/styles';
import { useGetStatisticsQuery } from 'diagraph/app/services';
import { Loader, toLocalISODateString } from 'diagraph/app/modules/common';

export type GlucoseManagementIndicatorProps = { periodEndDate: Date; }

export const GlucoseManagementIndicator = ({ periodEndDate }: GlucoseManagementIndicatorProps) => {
    const [start, end] = dateLimits(periodEndDate, 14);
    const { data: biWeeklyData,
              isLoading: isBiWeeklyLoading } = useGetStatisticsQuery({ from: start, to: end });

    const [longStart, longEnd] = dateLimits(periodEndDate, 30)
    const { data, isLoading }  = useGetStatisticsQuery({ from: longStart, to: longEnd });

    if (isLoading || isBiWeeklyLoading) return <Loader />;

    return (
        <Container as={Box} vertical>
            <span>Glucose management indicator:</span>
            <Divider />
            <GmiData title={'Last 14 days:'}
                     mmolGmi={biWeeklyData.glucoseManagementIndicator}
                     percentGmi={biWeeklyData.glucoseManagementIndicatorPercentage} />
            <Divider />
            <GmiData title={'Last 30 days:'}
                     mmolGmi={data.glucoseManagementIndicator}
                     percentGmi={data.glucoseManagementIndicatorPercentage} />
        </Container>
    );
};

type GmiDataProps = {
    title: string;
    mmolGmi: number;
    percentGmi: number;
}

const GmiData = ({ title, mmolGmi, percentGmi }: GmiDataProps) => (
    <>
        <Item>
            <span><b>{title}</b></span>
        </Item>
        <Item>
            <span>{mmolGmi.toFixed(2)} mmol/L</span>
        </Item>
        <Item>
            <span>{percentGmi.toFixed(2)}%</span>
        </Item>
    </>
);

function dateLimits(endDate: Date, daysBack: number) {
    const end = new Date(endDate);
    end.setHours(0, 0, 0, 0);

    const start = new Date(end);
    start.setDate(start.getDate() - daysBack);
    start.setHours(0, 0, 0, 0);

    return [toLocalISODateString(start), toLocalISODateString(end)];
}

import React from 'react';

import { Box, Container, Item, Divider } from 'styles';
import { Loader, toLocalISODateString } from 'modules/common';
import { useGetStatisticsQuery} from 'services';

function dateLimits(endDate: Date, daysBack: number) {
    const end = new Date(endDate);
    end.setHours(0, 0, 0, 0);

    const start = new Date(end);
    start.setDate(start.getDate() - daysBack);
    start.setHours(0, 0, 0, 0);

    return [start, end];
}

export interface GlucoseManagementIndicatorProps {
    periodEndDate: Date;
}

export const GlucoseManagementIndicator: React.FC<GlucoseManagementIndicatorProps> = ({ periodEndDate }) => {
    const [start, end] = dateLimits(periodEndDate, 14);
    const { data: biWeeklyData, isLoading: isBiWeeklyLoading } = useGetStatisticsQuery({
        from: toLocalISODateString(start),
        to: toLocalISODateString(end)
    });

    const [longStart, longEnd] = dateLimits(periodEndDate, 30)
    const { data, isLoading } = useGetStatisticsQuery({
        from: toLocalISODateString(longStart),
        to: toLocalISODateString(longEnd)
    });

    function renderData(title: string, mmolGmi: number, percentGmi: number) {
        return (
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
    }

    if (isLoading || isBiWeeklyLoading) return <Loader />;
    return (
        <Container as={Box} vertical>
            <span>Glucose management indicator:</span>
            <Divider />
            {renderData(
                'Last 14 days:',
                biWeeklyData.glucoseManagementIndicator,
                biWeeklyData.glucoseManagementIndicatorPercentage
            )}
            <Divider />
            {renderData(
                'Last 30 days:',
                data.glucoseManagementIndicator,
                data.glucoseManagementIndicatorPercentage
            )}
        </Container>
    );
};
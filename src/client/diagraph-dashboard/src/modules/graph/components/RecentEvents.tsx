import React from 'react';

import { BlueButton, Box, ScrollBar } from 'styles';
import { Event } from 'types';
import { For, LocalTime } from 'modules/common';

export interface RecentEventsProps {
    events: Event[];
    onEdit?: (e: Event) => void
}

export const RecentEvents: React.FC<RecentEventsProps> = ({ events, onEdit }) => {
    const hasEvents = events.length > 0;
    return (
        <Box>
            <h3>Recent events:</h3>
            <table style={{tableLayout: "auto"}}>
                <thead>
                <tr>
                    {hasEvents && (
                        <>
                            <td></td>
                            <td></td>
                            <td></td>
                        </>
                    )}
                </tr>
                </thead>
                <tbody>
                {hasEvents && (
                    <For each={events} onEach={e => (
                        <tr key={e.id.toString()}>
                            <td className="box">
                                <LocalTime date={e.occurredAtUtc} />
                            </td>
                            <td className="block" style={{whiteSpace:"pre-line"}}>
                                <ScrollBar heightPx={125} widthPx={300}>
                                    {e.text}
                                </ScrollBar>
                            </td>
                            <td>
                                {onEdit && (
                                    <BlueButton onClick={() => onEdit(e)}>
                                        Edit
                                    </BlueButton>
                                )}
                            </td>
                        </tr>
                    )} />
                )}
                </tbody>
            </table>
        </Box>
    );
}
import React from 'react';

import { BlueButton, Box, Container, Item, ScrollBar } from 'styles';
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
            <table style={{tableLayout: "auto"}}>
                <thead>
                <tr>
                    {hasEvents && (<><td></td><td></td></>)}
                </tr>
                </thead>
                <tbody>
                <For each={events} onEach={e => (
                    <tr key={e.id.toString()}>
                        <td className="box">
                            <Container vertical>
                                <Item as={LocalTime} date={e.occurredAtUtc} />
                                {onEdit && (
                                    <Item as={BlueButton} onClick={() => onEdit(e)}>
                                        Edit
                                    </Item>
                                )}
                            </Container>
                        </td>
                        <td className="block" style={{whiteSpace:"pre-line",width:"100%"}}>
                            <ScrollBar style={{maxHeight:"125px"}}>{e.text}</ScrollBar>
                        </td>
                    </tr>
                )} />
                </tbody>
            </table>
        </Box>
    );
}
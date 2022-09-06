import React from 'react';

import { PrimaryButton, DangerButton, Divider, Box, Container, Item, ScrollBar } from 'diagraph/styles';
import { Event } from 'diagraph/app/types';
import { For, LocalTime } from 'diagraph/app/modules/common';

export type RecentEventsProps = {
    events: Event[];
    onEdit: (e: Event) => void
    onDelete: (e: Event) => void;
}

export const RecentEvents = ({ events, onEdit, onDelete }: RecentEventsProps) => (
    <Box>
        <table style={{tableLayout: "auto"}}>
            <thead>
            <tr>
                {events.length > 0 && (
                    <>
                        <td></td>
                        <td></td>
                    </>
                )}
            </tr>
            </thead>
            <tbody>
            <For each={events} onEach={e => (
                <tr key={e.id.toString()}>
                    <td className="box">
                        <Container vertical>
                            <Item as={LocalTime} date={e.occurredAtUtc} />
                            <Divider />
                            {e.endedAtUtc && <Item as={LocalTime} date={e.endedAtUtc} />}
                            <Item as={PrimaryButton} onClick={() => onEdit(e)}>Edit</Item>
                            <Item as={DangerButton} onClick={() => onDelete(e)}>Delete</Item>
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

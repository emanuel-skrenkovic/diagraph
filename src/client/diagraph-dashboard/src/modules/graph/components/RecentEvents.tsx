import React from 'react';

import { Event } from 'types';
import { For, LocalTime, ScrollBar } from 'modules/common';

import 'App.css';
import './RecentEvents.css';

export interface RecentEventsProps {
    events: Event[];
}

export const RecentEvents: React.FC<RecentEventsProps> = ({ events }) => {
    return (
        <div className="box">
            <h3>Recent events:</h3>
            <table className="table">
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
                                <LocalTime date={e.occurredAtUtc} />
                            </td>
                            <td className="block" style={{whiteSpace:"pre-line"}}>
                                <ScrollBar heightPx={125} widthPx={300}>
                                    {e.text}
                                </ScrollBar>
                            </td>
                        </tr>
                    )}
                />
                </tbody>
            </table>
        </div>
    );
}
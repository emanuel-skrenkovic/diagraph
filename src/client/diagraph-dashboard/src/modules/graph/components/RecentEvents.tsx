import React from 'react';
import { For, LocalTime } from 'modules/common';
import { Event } from 'types';

import 'App.css';
import './RecentEvents.css';

export interface RecentEventsProps {
    events: Event[];
    pageSize: number;
}

export const RecentEvents: React.FC<RecentEventsProps> = ({ events, pageSize }) => {
    return (
        <div className="box">
            <h3>Recent events:</h3>
            <table className="table">
                <thead>
                <tr>
                    <td>Occurred at</td>
                    <td>Text</td>
                </tr>
                </thead>
                <tbody>
                <For each={events}
                     onEach={e => (
                        <tr key={e.id.toString()}>
                            <td>
                                <LocalTime date={e.occurredAtUtc} />
                            </td>
                            <td>{e.text}</td>
                        </tr>
                    )}
                />
                </tbody>
            </table>
        </div>
    );
}
import React from 'react';
import { Event } from 'types';

import 'App.css';
import './RecentEvents.css';

export interface RecentEventsProps {
    events: Event[];
    pageSize: number;
}

function renderEvents(events: Event[]) {
    if (!events) return null;

    return events.map(e => (
        <tr id={e.id.toString()}>
            <td>{e.occurredAt}</td>
            <td>{e.text}</td>
        </tr>
    ));
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
                {renderEvents(events)}
                </tbody>
            </table>
        </div>
    );
}
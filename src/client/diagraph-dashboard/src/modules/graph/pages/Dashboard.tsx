import React from 'react';
import { useDispatch, useSelector } from 'react-redux';

import { Event } from 'types';
import { RootState } from 'store';
import { GlucoseGraph, EventForm, RecentEvents, setEvents } from 'modules/graph';
import { useCreateEventMutation, useGetEventsQuery } from 'services';

import 'App.css';

function graphDateLimits() {
    const today = new Date();
    today.setHours(0, 0, 0, 0);

    const tomorrow = new Date(today);
    tomorrow.setDate(tomorrow.getDate() + 1);
    today.setHours(0, 0, 0, 0);

    return { today, tomorrow };
}

function displayEvents(events: Event[]) {
    if (!events) return null;

    return events.map(e => (
        <div id={e.id.toString()}>
            <label>Event</label>
            {e.text}
        </div>
    ));
}

export function Dashboard() {
    const events = useSelector((state: RootState) => state.graph.events);
    const dispatch = useDispatch();
    const [createEvent, _] = useCreateEventMutation();

    const { today, tomorrow } = graphDateLimits();

    const { data, error, isLoading } = useGetEventsQuery({
        from: today.toISOString(),
        to: tomorrow.toISOString()
    });

    if (error) {
        console.error(error); // TODO
    }

    if (isLoading) {
        return <>I AM A PRETTY LOADER</>
    }

    if (data) {
        dispatch(setEvents(data));
    }

    return (
        <div className="container horizontal">
            <h1>Diagraph</h1>
            <GlucoseGraph points={[]} />
            <br/>
            <div className="container">
                <div className="item">
                    <RecentEvents events={events} pageSize={10} />
                </div>
                <div className="item">
                    <EventForm
                        initialValue={{
                            occurredAt: new Date(),
                            text: ''
                        } as Event}
                        onSubmit={createEvent}
                        submitButtonText="Create Event" />
                </div>
            </div>
        </div>
    );
}
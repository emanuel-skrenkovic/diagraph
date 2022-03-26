import React from 'react';
import { GlucoseGraph, EventForm } from 'modules/graph';
import { useCreateEventMutation } from 'services';

export function Dashboard() {
    const [createEvent, _] = useCreateEventMutation();
    return (
        <>
        <h2>Diagraph</h2>
        <GlucoseGraph />
        <EventForm onSubmit={e => createEvent(e)} submitButtonText="Create Event" />
        </>
    );
}
import React, { useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';

import { RootState } from 'store';
import { Event, EventTag, GlucoseMeasurement } from 'types';
import { setTags, Loader, DateRangePicker, toLocalISODateString } from 'modules/common';
import { GlucoseGraph, EventForm, RecentEvents, setDateRange, setData, setEvents } from 'modules/graph';
import {
    useCreateEventMutation,
    useUpdateEventMutation,
    useGetEventsQuery,
    useGetDataQuery,
    useGetProfileQuery,
    useGetTagsQuery } from 'services';

import 'App.css';

const EMPTY_EVENT = {
    occurredAtUtc: new Date(),
    text: '',
    tags: [] as EventTag[]
} as Event;

function toLocalDate(dateString: string): Date {
    const date = new Date(dateString);
    date.setHours(0, 0, 0, 0);
    return date;
}

export function Dashboard() {
    const events       = useSelector((state: RootState) => state.graph.events);
    const measurements = useSelector((state: RootState) => state.graph.data);
    const dateRange    = useSelector((state: RootState) => state.graph.dateRange);
    const tagsData     = useSelector((state: RootState) => state.shared.tags);

    const [editing, setEditing] = useState(false);
    const [selectedMeasurement, setSelectedMeasurement] = useState<GlucoseMeasurement | undefined>();
    const [selectedEvent, setSelectedEvent]             = useState<Event | undefined>();

    const [createEvent] = useCreateEventMutation();
    const [updateEvent] = useUpdateEventMutation();

    const dispatch = useDispatch();

    const getData    = useGetDataQuery({ from: dateRange.from, to: dateRange.to });
    const getEvents  = useGetEventsQuery({ from: dateRange.from,to: dateRange.to });
    const getTags    = useGetTagsQuery(undefined);
    const getProfile = useGetProfileQuery(undefined);

    {
        const { data, isLoading, isError, error } = getData;

        if (isError)   console.error(error); // TODO
        if (isLoading) return <Loader />;

        if (data) dispatch(setData(data));
    }

    {
        const { data, isLoading, isError, error } = getEvents;

        if (isError)   console.error(error);
        if (isLoading) return <Loader />;

        if (data) dispatch(setEvents(data));
    }

    {
        const { data, isLoading, isError, error} = getTags;

        if (isError)   console.error(error);
        if (isLoading) return <Loader />;

        if (data) dispatch(setTags(data));
    }

    {
        const { isLoading } = getProfile;
        if (isLoading) return <Loader />;
    }

    return (
        <div className="container vertical">
            <div className="container">
                <DateRangePicker
                    from={toLocalDate(dateRange.from)}
                    to={toLocalDate(dateRange.to)}
                    onSubmit={(from, to) => dispatch(setDateRange({
                        from: toLocalISODateString(from), to: toLocalISODateString(to)
                    }))}
                    submitButtonText="Apply dates" />
            </div>
            <div className="container">
                <div>
                    <h2>Glucose graph</h2>
                    <GlucoseGraph
                        from={toLocalDate(dateRange.from)}
                        to={toLocalDate(dateRange.to)}
                        points={measurements}
                        events={events}
                        onClickEvent={setSelectedEvent}
                        onClickMeasurement={setSelectedMeasurement} />
                </div>
            </div>
            <div className="container">
                <div className="item">
                    {selectedMeasurement && (
                        <div className="container vertical box item">
                            <button className="button"
                                    onClick={() => setSelectedMeasurement(undefined)}>
                                x
                            </button>
                            <label>Date: </label>
                            <input disabled value={selectedMeasurement!.takenAt.toLocaleString()} />
                            <label>Glucose mmol/L</label>
                            <input disabled value={selectedMeasurement!.level} />
                        </div>
                    )}
                    {selectedEvent ? (
                        <>
                            <div className="container vertical box item">
                                <button className="button centered" onClick={() => {
                                    setSelectedEvent(undefined);
                                    if (editing) setEditing(false);
                                }}>
                                    Close
                                </button>
                                <button className="button centered" onClick={() => setEditing(!editing)}>
                                    Edit
                                </button>
                                <EventForm
                                    value={selectedEvent}
                                    onSubmit={e => {
                                        updateEvent(e);
                                        setEditing(false);
                                    }}
                                    tagOptions={tagsData ?? []}
                                    submitButtonText="Save"
                                    disabled={!editing} />
                            </div>

                        </>
                    ) : (
                        <div className="item">
                            <EventForm
                                value={EMPTY_EVENT}
                                onSubmit={createEvent}
                                tagOptions={tagsData ?? []}
                                submitButtonText="Create Event" />
                        </div>
                    )}
                </div>
                <div className="item">
                    <RecentEvents events={events} />
                </div>
            </div>
        </div>
    );
}
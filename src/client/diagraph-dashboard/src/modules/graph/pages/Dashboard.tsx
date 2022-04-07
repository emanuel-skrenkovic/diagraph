import React, { useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';

import { RootState } from 'store';
import { Event, GlucoseMeasurement } from 'types';
import { Loader, DateRangePicker, toLocalISODateString } from 'modules/common';
import { GlucoseGraph, EventForm, RecentEvents, setDateRange, setData, setEvents } from 'modules/graph';
import {
    useCreateEventMutation,
    useGetEventsQuery,
    useGetDataQuery,
    useGetProfileQuery } from 'services';

import 'App.css';

function toLocalDate(dateString: string): Date {
    const date = new Date(dateString);
    date.setHours(0, 0, 0, 0);
    return date;
}

export function Dashboard() {
    const events       = useSelector((state: RootState) => state.graph.events);
    const measurements = useSelector((state: RootState) => state.graph.data);
    const dateRange    = useSelector((state: RootState) => state.graph.dateRange);

    const [selectedMeasurement, setSelectedMeasurement] = useState<GlucoseMeasurement | undefined>();
    const [selectedEvent, setSelectedEvent]             = useState<Event | undefined>();

    const [createEvent] = useCreateEventMutation();
    const { data: measurementData,
            isLoading: isDataLoading,
            isError: isDataError,
            error: dataError } = useGetDataQuery({ from: dateRange.from, to: dateRange.to });
    const { data: eventData,
            isLoading: isEventsLoading,
            isError: isEventsError,
            error: eventsError } = useGetEventsQuery({ from: dateRange.from, to: dateRange.to });
    const { isLoading: isProfileLoading } = useGetProfileQuery(undefined);

    const dispatch = useDispatch();

    if (isDataError)   console.error(dataError); // TODO
    if (isEventsError) console.error(eventsError);

    if (isDataLoading || isEventsLoading || isProfileLoading) return <Loader />;

    if (measurementData) dispatch(setData(measurementData));
    if (eventData)       dispatch(setEvents(eventData));

    return (
        <div className="container horizontal">
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
                {selectedMeasurement && (
                    <div className="container horizontal box item">
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
                {selectedEvent && (
                    <div className="container horizontal box item">
                        <button className="button"
                                onClick={() => setSelectedEvent(undefined)}>
                            x
                        </button>
                        <label>Date: </label>
                        <input disabled value={selectedEvent!.occurredAtUtc.toLocaleString()} />
                        <textarea disabled value={selectedEvent!.text} />
                    </div>
                )}
            </div>
            <div className="container">
                <div className="item">
                    <RecentEvents events={events} pageSize={10} />
                </div>
                <div className="item">
                    <EventForm
                        initialValue={{
                            occurredAtUtc: new Date(),
                            text: ''
                        } as Event}
                        onSubmit={createEvent}
                        submitButtonText="Create Event" />
                </div>
            </div>
        </div>
    );
}
import React, { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';

import { Event } from 'types';
import { RootState } from 'store';
import { Loader, DateRangePicker, toLocalISODateString } from 'modules/common';
import { GlucoseGraph, EventForm, RecentEvents, setEvents, setData, setDateRange } from 'modules/graph';
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

function moveDateRange(dateRange: { from: string, to: string}, n: number) {
    const from = new Date(dateRange.from);
    from.setHours(0, 0, 0, 0);
    from.setDate(from.getDate() + n);

    const to = new Date(dateRange.to);
    to.setDate(to.getDate() + n);
    to.setHours(0, 0, 0, 0);

    return { from: toLocalISODateString(from), to: toLocalISODateString(to) };
}

export function Dashboard() {
    const dateRange = useSelector((state: RootState) => state.graph.dateRange);

    const events    = useSelector((state: RootState) => state.graph.events);
    const pointData = useSelector((state: RootState) => state.graph.data);

    useEffect(() => {
    }, [dateRange]);

    const [createEvent] = useCreateEventMutation();

    const { data, error, isLoading }  = useGetDataQuery({
        from: dateRange.from,
        to:   dateRange.to
    });
    const {
        data: eventData,
        error: eventError,
        isLoading: isEventLoading
    } = useGetEventsQuery({
        from: dateRange.from,
        to:   dateRange.to
    });

    const { isLoading: isProfileLoading } = useGetProfileQuery(undefined);

    const dispatch = useDispatch();

    if (error)      console.error(error); // TODO
    if (eventError) console.error(eventError);

    if (isLoading || isEventLoading || isProfileLoading) return <Loader />;

    if (data)      dispatch(setData(data));
    if (eventData) dispatch(setEvents(eventData));

    return (
        <div className="container horizontal">
            <div className="container">
                <button
                    className="button"
                    onClick={() => dispatch(setDateRange(moveDateRange(dateRange, -1)))}>
                    &lt;
                </button>
                <DateRangePicker
                    from={toLocalDate(dateRange.from)}
                    to={toLocalDate(dateRange.to)}
                    onSubmit={(from, to) => dispatch(setDateRange({
                        from: toLocalISODateString(from), to: toLocalISODateString(to)
                    }))}
                    submitButtonText="Apply dates" />
                <button
                    className="button"
                    onClick={() => dispatch(setDateRange(moveDateRange(dateRange, 1)))}>
                    &gt;
                </button>
            </div>
            <div className="container">
                <GlucoseGraph
                    from={toLocalDate(dateRange.from)}
                    to={toLocalDate(dateRange.to)}
                    points={pointData}
                    events={events} />
            </div>
            <br/>
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
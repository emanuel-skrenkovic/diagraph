import React, { useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';

import { Event } from 'types';
import { RootState } from 'store';
import { GlucoseGraph, EventForm, RecentEvents, setEvents, setData } from 'modules/graph';
import { Loader, DateRangePicker } from 'modules/common';
import { useCreateEventMutation, useGetEventsQuery, useGetDataQuery } from 'services';

import 'App.css';

function toLocalISOString(date: Date) {
    return new Date(date.getTime() - (date.getTimezoneOffset() * 60000)).toISOString().split('T')[0];
}

function graphDateLimits() {
    const today = new Date();
    today.setHours(0, 0, 0, 0);

    const tomorrow = new Date(today);
    tomorrow.setDate(tomorrow.getDate() + 1);
    tomorrow.setHours(0, 0, 0, 0);

    return { today, tomorrow };
}

export function Dashboard() {
    const { today, tomorrow } = graphDateLimits();

    const [dateRange, setDateRange] = useState<{ from: Date, to: Date }>(
        { from: today, to: tomorrow }
    );

    const events = useSelector((state: RootState) => state.graph.events);
    const pointData = useSelector((state: RootState) => state.graph.data);

    const dispatch = useDispatch();
    const [createEvent, _] = useCreateEventMutation();

    const { data, error, isLoading }  = useGetDataQuery({
        from: toLocalISOString(dateRange.from),
        to: toLocalISOString(dateRange.to)
    });
    const {
        data: eventData,
        error: eventError,
        isLoading: isEventLoading
    } = useGetEventsQuery({
        from: toLocalISOString(dateRange.from),
        to: toLocalISOString(dateRange.to)
    });

    function moveDateRange(n: number) {
        const from = new Date(dateRange.from);
        from.setHours(0, 0, 0, 0);
        from.setDate(from.getDate() + n);

        const to = new Date(from);
        to.setDate(to.getDate() + 1);
        to.setHours(0, 0, 0, 0);

        return { from, to };
    }

    if (error) {
        console.error(error); // TODO
    }

    if (eventError) {
        console.error(eventError);
    }

    if (isLoading || isEventLoading) {
        return <Loader />
    }

    if (data)      dispatch(setData(data));
    if (eventData) dispatch(setEvents(eventData));

    return (
        <div className="container horizontal">
            <h1>Diagraph</h1>
            <div className="container">
                <button
                    className="button"
                    onClick={() => setDateRange(moveDateRange(-1))}>
                    &lt;
                </button>
                <DateRangePicker
                    from={dateRange.from}
                    to={dateRange.to}
                    onSubmit={(fromDate, toDate) => setDateRange({from: fromDate, to: toDate})}
                    submitButtonText="Apply dates" />
                <button
                    className="button"
                    onClick={() => setDateRange(moveDateRange(1))}>
                    &gt;
                </button>
            </div>
            <GlucoseGraph
                from={dateRange.from}
                to={dateRange.to}
                points={pointData}
                events={events} />
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
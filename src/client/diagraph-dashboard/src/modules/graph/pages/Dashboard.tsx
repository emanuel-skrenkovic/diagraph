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

    const [from, setFrom] = useState(toLocalISOString(today));
    const [to, setTo] = useState(toLocalISOString(tomorrow));

    const events = useSelector((state: RootState) => state.graph.events);
    const pointData = useSelector((state: RootState) => state.graph.data);

    const dispatch = useDispatch();
    const [createEvent, _] = useCreateEventMutation();

    const { data, error, isLoading }  = useGetDataQuery({from, to});
    const {
        data: eventData,
        error: eventError,
        isLoading: isEventLoading
    } = useGetEventsQuery({from, to});

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
            <DateRangePicker
                from={new Date(from)}
                to={new Date(to)}
                onSubmit={(fromDate, toDate) => { setFrom(toLocalISOString(fromDate)); setTo(toLocalISOString(toDate)); }}
                submitButtonText="Apply dates" />
            <GlucoseGraph
                from={new Date(from)}
                to={new Date(to)}
                points={pointData} />
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
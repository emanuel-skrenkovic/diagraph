import React from 'react';
import { useSelector, useDispatch } from 'react-redux';

import { RootState } from 'store';
import { useGetEventsQuery } from 'services';
import { setEvents } from 'modules/graph';

export function GlucoseGraph() {
    const today = new Date();
    today.setHours(0, 0, 0, 0);

    const tomorrow = new Date(today);
    tomorrow.setDate(tomorrow.getDate() + 1);
    today.setHours(0, 0, 0, 0);

    const { data, error, isLoading } = useGetEventsQuery({
        from: today.toISOString(),
        to: tomorrow.toISOString()
    });
    const events = useSelector((state: RootState) => state.graph.events);
    const dispatch = useDispatch();

    if (data) {
        dispatch(setEvents(data));
    }

    return (
        <>
            Hello from the glucose graph!
        </>
    )
}
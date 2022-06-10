import React, { useState, useEffect } from 'react';
import { useSelector, useDispatch } from 'react-redux';

import { RootState, AppDispatch } from 'store';
import {
    diagraphApi,
    useCreateEventMutation,
    useUpdateEventMutation,
    useGetEventsQuery,
    useGetDataQuery,
    useGetProfileQuery,
    useGetTagsQuery } from 'services';
import {
    NotificationForm,
    handleQuery,
    Box,
    Container,
    Item,
    Loader,
    DateRangePicker,
    toLocalISODateString,
    useValidation } from 'modules/common';
import { Notification, CreateEventCommand, Event, EventTag, GlucoseMeasurement } from 'types';
import { GlucoseGraph, EventForm, RecentEvents, setDateRange } from 'modules/graph';


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

function notificationValidation(notification: Notification | undefined): [boolean, string] {
    if (!notification)                         return [false, ''];
    if (notification.notifyAtUtc < new Date()) return [false, ''];
    if (!notification.text)                    return [false, ''];

    return [true, ''];
}

export function Dashboard() {
    const events       = useSelector((state: RootState) => state.graph.events);
    const measurements = useSelector((state: RootState) => state.graph.data);
    const dateRange    = useSelector((state: RootState) => state.graph.dateRange);
    const tagsData     = useSelector((state: RootState) => state.shared.tags);
    const taskList     = useSelector((state: RootState) => state.profile.profile.googleTaskList);
    const integration  = useSelector((state: RootState) => state.profile.profile.googleIntegration);

    useEffect(() => {}, [dateRange, events, measurements]);

    const [createTask, setCreateTask]                   = useState(false);
    const [editing, setEditing]                         = useState(false);
    const [selectedMeasurement, setSelectedMeasurement] = useState<GlucoseMeasurement | undefined>();
    const [selectedEvent, setSelectedEvent]             = useState<Event | undefined>();

    const [notification, setNotification, notificationError] = useValidation(
        notificationValidation,
        {} as Notification
    )

    const [createEvent]  = useCreateEventMutation();
    const [updateEvent]  = useUpdateEventMutation();

    const dispatch = useDispatch<AppDispatch>();

    const getData    = useGetDataQuery({ from: dateRange.from, to: dateRange.to });
    const getEvents  = useGetEventsQuery({ from: dateRange.from,to: dateRange.to });
    const getTags    = useGetTagsQuery(undefined);
    const getProfile = useGetProfileQuery(undefined);

    if (handleQuery(getProfile)) return <Loader />
    if (handleQuery(getData))    return <Loader />
    if (handleQuery(getEvents))  return <Loader />
    if (handleQuery(getTags))    return <Loader />

    function onCreateEvent(event: Event) {
        const command: CreateEventCommand = { event };
        if (notification?.text) command.notification = { ...notification, parent: taskList };

        createEvent(command);
    }

    function onChangeDateRange(from: Date, to: Date) {
        const dateRange = {from: toLocalISODateString(from), to: toLocalISODateString(to)};
        dispatch(setDateRange(dateRange));

        const fetchOptions = { subscribe: false, forceRefetch: true };
        dispatch(diagraphApi.endpoints.getData.initiate(dateRange, fetchOptions));
        dispatch(diagraphApi.endpoints.getEvents.initiate(dateRange, fetchOptions));
    }

    async function onExportEvents() {
        // TODO: there is, most certainly, a waaaaay better way.
        window.location.href = 'https://localhost:7053/events/data-export/csv?mergeSequential=true';
    }

    return (
        <Container vertical>
            <button className="button"
                    style={{marginLeft:"80%"}}
                    onClick={onExportEvents}>
                Export Events
            </button>
            <Container>
                <DateRangePicker
                    from={toLocalDate(dateRange.from)}
                    to={toLocalDate(dateRange.to)}
                    onSubmit={onChangeDateRange}
                    submitButtonText="Apply dates" />
            </Container>
            <Container vertical>
                <div className="centered">
                    <GlucoseGraph
                        from={toLocalDate(dateRange.from)}
                        to={toLocalDate(dateRange.to)}
                        points={measurements}
                        events={events}
                        onClickEvent={setSelectedEvent}
                        onClickMeasurement={setSelectedMeasurement} />
                </div>
            </Container>
            <Container>
                <Item>
                    {selectedMeasurement && (
                        <Box>
                        <Container vertical>
                            <button className="button"
                                    onClick={() => setSelectedMeasurement(undefined)}>
                                x
                            </button>
                            <label>Date: </label>
                            <input disabled value={selectedMeasurement!.takenAt.toLocaleString()} />
                            <label>Glucose mmol/L</label>
                            <input disabled value={selectedMeasurement!.level} />
                        </Container>
                        </Box>
                    )}
                    {selectedEvent ? (
                        <Item>
                        <Container vertical>
                        <Box>
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
                        </Box>
                        </Container>
                        </Item>
                    ) : (
                        <Container vertical>
                            <Item>
                                <EventForm
                                    value={EMPTY_EVENT}
                                    onSubmit={onCreateEvent}
                                    tagOptions={tagsData ?? []}
                                    submitButtonText="Create Event" />
                            </Item>
                            {integration && (
                                <Item>
                                    <label htmlFor="eventCreateTask">Create task</label>
                                    <input id="eventCreateTask"
                                           type="checkbox"
                                           checked={createTask}
                                           onChange={() => setCreateTask(!createTask)}/>
                                    {createTask && <NotificationForm onChange={n => setNotification(n)} />}
                                    {createTask && notificationError && <span>{notificationError}</span>}
                                </Item>
                            )}
                        </Container>
                    )}
                </Item>
                <Item>
                    <RecentEvents events={events} onEdit={setSelectedEvent} />
                </Item>
            </Container>
        </Container>
    );
}
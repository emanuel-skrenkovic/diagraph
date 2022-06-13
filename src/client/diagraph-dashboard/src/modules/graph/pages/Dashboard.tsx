import React, { useState, useEffect } from 'react';
import { useSelector, useDispatch } from 'react-redux';

import { Box, Button, RedButton, Centered, Container, Input, Item, Title } from 'styles';
import { RootState, AppDispatch } from 'store';
import {
    diagraphApi,
    useCreateEventMutation,
    useUpdateEventMutation,
    useDeleteEventMutation,
    useGetEventsQuery,
    useGetDataQuery,
    useGetProfileQuery,
    useGetTagsQuery } from 'services';
import {
    NotificationForm,
    handleQuery,
    Loader,
    DateRangePicker,
    toLocalISODateString,
    useValidation } from 'modules/common';
import { Notification, CreateEventCommand, Event, EventTag, GlucoseMeasurement } from 'types';
import { GlucoseGraph, EventForm, RecentEvents, setDateRange } from 'modules/graph';

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

    const [createEvent] = useCreateEventMutation();
    const [updateEvent] = useUpdateEventMutation();
    const [deleteEvent] = useDeleteEventMutation();

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

    function onExportEvents() {
        // TODO: there is, most certainly, a waaaaay better way.
        window.location.href = 'https://localhost:7053/events/data-export/csv?mergeSequential=true';
    }

    function renderNewEventForm() {
        return <Container vertical>
            <EventForm
                value={EMPTY_EVENT}
                onSubmit={onCreateEvent}
                tagOptions={tagsData ?? []}
                submitButtonText="Create Event" />
            {integration && (
                <>
                    <label htmlFor="eventCreateTask">Create task</label>
                    <Input id="eventCreateTask"
                           type="checkbox"
                           checked={createTask}
                           onChange={() => setCreateTask(!createTask)}/>
                    {createTask && (
                        <>
                            <NotificationForm onChange={n => setNotification(n)} />
                            {notificationError && <span>{notificationError}</span>}
                        </>
                    )}
                </>
            )}
        </Container>
    }

    function renderEditEventForm() {
        return <Box>
            <Centered>
                <Item as={Button} onClick={() => {
                    setSelectedEvent(undefined);
                    if (editing) setEditing(false);
                }}>
                    Close
                </Item>
                <Item as={Button} onClick={() => setEditing(!editing)}>
                    Edit
                </Item>
                <Item as={RedButton} onClick={() => {
                    setSelectedEvent(undefined);
                    setEditing(false);
                    deleteEvent(selectedEvent!.id!);
                }}>
                    Delete
                </Item>
            </Centered>
            <EventForm
                value={selectedEvent!}
                onSubmit={e => {
                    updateEvent(e);
                    setEditing(false);
                }}
                tagOptions={tagsData ?? []}
                submitButtonText="Save"
                disabled={!editing} />
        </Box>
    }

    return (
        <Container vertical>
            <Button onClick={onExportEvents} style={{marginLeft:"90%",whiteSpace:"nowrap"}}>
                Export Events
            </Button>
            <Item as={DateRangePicker} from={toLocalDate(dateRange.from)}
                             to={toLocalDate(dateRange.to)}
                             onSubmit={onChangeDateRange}
                             submitButtonText="Apply dates" />
            <Container vertical>
                <Centered>
                    <GlucoseGraph
                        from={toLocalDate(dateRange.from)}
                        to={toLocalDate(dateRange.to)}
                        points={measurements}
                        events={events}
                        onClickEvent={setSelectedEvent}
                        onClickMeasurement={setSelectedMeasurement} />
                </Centered>
            </Container>
            <Container>
                <Item>
                    {selectedMeasurement && (
                        <Box>
                        <Container vertical>
                            <Button onClick={() => setSelectedMeasurement(undefined)}>
                                x
                            </Button>
                            <label>Date: </label>
                            <Input disabled value={selectedMeasurement!.takenAt.toLocaleString()} />
                            <label>Glucose mmol/L</label>
                            <Input disabled value={selectedMeasurement!.level} />
                        </Container>
                        </Box>
                    )}
                    {selectedEvent ? renderEditEventForm() : renderNewEventForm()}
                </Item>
                <Item style={{width:"50%"}}>
                    <Title>Recent events:</Title>
                    {events.length > 0 && <RecentEvents events={events} onEdit={setSelectedEvent} />}
                </Item>
            </Container>
        </Container>
    );
}
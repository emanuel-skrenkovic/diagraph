import React, { useState, useEffect } from 'react';
import { useDispatch } from 'react-redux';

import { Box, Button, DangerButton, Centered, Container, Input, Item, Title } from 'styles';
import { AppDispatch } from 'store';
import { diagraphApi, useCreateEventMutation, useUpdateEventMutation, useDeleteEventMutation,
    useGetEventQuery, useGetEventsQuery, useGetDataQuery, useGetProfileQuery, useGetTagsQuery
} from 'services';
import {
    NotificationForm, Loader, DateRangePicker, useAppSelector, withLoading
} from 'modules/common';
import { Notification, CreateEventCommand, Event, GlucoseMeasurement } from 'types';
import { GlucoseManagementIndicator, Statistics, GlucoseGraph, EventForm, RecentEvents, setDateRange,
    setSelectedEventId, GlucoseMeasurementView } from 'modules/graph';

export const Dashboard = () => {
    const { selectedEventId, events, measurements, dateRange } = useAppSelector(state => state.graph);
    const { googleTaskList, googleIntegration } = useAppSelector (state => state.profile.profile);

    const [createTask, setCreateTask]                   = useState(false);
    const [editing, setEditing]                         = useState(false);
    const [selectedMeasurement, setSelectedMeasurement] = useState<GlucoseMeasurement | undefined>();
    const [selectedEvent, setSelectedEvent]             = useState<Event | undefined>();
    const [notification, setNotification]               = useState<Notification>(EMPTY_NOTIFICATION);

    const [createEvent, { isLoading: isCreateEventLoading }] = useCreateEventMutation();
    const [updateEvent, { isLoading: isUpdateEventLoading }] = useUpdateEventMutation();
    const [deleteEvent, { isLoading: isDeleteEventLoading }] = useDeleteEventMutation();

    const dispatch = useDispatch<AppDispatch>();
    const { data: eventData, isLoading: isEventLoading } = useGetEventQuery(
        selectedEventId!,
        { skip: selectedEventId === selectedEvent?.id || selectedEventId === undefined }
    );
    useEffect(
        () => { if (selectedEventId && eventData) setSelectedEvent(eventData) },
        [dateRange, selectedEventId, eventData, events, measurements]
    );

    const range = { from: dateRange.from, to: dateRange.to };
    const { isLoading: isDataLoading }    = useGetDataQuery(range);
    const { isLoading: isEventsLoading }  = useGetEventsQuery(range);
    const { isLoading: isTagsLoading }    = useGetTagsQuery(undefined);
    const { isLoading: isProfileLoading } = useGetProfileQuery(undefined);

    if (isDataLoading
        || isEventsLoading
        || isTagsLoading
        || isProfileLoading
        || isCreateEventLoading
        || isUpdateEventLoading
        || isDeleteEventLoading) return <Loader />

    const onCreateEvent = (event: Event) => {
        const command: CreateEventCommand = { event };
        if (notification?.text) command.notification = { ...notification, parent: googleTaskList };

        createEvent(command);
    }

    const onChangeDateRange = (from: Date, to: Date) => {
        // convert parameters to UTC
        const dateRange = { from: from.toISOString(), to: to.toISOString() };
        dispatch(setDateRange(dateRange));

        const fetchOptions = { subscribe: false, forceRefetch: true };
        dispatch(diagraphApi.endpoints.getData.initiate(dateRange, fetchOptions));
        dispatch(diagraphApi.endpoints.getEvents.initiate(dateRange, fetchOptions));
    }

    const selectEvent = (event: Event) => dispatch(setSelectedEventId(event.id));

    const deselectEvent = () => {
        setSelectedEvent(undefined);
        dispatch(setSelectedEventId(undefined));
        if (editing) setEditing(false);
    }

    const deleteSelectedEvent = () => {
        deselectEvent();
        deleteEvent(selectedEvent!.id!);
    }

    const updateSelectedEvent = (e: Event) => {
        updateEvent(e);
        setEditing(false);
    }

    const EditEventFormWithLoading = withLoading(() => (
        <Box>
            <Centered>
                <Item as={Button} onClick={deselectEvent}>Close</Item>
                <Item as={Button} onClick={() => setEditing(!editing)}>Edit</Item>
                <Item as={DangerButton} onClick={deleteSelectedEvent}>Delete</Item>
            </Centered>
            <EventForm value={selectedEvent!}
                       onSubmit={updateSelectedEvent}
                       submitButtonText="Save"
                       disabled={!editing} />
        </Box>
    ));

    const localTimeEvents = events.map(e => ({
        ...e,
        occurredAtUtc: new Date(e.occurredAtUtc),
        endedAtUtc: e.endedAtUtc ? new Date(e.endedAtUtc) : undefined
    }));

    return (
        <Container vertical>
            <Item as={DateRangePicker}
                  from={toLocalDate(dateRange.from)}
                  to={toLocalDate(dateRange.to)}
                  onSubmit={onChangeDateRange}
                  submitButtonText="Apply dates" />
            <Container>
                <Item>
                    <Container vertical>
                        <Statistics measurements={measurements} />
                        <GlucoseManagementIndicator periodEndDate={new Date(dateRange.to)} />
                    </Container>
                </Item>
                <Item>
                    <GlucoseGraph from={toLocalDate(dateRange.from)}
                                  to={toLocalDate(dateRange.to)}
                                  points={measurements}
                                  events={localTimeEvents}
                                  onClickEvent={selectEvent}
                                  onClickMeasurement={setSelectedMeasurement} />
                </Item>
            </Container>
            <Container>
                <Item style={{width:"50%"}}>
                    {selectedEvent
                        ? <EditEventFormWithLoading isLoading={isEventLoading} />
                        :  (
                            <Container vertical>
                                <EventForm onSubmit={onCreateEvent} submitButtonText="Create Event" />
                                <Item>
                                    {googleIntegration && (
                                        <>
                                            <label htmlFor="eventCreateTask">Create task</label>
                                            <Input id="eventCreateTask"
                                                   type="checkbox"
                                                   checked={createTask}
                                                   onChange={() => {
                                                       setCreateTask(!createTask)
                                                       setNotification(EMPTY_NOTIFICATION);
                                                   }}/>
                                            {createTask && (
                                                <NotificationForm value={notification}
                                                                  onChange={n => setNotification(n)} />

                                            )}
                                        </>
                                    )}
                                </Item>
                            </Container>
                        )}
                </Item>
                <Item style={{width:"35%"}}>
                    {selectedMeasurement && (
                        <Box as={Container} vertical>
                            <Button onClick={() => setSelectedMeasurement(undefined)}>
                                Close
                            </Button>
                            <GlucoseMeasurementView value={selectedMeasurement} />
                        </Box>
                    )}
                </Item>
                <Item style={{width:"50%"}}>
                    <Title>Recent events:</Title>
                    <Item style={{width:"100%"}}>
                        {events.length > 0 && (
                            <RecentEvents events={events}
                                          onEdit={selectEvent}
                                          onDelete={e => deleteEvent(e.id)} />
                        )}
                    </Item>
                </Item>
            </Container>
        </Container>
    );
}

function toLocalDate(dateString: string): Date {
    const date = new Date(dateString);
    date.setHours(0, 0, 0, 0);
    return date;
}

const EMPTY_NOTIFICATION: Notification = {
    notifyAtUtc: new Date(),
    text: '',
    parent: 'My Tasks'
};
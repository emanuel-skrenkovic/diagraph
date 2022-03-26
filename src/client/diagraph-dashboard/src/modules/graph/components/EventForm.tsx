import React, { useState, FormEvent } from 'react';
import { Event } from 'types';

export interface EventFormProps {
    initialValue?: Event | undefined;
    onSubmit: (e: Event) => void;
    submitButtonText?: string | undefined;
}

export function EventForm(props: EventFormProps) {
    const [event, setEvent] = useState(props.initialValue ?? {
        occurredAtUtc: new Date()
    } as Event);
    // TODO: add tags - check the store repo for an example

    const onClickSubmit = (e: FormEvent<HTMLButtonElement>) => {
        e.preventDefault();
        props.onSubmit(event);
    }

    return (
        <form>
            <label htmlFor="eventText">Text</label>
            <input
                id="eventText"
                type="text"
                value={event.text}
                onChange={e => setEvent({ ...event, text: e.currentTarget.value })}/>
            <label htmlFor="eventOccurredAt">Time</label>
            <input
                id="eventOccurredAt"
                type="time"
                value={`${event.occurredAtUtc.getHours()}:${event.occurredAtUtc.getMinutes()}`}
                onChange={e => setEvent({ ...event, occurredAtUtc: new Date(e.currentTarget.value) })} />
            <button onClick={onClickSubmit} type="submit">
                {props.submitButtonText ?? 'Submit'}
            </button>
        </form>
    )
}
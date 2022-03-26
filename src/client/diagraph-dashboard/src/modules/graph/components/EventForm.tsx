import React, { useState, FormEvent } from 'react';
import { Event } from 'types';

import 'App.css';

export interface EventFormProps {
    initialValue: Event;
    onSubmit: (e: Event) => void;
    submitButtonText?: string | undefined;
}

function hoursFormat(date: Date) {
    const hours   = date.getHours().toString().padStart(2, '0');
    const minutes = date.getMinutes().toString().padStart(2, '0');
    return `${hours}:${minutes}`;
}

export function EventForm(props: EventFormProps) {
    const [event, setEvent] = useState(props.initialValue);
    // TODO: add tags - check the store repo for an example

    const onClickSubmit = (e: FormEvent<HTMLButtonElement>) => {
        e.preventDefault();
        props.onSubmit(event);
    };

    // this seems so bad.
    const onChangeTime = (e: FormEvent<HTMLInputElement>) => {
        const newOccurredAtUtc = new Date(event.occurredAtUtc);

        const [hours, minutes] = e.currentTarget.value.split(':');
        newOccurredAtUtc.setHours(Number(hours));
        newOccurredAtUtc.setMinutes(Number(minutes));

        setEvent({ ...event, occurredAtUtc: newOccurredAtUtc });
    };

    return (
        <form className="container horizontal box">
            <div className="item">
                <textarea
                    id="eventText"
                    value={event.text}
                    onChange={e => setEvent({ ...event, text: e.currentTarget.value })}/>
            </div>
            <div className="item">
                <label htmlFor="eventOccurredAt">Time</label>
                <input
                    id="eventOccurredAt"
                    type="time"
                    value={hoursFormat(event.occurredAtUtc)}
                    onChange={onChangeTime} />
            </div>
            <button className="submit button" onClick={onClickSubmit} type="submit">
                {props.submitButtonText ?? 'Submit'}
            </button>
        </form>
    )
}
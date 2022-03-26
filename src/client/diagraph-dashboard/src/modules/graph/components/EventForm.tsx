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
        const newOccurredAt = new Date(event.occurredAt);

        const [hours, minutes] = e.currentTarget.value.split(':');
        newOccurredAt.setHours(Number(hours));
        newOccurredAt.setMinutes(Number(minutes));

        setEvent({ ...event, occurredAt: newOccurredAt });
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
                    value={hoursFormat(event.occurredAt)}
                    onChange={onChangeTime} />
            </div>
            <button className="button submit" onClick={onClickSubmit} type="submit">
                {props.submitButtonText ?? 'Submit'}
            </button>
        </form>
    )
}
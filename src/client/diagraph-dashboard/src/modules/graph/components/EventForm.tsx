import React, { useState, FormEvent } from 'react';

import { Event } from 'types';

import 'App.css';

export interface EventFormProps {
    value: Event;
    onSubmit: (e: Event) => void;
    submitButtonText?: string | undefined;
    disabled?: boolean
}

function hoursFormat(date: Date) {
    const hours   = new Date(date).getUTCHours().toString().padStart(2, '0');
    const minutes = new Date(date).getUTCMinutes().toString().padStart(2, '0');
    return `${hours}:${minutes}`;
}

export function EventForm(props: EventFormProps) {
    const [event, setEvent] = useState(props.value);

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

    const { disabled } = props;

    return (
        <form className="container horizontal box">
            <div className="item">
                <textarea
                    disabled={disabled}
                    id="eventText"
                    value={event.text}
                    onChange={e => setEvent({ ...event, text: e.currentTarget.value })}/>
            </div>
            <div className="item">
                <label htmlFor="eventOccurredAt">Time</label>
                <input
                    id="eventOccurredAt"
                    type="time"
                    disabled={disabled}
                    value={hoursFormat(event.occurredAtUtc)}
                    onChange={onChangeTime} />
            </div>
            {!disabled &&
                <button className="submit button blue" onClick={onClickSubmit} type="submit">
                {props.submitButtonText ?? 'Submit'}
            </button>
            }
        </form>
    )
}
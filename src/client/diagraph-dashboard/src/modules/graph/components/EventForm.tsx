import React, { useEffect, FormEvent } from 'react';

import { Event, EventTag } from 'types';
import { useValidation, TagSelector } from 'modules/common';

import 'App.css';

export interface EventFormProps {
    value: Event;
    onSubmit: (e: Event) => void;
    tagOptions: EventTag[];
    submitButtonText?: string | undefined;
    disabled?: boolean,
}

function hoursFormat(date: Date) {
    const hours   = new Date(date).getUTCHours().toString().padStart(2, '0');
    const minutes = new Date(date).getUTCMinutes().toString().padStart(2, '0');
    return `${hours}:${minutes}`;
}

export const EventForm = (props: EventFormProps) => {
    const [event, setEvent, error, validateEvent] = useValidation<Event>(
        event => !(event?.text)
            ? [false, 'Event text must not be empty.']
            : [true, ''],
        props.value
    );
    useEffect(() => setEvent(props.value), [props]);

    function onClickSubmit(e: FormEvent<HTMLButtonElement>) {
        e.preventDefault();

        if (!validateEvent()) return;
        props.onSubmit(event!);
    }

    // this seems so bad.
    function onChangeTime(e: FormEvent<HTMLInputElement>) {
        const newOccurredAtUtc = new Date(event!.occurredAtUtc);

        const [hours, minutes] = e.currentTarget.value.split(':');
        newOccurredAtUtc.setHours(Number(hours));
        newOccurredAtUtc.setMinutes(Number(minutes));

        setEvent({ ...event, occurredAtUtc: newOccurredAtUtc } as Event);
    }

    const { disabled } = props;

    return (
        <form className="container vertical box">
            {!disabled &&
                <button className="submit button blue centered" onClick={onClickSubmit} type="submit">
                    {props.submitButtonText ?? 'Submit'}
                </button>
            }
            <div className="container vertical item">
                <label className="centered" htmlFor="eventText">Text</label>
                <textarea className={`centered input ${error && 'invalid'}`}
                          style={{height:"125px", width:"300px"}}
                          disabled={disabled}
                          id="eventText"
                          value={event!.text}
                          onChange={e => setEvent({ ...event, text: e.currentTarget.value } as Event)} />
                {error && <span className="input label">{error}</span>}
            </div>
            <div className="item">
                <label htmlFor="eventOccurredAt">Occurred at</label>
                <input
                    id="eventOccurredAt"
                    type="time"
                    disabled={disabled}
                    value={hoursFormat(event!.occurredAtUtc)}
                    onChange={onChangeTime} />
            </div>
            <div className="item">
                <TagSelector initialSelectedTags={event!.tags}
                             onChange={tags => setEvent({...event, tags} as Event)} />
            </div>
        </form>
    )
};
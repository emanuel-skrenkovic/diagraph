import React, { FormEvent } from 'react';

import { BlueButton, Box, Centered, Container, Input } from 'styles';
import { Event, EventTag } from 'types';
import { useValidation, TagSelector } from 'modules/common';

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
        <Container vertical>
            <Box>
                {!disabled &&
                    <Centered as={BlueButton} onClick={onClickSubmit} type="submit">
                        {props.submitButtonText ?? 'Submit'}
                    </Centered>
                }
                <Container vertical>
                    <label htmlFor="eventText">Text</label>
                    <Centered>
                    <textarea className={`input ${error && 'invalid'}`}
                              disabled={disabled}
                              id="eventText"
                              value={event!.text}
                              onChange={e => setEvent({ ...event, text: e.currentTarget.value } as Event)} />
                        {error && <span className="input label">{error}</span>}
                    </Centered>
                </Container>
                <label htmlFor="eventOccurredAt">Occurred at </label>
                <Input id="eventOccurredAt"
                       type="time"
                       disabled={disabled}
                       value={hoursFormat(event!.occurredAtUtc)}
                       onChange={onChangeTime} />
                <TagSelector initialSelectedTags={event!.tags}
                             onChange={tags => setEvent({...event, tags} as Event)} />
            </Box>
        </Container>
    )
};
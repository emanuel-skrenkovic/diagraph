import React, { useEffect, ChangeEvent, FormEvent } from 'react';

import { Button, PrimaryButton, Box, Centered, Container, Input, Item } from 'styles';
import { Event, EventTag } from 'types';
import { useValidation, TagSelector } from 'modules/common';

export type EventFormProps = {
    value?: Event | undefined;
    onSubmit: (e: Event) => void;
    submitButtonText?: string | undefined;
    disabled?: boolean,
}

export const EventForm = ({ value, disabled, onSubmit, submitButtonText }: EventFormProps) => {
    const [event, setEvent, error, validateEvent] = useValidation<Event>(
        event => !(event?.text)
            ? [false, 'Event text must not be empty.']
            : [true, ''],
        value ?? EMPTY_EVENT
    );

    useEffect(() => value && setEvent(value), [value]);

    const onClickSubmit = (e: FormEvent<HTMLButtonElement>) => {
        e.preventDefault();

        if (!validateEvent()) return;
        onSubmit(event!);
    }

    // this seems so bad.
    const onChangeOccurredAt = (e: FormEvent<HTMLInputElement>) => {
        const newOccurredAtUtc = new Date(event!.occurredAtUtc);

        const [hours, minutes] = e.currentTarget.value.split(':');
        newOccurredAtUtc.setHours(Number(hours));
        newOccurredAtUtc.setMinutes(Number(minutes));

        setEvent({ ...event, occurredAtUtc: newOccurredAtUtc } as Event);
    }

    const onChangeEndedAt = (e: FormEvent<HTMLInputElement>) => {
        const newEndedAt = event?.endedAtUtc
            ? new Date(event.endedAtUtc)
            : new Date();

        const [hours, minutes] = e.currentTarget.value.split(':');
        newEndedAt.setHours(Number(hours));
        newEndedAt.setMinutes(Number(minutes));

        setEvent({ ...event, endedAtUtc: newEndedAt } as Event);
    }
    const onClearEndedAt = () => setEvent({ ...event, endedAtUtc: undefined } as Event);

    const onChangeEventText = (e: ChangeEvent<HTMLTextAreaElement>) =>
        setEvent({ ...event, text: e.currentTarget.value } as Event);

    const setEventTags = (tags: EventTag[]) => setEvent({...event, tags} as Event);

    return (
        <Container vertical>
            <Box>
                {!disabled &&
                    <Centered as={PrimaryButton} onClick={onClickSubmit} type="submit">
                        {submitButtonText ?? 'Submit'}
                    </Centered>
                }
                <Container vertical>
                    <label htmlFor="eventText">Text</label>
                    <Centered as="textarea" className={`input ${error && 'invalid'}`}
                              style={{width:"90%",minHeight:"150px"}}
                              disabled={disabled}
                              id="eventText"
                              value={event!.text}
                              onChange={onChangeEventText} />
                    {error && <span className="input label">{error}</span>}
                </Container>
                <Container>
                    <Item>
                        <label htmlFor="eventOccurredAt">Occurred at </label>
                        <Input id="eventOccurredAt"
                               type="time"
                               disabled={disabled}
                               value={hoursFormat(event!.occurredAtUtc)}
                               onChange={onChangeOccurredAt} />
                    </Item>
                    <Item>
                        <label htmlFor="eventEndedAt">Ended at </label>
                        <Input id="eventEndedAt"
                               type="time"
                               disabled={disabled}
                               value={hoursFormat(event!.endedAtUtc)}
                               onChange={onChangeEndedAt} />
                        <Button onClick={onClearEndedAt}>
                            Clear
                        </Button>
                    </Item>
                </Container>
                <TagSelector initialSelectedTags={event!.tags} onChange={setEventTags} />
            </Box>
        </Container>
    )
};

const EMPTY_EVENT = {
    occurredAtUtc: new Date(),
    text: '',
    tags: [] as EventTag[]
} as Event;

function hoursFormat(date: Date | undefined) {
    if (!date) return '';

    const hours   = new Date(date).getHours().toString().padStart(2, '0');
    const minutes = new Date(date).getMinutes().toString().padStart(2, '0');
    return `${hours}:${minutes}`;
}
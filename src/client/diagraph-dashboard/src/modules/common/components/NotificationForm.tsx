import React, { ChangeEvent } from 'react';

import { Container, Input } from 'styles';
import { Notification } from 'types';
import { useValidation } from 'modules/common';

export type NotificationFormProps = {
    value?: Notification;
    onChange: (notification: Notification) => void;
}

function hoursFormat(date: Date) {
    const hours   = new Date(date).getHours().toString().padStart(2, '0');
    const minutes = new Date(date).getMinutes().toString().padStart(2, '0');
    return `${hours}:${minutes}`;
}

export const NotificationForm = ({ value, onChange }: NotificationFormProps) => {
    const [notification, setNotification, notificationError] = useValidation<Notification>(
        notificationValidation,
        value ?? EMPTY_NOTIFICATION
    );

    const onChangeText = (e: ChangeEvent<HTMLInputElement>) => {
        e.preventDefault();
        const updated: Notification = { ...notification!, text: e.currentTarget.value };
        setNotification(updated);
        onChange(updated);
    };

    const onChangeNotifyAt = (e: ChangeEvent<HTMLInputElement>) => {
        e.preventDefault();

        const date = new Date();
        const parts = e.currentTarget.value.split(':');

        date.setHours(Number(parts[0]));
        date.setMinutes(Number(parts[1]))

        console.log(date)

        const updated: Notification = { ...notification!, notifyAtUtc: new Date(date) };
        setNotification(updated)
        onChange(updated);
    }

    return (
        <Container vertical>
            <Container>
                <Input type="text"
                       value={notification!.text}
                       onChange={onChangeText} />
                <Input type="time"
                       value={hoursFormat(notification!.notifyAtUtc)}
                       onChange={onChangeNotifyAt} />
            </Container>
            {notificationError && <span>{notificationError}</span>}
        </Container>

    );
};

function notificationValidation(notification: Notification | undefined): [boolean, string] {
    if (!notification)                         return [false, ''];
    if (notification.notifyAtUtc < new Date()) return [false, 'Cannot set time to before now.'];
    if (!notification.text)                    return [false, 'Notification text cannot be empty.'];

    return [true, ''];
}

const EMPTY_NOTIFICATION: Notification = {
    notifyAtUtc: new Date(),
    text: '',
    parent: 'My Tasks'
};
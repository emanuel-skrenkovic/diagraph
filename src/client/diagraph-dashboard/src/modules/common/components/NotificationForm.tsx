import React, { useState, useEffect } from 'react';

import { Container, Input } from 'styles';
import { Notification } from 'types';

export interface NotificationFormProps {
    value?: Notification;
    onChange: (notification: Notification) => void;
}

const EMPTY_NOTIFICATION: Notification = {
    notifyAtUtc: new Date(),
    text: '',
    parent: 'My Tasks'
};

function hoursFormat(date: Date) {
    const hours   = new Date(date).getUTCHours().toString().padStart(2, '0');
    const minutes = new Date(date).getUTCMinutes().toString().padStart(2, '0');
    return `${hours}:${minutes}`;
}

export const NotificationForm: React.FC<NotificationFormProps> = ({ value, onChange }) => {
    const [notification, setNotification] = useState<Notification>(value ?? EMPTY_NOTIFICATION);

    useEffect(() => value && setNotification(value), [value])

    function updateNotification(newNotification: Notification) {
        setNotification(newNotification);
        onChange(newNotification);
    }

    return (
        <Container>
            <Input type="text"
                   value={notification.text}
                   onChange={e => updateNotification({
                       ...notification,
                       text: e.currentTarget.value
                   })} />
            <Input type="time"
                   value={hoursFormat(notification.notifyAtUtc)}
                   onChange={e => updateNotification({
                       ...notification,
                       notifyAtUtc: new Date(e.currentTarget.value)
                   })} />
        </Container>
    );
};
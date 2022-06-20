import React from 'react';

export type LocalTimeProps = {
    date: Date;
}

export const LocalTime = ({ date }: LocalTimeProps) => {
    return <>{new Date(date).toLocaleTimeString()}</>
}
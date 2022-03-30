import React from 'react';

export interface LocalTimeProps {
    date: Date;
}

export const LocalTime = ({date}: LocalTimeProps) => {
    return <>{new Date(date).toLocaleTimeString()}</>
}
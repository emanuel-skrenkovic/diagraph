import React, { useState, FormEvent } from 'react';

import 'App.css';

function toLocalISODateString(date: Date) {
    return new Date(date.getTime() - (date.getTimezoneOffset() * 60000))
        .toISOString()
        .split('T')[0];
}

export interface DateRangePickerProps {
    from :Date;
    to: Date;
    onSubmit: (from: Date, to: Date) => void;
    submitButtonText?: string;
}

export const DateRangePicker = ({from, to, onSubmit, submitButtonText}: DateRangePickerProps) => {
    const [formFrom, setFormFrom] = useState(toLocalISODateString(from));
    const [formTo, setFormTo] = useState(toLocalISODateString(to));

    const onClickSubmit = (_: FormEvent<HTMLButtonElement>) => {
        onSubmit(new Date(formFrom), new Date(formTo));
    }

    return (
        <>
            <div className="container">
                <div className="container horizontal">
                    <label htmlFor="from">From</label>
                    <input id="from"
                           type="date"
                           value={formFrom}
                           onChange={e => setFormFrom(e.currentTarget.value)}/>
                </div>
                <div className="container horizontal">
                    <label htmlFor="to">To</label>
                    <input id="to"
                           type="date"
                           value={formTo}
                           onChange={e => setFormTo(e.currentTarget.value)} />
                </div>
            </div>
            <button className="button item" onClick={onClickSubmit}>{submitButtonText ?? 'Submit'}</button>
        </>
);
};

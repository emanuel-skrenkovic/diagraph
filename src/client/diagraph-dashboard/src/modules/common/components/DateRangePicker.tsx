import React, { useState, useEffect, FormEvent } from 'react';

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
    const [dateRange, setDateRange] = useState<{ from: string, to: string }>({
        from: toLocalISODateString(from),
        to: toLocalISODateString(to)
    });

    useEffect(() => {
        setDateRange({
            from: toLocalISODateString(from),
            to: toLocalISODateString(to)
        });
    }, [from, to]);

    const onClickSubmit = (_: FormEvent<HTMLButtonElement>) => {
        onSubmit(new Date(dateRange.from), new Date(dateRange.to));
    }

    return (
        <div className="container horizontal">
            <div className="container">
                <div className="container horizontal">
                    <label htmlFor="from">From</label>
                    <input id="from"
                           type="date"
                           value={dateRange.from}
                           onChange={e => setDateRange({...dateRange, from: e.currentTarget.value})}/>
                </div>
                <div className="container horizontal">
                    <label htmlFor="to">To</label>
                    <input id="to"
                           type="date"
                           value={dateRange.to}
                           onChange={e => setDateRange({...dateRange, to: e.currentTarget.value})} />
                </div>
            </div>
            <button className="button item" onClick={onClickSubmit}>{submitButtonText ?? 'Submit'}</button>
        </div>
);
};

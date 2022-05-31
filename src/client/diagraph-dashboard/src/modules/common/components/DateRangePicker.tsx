import React, { useState, useEffect, FormEvent } from 'react';

import { toLocalISODateString, Container } from 'modules/common';

import 'App.css';

export interface DateRangePickerProps {
    from :Date;
    to: Date;
    onSubmit: (from: Date, to: Date) => void;
    submitButtonText?: string;
}

export const DateRangePicker = ({from, to, onSubmit, submitButtonText}: DateRangePickerProps) => {
    const [dateRange, setDateRange] = useState<{ from: string, to: string }>({
        from: toLocalISODateString(from),
        to:   toLocalISODateString(to)
    });

    const moveDateRange = (n: number) => {
        const newFrom = new Date(dateRange.from);
        newFrom.setHours(0, 0, 0, 0);
        newFrom.setDate(newFrom.getDate() + n);

        const newTo = new Date(dateRange.to);
        newTo.setDate(newTo.getDate() + n);
        newTo.setHours(0, 0, 0, 0);

        onSubmit(new Date(newFrom), new Date(newTo));
    }

    useEffect(() => setDateRange({
            from: toLocalISODateString(from),
            to:   toLocalISODateString(to)
    }), [from, to]);

    const onClickSubmit = (e: FormEvent<HTMLButtonElement>) => {
        e.preventDefault();
        onSubmit(new Date(dateRange.from), new Date(dateRange.to));
    }

    return (
        <Container vertical>
            <Container>
                <button
                    className="button"
                    onClick={() => moveDateRange(-1)}>
                    &lt;
                </button>
                <Container>
                    <Container vertical>
                        <label htmlFor="from">From</label>
                        <input id="from"
                               type="date"
                               value={dateRange.from}
                               onChange={e => setDateRange({...dateRange, from: e.currentTarget.value})}/>
                    </Container>
                    <Container vertical>
                        <label htmlFor="to">To</label>
                        <input id="to"
                               type="date"
                               value={dateRange.to}
                               onChange={e => setDateRange({...dateRange, to: e.currentTarget.value})} />
                    </Container>
                </Container>
                <button
                    className="button"
                    onClick={() => moveDateRange(1)}>
                    &gt;
                </button>
            </Container>
            <button className="button item centered" onClick={onClickSubmit}>
                {submitButtonText ?? 'Submit'}
            </button>
        </Container>
);
};

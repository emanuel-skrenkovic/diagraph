import React from 'react';

export type OptionsProps<T> = {
    elements: T[];
    value: (e: T) => string;
}

export const Options = <T extends any>({ elements, value }: OptionsProps<T>) => (
    <>
        <option key={-1}></option>
        {elements.length > 0 && elements.map((element, index) => (
            <option key={index}>{value(element)}</option>
        ))}
    </>
);
import React, { useState, useEffect, ChangeEvent, MouseEvent } from 'react';

import { For } from 'modules/common';

export interface MultiSelectFormProps<T> {
    options: T[];
    keySelector: (option: T) => string;
    onAdd: (selected: T[]) => void;
}

export const MultiSelectForm = <T extends object>({ options, keySelector, onAdd }: MultiSelectFormProps<T>) => {
    const [selectedOptions, setSelectedOptions] = useState<T[]>([]);
    useEffect(() => {}, [options, onAdd]); // TODO: check if necessary

    function onClickAdd(e: MouseEvent<HTMLButtonElement>) {
        e.preventDefault();
        onAdd(selectedOptions);
    }

    function onChange(e: ChangeEvent<HTMLSelectElement>) {
        const keys = Array.from(
            e.currentTarget.selectedOptions,
            o => o.value
        );

        setSelectedOptions(
            options.filter(
                o => keys.includes(keySelector(o))
            )
        );
    }

    return (
        <div className="container horizontal">
            <select className="item" multiple onChange={onChange}>
                <For each={options} onEach={(option, index) => (
                    <option key={index}>
                        {keySelector(option)}
                    </option>
                )} />
            </select>
            <button className="button blue item" onClick={onClickAdd}>
                Add
            </button>
        </div>
    );
};
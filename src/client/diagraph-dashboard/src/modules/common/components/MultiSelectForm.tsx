import React, { useState, useEffect, ChangeEvent, MouseEvent } from 'react';

import { PrimaryButton, Container, Item } from 'styles';
import { For } from 'modules/common';

export type MultiSelectFormProps<T> = {
    options: T[];
    keySelector: (option: T) => string;
    onAdd: (selected: T[]) => void;
}

export const MultiSelectForm = <T extends any>({ options, keySelector, onAdd }: MultiSelectFormProps<T>) => {
    const [selectedOptions, setSelectedOptions] = useState<T[]>([]);
    useEffect(() => {}, [options, onAdd]); // TODO: check if necessary

    function onClickAdd(e: MouseEvent<HTMLButtonElement>) {
        e.preventDefault();
        onAdd(selectedOptions);
    }

    function onChange(e: ChangeEvent<HTMLSelectElement>) {
        const keys = Array.from(e.currentTarget.selectedOptions, o => o.value);
        setSelectedOptions(options.filter(o => keys.includes(keySelector(o))));
    }

    return (
        <Container vertical>
            <Item as="select" multiple onChange={onChange}>
               <For each={options} onEach={(option, index) => (
                    <option key={index}>
                        {keySelector(option)}
                    </option>
                )} />
            </Item>
            <PrimaryButton onClick={onClickAdd}>
                Select
            </PrimaryButton>
        </Container>
    );
};
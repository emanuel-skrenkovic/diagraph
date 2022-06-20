import React from 'react';

export type ForProps<T> = {
    each: T[];
    onEach: (e: T, index: number) => JSX.Element;
}

export const For = <T extends unknown>({ each, onEach }: ForProps<T>) =>
    <>{each.map(onEach)}</>;
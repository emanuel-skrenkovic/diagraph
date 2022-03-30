import React from 'react';

export interface ForProps<T> {
    each: T[];
    onEach: (e: T) => JSX.Element;
}

export const For = <T extends unknown>({ each, onEach }: ForProps<T>) => {
   return (
       <>
           {each.map(onEach)}
       </>
   );
};
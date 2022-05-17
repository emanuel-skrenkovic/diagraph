import { Dispatch, SetStateAction, useState } from 'react';

type ValidationActions<T> =
    [(T | undefined), Dispatch<SetStateAction<T | undefined>>, string, () => boolean];

export const useValidation = <T>(
    validate: (value: T | undefined) => [boolean, string],
    initialValue: (T | undefined) | (() => (T | undefined))): ValidationActions<T> =>
{
    const [value, setValue] = useState(initialValue)
    const [error, setError] = useState('');

    const validateAndSet: Dispatch<SetStateAction<T | undefined>> =
        (setStateAction: SetStateAction<T | undefined>) => {
        let newValue;

        // TODO: there's probably a better way to narrow the type.
        if (typeof setStateAction === 'function') {
            const action = setStateAction as (prevState: T) => T;
            newValue = action(value!);
        } else {
            newValue = setStateAction as T;
        }

        validationAction(newValue);
        setValue(newValue);
    };

    const validationAction = (input: T | undefined): boolean => {
        const [valid, errorMessage] = validate(input);
        if (!valid) {
            setError(errorMessage);
            return false
        }

        setError('');
        return true;
    };

    return [value, validateAndSet, error, () => validationAction(value)];
};
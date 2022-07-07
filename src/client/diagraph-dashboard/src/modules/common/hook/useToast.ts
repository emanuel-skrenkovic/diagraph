import { useDispatch } from 'react-redux';

import { AppDispatch } from 'store';
import { useAppSelector, showToast, removeToast } from 'modules/common';

export const useToast = () => {
    const dispatch = useDispatch<AppDispatch>();
    const shownToasts = useAppSelector(state => state.shared.shownToasts);

    return (message: string) => {
        if (shownToasts.includes(message)) return;

        dispatch(showToast(message));
        setTimeout(() => dispatch(removeToast(message)), 2000);
    }
}
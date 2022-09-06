import { TypedUseSelectorHook, useSelector } from 'react-redux';

import { RootState } from 'diagraph/store';

export const useAppSelector: TypedUseSelectorHook<RootState> = useSelector;

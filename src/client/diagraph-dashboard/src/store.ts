import { configureStore } from '@reduxjs/toolkit';
import { graphReducer } from 'modules/graph';
import { authReducer } from 'modules/auth';
import { diagraphApi } from 'services';

export const store = configureStore({
    reducer: {
        [diagraphApi.reducerPath]: diagraphApi.reducer,
        graph: graphReducer,
        auth: authReducer
    },
    middleware: (getDefaultMiddleware) =>
        getDefaultMiddleware().concat(diagraphApi.middleware)
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
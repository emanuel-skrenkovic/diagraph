import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';

import { Event, GlucoseMeasurement } from 'types';
import { setAuthenticated } from 'modules/auth';

export const diagraphApi = createApi({
    reducerPath: 'diagraphApi',
    baseQuery: fetchBaseQuery({ baseUrl: 'https://localhost:7053' }), // TODO: url
    endpoints: (builder) => ({
        getEvents: builder.query<any, any>({
            query: ({from, to}) => ({ url: 'events', params: {from, to} })
        }),
        getEvent: builder.query<any, any>({
            query: id => ({ url: `events/${id}` })
        }),
        createEvent: builder.mutation<void, Event>({
            query: (request) => ({
                url: 'events',
                method: 'POST',
                body: request
            })
        }),
        updateEvent: builder.mutation<any, any>({
            query: ({ id, ...request }) => ({
                url: `events/${id}`,
                method: 'PUT',
                body: request
            })
        }),

        getData: builder.query<GlucoseMeasurement[], any>({
            query: ({from, to}) => ({ url: 'data', params: {from, to}})
        }),

        login: builder.mutation<any, any>({
            query: request => ({
                url: 'auth/login',
                method: 'POST',
                body: request
            }),
            async onQueryStarted({ id, ...post }, { dispatch, queryFulfilled }) {
                try {
                    const result = await queryFulfilled;
                    await dispatch(setAuthenticated(true));
               } catch {
                    dispatch(setAuthenticated(false));
                }
            }
        }),
        register: builder.mutation<any, any>({
            query: request => ({
                url: 'auth/register',
                method: 'POST',
                body: request
            })
        })
    })
});

export const {
    useGetEventQuery,
    useGetEventsQuery,
    useCreateEventMutation,
    useUpdateEventMutation,
    useGetDataQuery,
    useLoginMutation,
    useRegisterMutation } = diagraphApi;
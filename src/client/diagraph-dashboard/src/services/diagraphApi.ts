import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';
import { Event, GlucoseMeasurement } from 'types';

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
        })
    })
});

export const {
    useGetEventQuery,
    useGetEventsQuery,
    useCreateEventMutation,
    useUpdateEventMutation,
    useGetDataQuery } = diagraphApi;
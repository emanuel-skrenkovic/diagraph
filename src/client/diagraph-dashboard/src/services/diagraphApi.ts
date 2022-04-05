import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';

import { Event, GlucoseMeasurement } from 'types';
import { setProfile, defaultProfile, Profile } from 'modules/profile';
import { setAuthenticated } from 'modules/auth';

export const diagraphApi = createApi({
    reducerPath: 'diagraphApi',
    baseQuery: fetchBaseQuery({
        baseUrl: 'https://localhost:7053', // TODO: configuration
        credentials: 'include'
    }),
    tagTypes: ['Authenticated', 'Profile', 'Events', 'Data'],
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
            query: ({from, to}) => ({ url: 'data', params: {from, to}}),
            async onQueryStarted(api, { queryFulfilled }) {
                try {
                    await queryFulfilled;
                } catch { /* do nothing since it's already not authenticated */ }
            },
            providesTags: [{ type: 'Data', id: 'all' }]
        }),

        importData: builder.mutation<any, any>({
            async queryFn(file, queryApi, extraOptions, fetch) {
                const formData = new FormData();
                formData.append('file', file, file.name);

                const response = await fetch({
                    url: 'data',
                    method: 'POST',
                    body: formData
                });

                // TODO: this bit
                if (response.error) throw response.error;
                return response.data ? { data: response.data } : { error: response.error };
            },
            invalidatesTags: [{ type: 'Data', id: 'all' }] // TODO
        }),

        getProfile: builder.query<Profile, undefined>({
            query: () => ({ url: 'my/profile' }),
            async onQueryStarted(api, { dispatch, queryFulfilled }) {
                const { data } = await queryFulfilled;
                dispatch(setProfile(data ?? defaultProfile))
            },
            providesTags: [{ type: 'Profile', id: 'logged-in' }]
        }),

        updateProfile: builder.mutation<any, any>({
            query: profile => ({ url: 'my/profile', method: 'PUT', body: profile }),
            async onQueryStarted(request, { dispatch, queryFulfilled }) {
                await queryFulfilled;
                dispatch(setProfile(request))
            },
            invalidatesTags: [{ type: 'Profile', id: 'logged-in' }]
        }),

        getSession: builder.query<any, any>({
            query: () => ({ url: 'auth/session' }),
            async onQueryStarted(api, { dispatch, queryFulfilled }) {
                try {
                    await queryFulfilled;
                    dispatch(setAuthenticated(true))
                } catch {
                    dispatch(setAuthenticated(false))
                }
            },
            providesTags: [{ type: 'Authenticated', id: 'true' }],
        }),

        login: builder.mutation<any, any>({
            query: request => ({
                url: 'auth/login',
                method: 'POST',
                body: request,
                credentials: 'include'
            }),
            async onQueryStarted({ id, ...post }, { dispatch, queryFulfilled }) {
                try {
                    await queryFulfilled;
                    dispatch(setAuthenticated(true)); // TODO: need better way
               } catch {
                    dispatch(setAuthenticated(false));
                }
            },
            invalidatesTags: [{ type: 'Authenticated', id: 'true' }]
        }),

        logout: builder.mutation<any, any>({
            query: request => ({
                url: 'auth/logout',
                method: 'POST',
                body: request,
                credentials: 'include'
            }),
            async onQueryStarted(_, { dispatch, queryFulfilled }) {
                try {
                    await queryFulfilled;
                    dispatch(setAuthenticated(false));
                    dispatch(setProfile(defaultProfile));
                } catch { /* TODO */ }
            },
            invalidatesTags: [
                { type: 'Profile', id: 'logged-in' },
                { type: 'Authenticated', id: 'true' }
            ]
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
    useImportDataMutation,
    useGetProfileQuery,
    useUpdateProfileMutation,
    useGetSessionQuery,
    useLoginMutation,
    useLogoutMutation,
    useRegisterMutation } = diagraphApi;
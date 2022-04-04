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
    tagTypes: ['Profile', 'Events', 'Data'],
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
            async onQueryStarted(api, { dispatch, queryFulfilled }) {
                try {
                    await queryFulfilled;
                    dispatch(setAuthenticated(true));
                } catch { /* do nothing since it's already not authenticated */ }
            }
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
            }
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
            }
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
            invalidatesTags: [{ type: 'Profile', id: 'logged-in' }]
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
    useGetProfileQuery,
    useUpdateProfileMutation,
    useGetSessionQuery,
    useLoginMutation,
    useLogoutMutation,
    useRegisterMutation } = diagraphApi;
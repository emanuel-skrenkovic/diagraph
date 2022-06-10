import { createApi, fetchBaseQuery, FetchBaseQueryMeta } from '@reduxjs/toolkit/query/react';

import { setTags } from 'modules/common';
import { login, logout } from 'modules/auth';
import { setData, setEvents } from 'modules/graph';
import { setProfile, defaultProfile, Profile } from 'modules/profile';
import { CreateEventCommand, Event, GlucoseMeasurement, EventTag, ImportTemplate } from 'types';

export const diagraphApi = createApi({
    reducerPath: 'diagraphApi',
    baseQuery: fetchBaseQuery({
        baseUrl: 'https://localhost:7053', // TODO: configuration
        credentials: 'include'
    }),
    tagTypes: ['Authenticated', 'Profile', 'Events', 'Data', 'Graph', 'ImportTemplates'],
    endpoints: (builder) => ({
        getEvents: builder.query<Event[], any>({
            query: ({from, to}) => ({ url: 'events', params: {from, to} }),
            async onQueryStarted(api, { dispatch, queryFulfilled }) {
                const { data } = await queryFulfilled;
                dispatch(setEvents(data ?? []));
            },
            providesTags: (result) => result
                ? [...result.map(r => ({ type: 'Events' as const, id: r.id }))]
                : [{ type: 'Events' as const, id: 'NONE' }]
        }),
        getEvent: builder.query<any, any>({
            query: id => ({ url: `events/${id}` })
        }),
        createEvent: builder.mutation<void, CreateEventCommand>({
            query: (request) => ({
                url: 'events',
                method: 'POST',
                body: request
            })
        }),
        updateEvent: builder.mutation<any, Event>({
            query: ({ id, ...request }) => ({
                url: `events/${id}`,
                method: 'PUT',
                body: request
            }),
            invalidatesTags: (_result, _error, { id }) => [{ type: 'Events' as const, id }]
        }),
        deleteEvent: builder.mutation<any, number>({
            query: id => ({
                url: `events/${id}`,
                method: 'DELETE',
            }),
            invalidatesTags: (_result, _error, id) => [{ type: 'Events' as const, id }]
        }),

        getTags: builder.query<EventTag[], any>({
            query: () => ({ url: 'events/tags' }),
            async onQueryStarted(api, { dispatch, queryFulfilled }) {
                const { data } = await queryFulfilled;
                dispatch(setTags(data ?? []));
            },
        }),

        importEvents: builder.mutation<any, any>({
            async queryFn(request, queryApi, extraOptions, fetch) {
                const formData = new FormData();
                formData.append('file', request.file, request.file.name);
                formData.append('templateName', request.templateName);

                const response = await fetch({
                    url: 'events/data-import',
                    method: 'POST',
                    body: formData
                });

                // TODO: this bit
                if (response.error) throw response.error;
                return response.data ? { data: response.data } : { error: response.error };
            }
        }),

        importEventsDryRun: builder.mutation<any, any>({
            async queryFn(request, queryApi, extraOptions, fetch) {
                const formData = new FormData();
                formData.append('file', request.file, request.file.name);
                formData.append('templateName', request.templateName);

                const response = await fetch({
                    url: 'events/data-import/dry-run',
                    method: 'POST',
                    body: formData
                });

                // TODO: this bit
                if (response.error) throw response.error;
                return response.data ? { data: response.data } : { error: response.error };
            }
        }),

        getData: builder.query<GlucoseMeasurement[], { from: string, to: string }>({
            query: ({from, to}) => ({ url: 'data', params: {from, to}}),
            async onQueryStarted(request, { dispatch, queryFulfilled }) {
                const { data } = await queryFulfilled;
                dispatch(setData(data))
            },
            providesTags: [{ type: 'ImportTemplates', id: 'all'}],
        }),
        importData: builder.mutation<any, any>({
            async queryFn(file, queryApi, extraOptions, fetch) {
                const formData = new FormData();
                formData.append('file', file, file.name);

                const response = await fetch({
                    url: 'data/imports',
                    method: 'POST',
                    body: formData
                });

                // TODO: this bit
                if (response.error) throw response.error;
                return response.data ? { data: response.data } : { error: response.error };
            },
            // invalidatesTags: [{ type: 'Data', id: 'all' }] // TODO
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

        getImportTemplates: builder.query<ImportTemplate[], any>({
            query: () => ({ url: 'events/import-templates', method: 'GET' }),
            providesTags: [{ type: 'ImportTemplates', id: 'all'}]
        }),
        getImportTemplate: builder.query<ImportTemplate, any>({
            query: id => ({ url: `events/import-templates/${id}`, method: 'GET' }),
            providesTags: [{ type: 'ImportTemplates', id: 'all'}] // TODO: single id
        }),
        createImportTemplate: builder.mutation<string, any>({
            query: template => ({ url: 'events/import-templates', method: 'POST', body: template }),
            transformResponse(apiResponse, meta: FetchBaseQueryMeta) {
                const location = meta?.response?.headers.get('location');
                return location!.split('/').pop()!;
            }
        }),
        updateImportTemplate: builder.mutation<any, { template: ImportTemplate, id: string }>({
            query: ({ template, id }) => ({ url: `events/import-templates/${id}`, method: 'PUT', body: template} ),
            invalidatesTags: [{ type: 'ImportTemplates', id: 'all' }]
        }),

        getSession: builder.query<any, any>({
            query: () => ({ url: 'auth/session' }),
            async onQueryStarted(api, { dispatch, queryFulfilled }) {
                try {
                    await queryFulfilled;
                    dispatch(login())
                } catch {
                    // TODO: What if the account gets deleted/blocked?
                    // Need to think about this.
                    // dispatch(logout())
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
                    dispatch(login()); // TODO: need better way
               } catch { /* Already logged out. */ }
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
                    dispatch(logout());
                    // dispatch(setProfile(defaultProfile));
                } catch { /* TODO */ }
            }
        }),
        register: builder.mutation<any, any>({
            query: request => ({
                url: 'auth/register',
                method: 'POST',
                body: request
            })
        }),

        googleIntegration: builder.query<any, { redirect: string, state?: string }>({
            query: ({redirect, state}) => ({
                url: `/auth/external-access/google/tasks/scopes/required?redirectUri=${redirect}${state && `&state=${state}`}`
            })
        }),
        googleIntegrationConfirm: builder.mutation<any, any>({
            query: request => ({
                url: '/auth/external-access/google/scopes/confirm',
                method: 'PUT',
                body: request,
            })
        })
    })
});

export const {
    // useGetEventQuery,
    useGetEventsQuery,
    useCreateEventMutation,
    useUpdateEventMutation,
    useDeleteEventMutation,
    useGetTagsQuery,
    useImportEventsMutation,
    useImportEventsDryRunMutation,
    useGetDataQuery,
    useImportDataMutation,
    useGetProfileQuery,
    useUpdateProfileMutation,
    useGetImportTemplatesQuery,
    useGetImportTemplateQuery,
    useCreateImportTemplateMutation,
    useUpdateImportTemplateMutation,
    useGetSessionQuery,
    useLoginMutation,
    useLogoutMutation,
    useRegisterMutation,
    useGoogleIntegrationQuery,
    useGoogleIntegrationConfirmMutation } = diagraphApi;
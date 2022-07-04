import React, { useState } from 'react';

import { PrimaryButton, Button, Container, Input, Item } from 'styles';
import { Loader, useAppSelector } from 'modules/common';
import { useUpdateProfileMutation, useGoogleImportFitEventsMutation } from 'services';

export const GoogleIntegrationSettings = () => {
    const profile = useAppSelector(state => state.profile.profile);
    const { googleTaskList } = profile;

    const [taskList, setTaskList] = useState(googleTaskList ?? '');

    const [updateProfile, { isLoading: isUpdateProfileLoading }] = useUpdateProfileMutation();
    const [importFitEvents, { isLoading: isImportLoading }]      = useGoogleImportFitEventsMutation();

    if (isUpdateProfileLoading || isImportLoading) return <Loader />

    return (
        <Container vertical>
            <Container>
                <label htmlFor="googleTaskListName">Google task list name:</label>
                <Input id="googleTaskListName"
                       type="text"
                       value={taskList}
                       onChange={e => setTaskList(e.currentTarget.value)}/>
                <Button type="submit"
                        onClick={() => updateProfile({...profile, googleTaskList: taskList})}>
                    Save
                </Button>
            </Container>
            <Item>
                <PrimaryButton onClick={() => importFitEvents(undefined)}>
                    Import Google Fit events
                </PrimaryButton>
            </Item>
        </Container>
    ) ;
};
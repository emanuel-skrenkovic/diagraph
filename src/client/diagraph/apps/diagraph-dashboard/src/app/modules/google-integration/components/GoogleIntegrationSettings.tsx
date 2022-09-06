import React, { useState, useEffect } from 'react';

import { PrimaryButton, Button, Container, Input, Item } from 'diagraph/styles';
import { Loader, useToast, useAppSelector } from 'diagraph/app/modules/common';
import { useUpdateProfileMutation, useGoogleImportFitEventsMutation } from 'diagraph/app/services';

export const GoogleIntegrationSettings = () => {
    const profile = useAppSelector(state => state.profile.profile);
    const { googleTaskList } = profile;

    const showToast = useToast();

    const [taskList, setTaskList] = useState(googleTaskList ?? '');

    const [updateProfile, { isLoading: isUpdateProfileLoading }] = useUpdateProfileMutation();
    const [importFitEvents, {
        isLoading: isImportLoading,
        isSuccess: isImportSuccess,
        data: importData }] = useGoogleImportFitEventsMutation();

    useEffect(() => {
        if (isImportSuccess) {
            const { count } = importData;

            const message = count == 0
                ? 'Found no new events to import.'
                : `Successfully imported ${count} event/s.`;

            showToast(message);
        }
    }, [isImportSuccess])

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

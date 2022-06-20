import React, { useState } from 'react';

import { Button, Container, Input } from 'styles';
import { Loader, useAppSelector } from 'modules/common';
import { useUpdateProfileMutation } from 'services';

export const GoogleIntegrationSettings = () => {
    const profile = useAppSelector(state => state.profile.profile);
    const { googleTaskList } = profile;

    const [taskList, setTaskList] = useState(googleTaskList ?? '');

    const [updateProfile, { isLoading }] = useUpdateProfileMutation();
    if (isLoading) return <Loader />

    return (
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
    ) ;
};
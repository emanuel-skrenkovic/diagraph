import React, { useState, useEffect } from 'react';
import { useSelector, useDispatch } from 'react-redux';

import { Button, Centered, Container, Input } from 'styles';
import { RootState } from 'store';
import { setKey } from 'modules/google-integration';
import { handleQuery, Loader } from 'modules/common';
import { useGetProfileQuery, useGoogleIntegrationQuery, useUpdateProfileMutation } from 'services';

export const GoogleIntegration: React.FC = () => {
    const profile        = useSelector((state: RootState) => state.profile.profile);
    const idempotencyKey = useSelector((state: RootState) => state.googleIntegration.idempotencyKey);

    const { googleIntegration, googleTaskList } = profile;

    const [taskList, setTaskList]                         = useState(googleTaskList);
    const [requestedIntegration, setRequestedIntegration] = useState(false)

    const { isSuccess, isLoading: profileLoading } = useGetProfileQuery(undefined);
    const [updateProfile] = useUpdateProfileMutation();
    const integrationQuery = useGoogleIntegrationQuery({
            redirect: encodeURIComponent('http://localhost:3000/integrations/google/confirm'), // TODO: url from where?
            state: idempotencyKey
        },
        { skip: !requestedIntegration }
    );

    const dispatch = useDispatch();
    useEffect(() => {
        if (!googleIntegration && !idempotencyKey) {
            dispatch(setKey(window.crypto.randomUUID()));
        }

        setTaskList(googleTaskList);
    }, [isSuccess]);

    function renderNotIntegrated() {
        return (
            <Button onClick={() => setRequestedIntegration(true)}>
                Add Google integration
            </Button>
        );
    }

    const queryLoading = handleQuery(
        integrationQuery,
        ({redirectUri}) => requestedIntegration && window.location.replace(redirectUri)
    );
    if (queryLoading)   return <Loader />;
    if (profileLoading) return <Loader />;

    function renderIntegrated() {
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
        );
    }

    return (
        <Centered>
            {googleIntegration ? renderIntegrated() : renderNotIntegrated()}
        </Centered>
    );
};
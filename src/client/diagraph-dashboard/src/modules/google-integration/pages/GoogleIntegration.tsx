import React, { useState, useEffect } from 'react';
import { useSelector } from 'react-redux';

import { RootState } from 'store';
import { Container, handleQuery, Loader } from 'modules/common';
import { useGetProfileQuery, useGoogleIntegrationQuery, useUpdateProfileMutation } from 'services';

export const GoogleIntegration: React.FC = () => {
    const profile = useSelector ((state: RootState) => state.profile.profile);
    const { googleIntegration, googleTaskList } = profile;

    const { isSuccess, isLoading: profileLoading } = useGetProfileQuery(undefined);

    useEffect(() => {
        setTaskList(googleTaskList);
    }, [isSuccess, googleIntegration, googleTaskList]);

    const [taskList, setTaskList]                         = useState(googleTaskList);
    const [requestedIntegration, setRequestedIntegration] = useState(false)

    const [updateProfile] = useUpdateProfileMutation();
    const integrationQuery = useGoogleIntegrationQuery(
        encodeURIComponent('http://localhost:3000/integrations/google/confirm'), // TODO: url from where?
        { skip: !requestedIntegration }
    );

    const queryLoading = handleQuery(
        integrationQuery,
        ({redirectUri}) => requestedIntegration && window.location.replace(redirectUri)
    );

    if (queryLoading)   return <Loader />;
    if (profileLoading) return <Loader />;

    function renderNotIntegrated() {
        return (
            <button className="button" onClick={() => setRequestedIntegration(true)}>
                Add Google integration
            </button>
        );
    }

    function renderIntegrated() {
        return (
            <Container>
                <label htmlFor="googleTaskListName">Google task list name:</label>
                <input id="googleTaskListName"
                       type="text"
                       value={taskList}
                       onChange={e => setTaskList(e.currentTarget.value)}/>
                <button className="button"
                        type="submit"
                        onClick={() => updateProfile({...profile, googleTaskList: taskList})}>
                    Save
                </button>
            </Container>
        );
    }

    return (
        <div className="centered">
            {googleIntegration ? renderIntegrated() : renderNotIntegrated()}
        </div>
    );
};
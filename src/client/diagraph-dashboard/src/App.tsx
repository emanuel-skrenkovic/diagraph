import React from 'react';
import { Dashboard } from 'modules/graph';
import { useCreateEventMutation } from 'services';

function App() {
    const [createEvent, _] = useCreateEventMutation();

    return (
        <>
            <Dashboard />

        </>
    );
}

export default App;

import React from 'react';
import { Provider } from 'react-redux';
import { createRoot } from 'react-dom/client';

import { Global } from 'diagraph/styles';
import App from 'diagraph/App';
import { store } from 'diagraph/store';

const container = document.getElementById('root');
const root = createRoot(container!);
root.render(
    <React.StrictMode>
        <Global />
        <Provider store={store}>
            <App />
        </Provider>
    </React.StrictMode>
);

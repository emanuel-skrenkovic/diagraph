import React from 'react';
import { createRoot } from 'react-dom/client';

import { Global } from 'styles';
import App from 'App';
import { store } from 'store';
import { Provider } from 'react-redux';

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
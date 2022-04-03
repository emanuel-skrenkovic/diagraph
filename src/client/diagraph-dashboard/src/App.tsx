import React from 'react';
import { useSelector } from 'react-redux';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';

import { RootState } from 'store';
import { Dashboard } from 'modules/graph';
import { useGetSessionQuery } from 'services';
import { Login, Register } from 'modules/auth';
import { Loader } from 'modules/common';

import 'App.css';

function App() {
    const authenticated = useSelector((state: RootState) => state.auth.authenticated);
    const { isLoading } = useGetSessionQuery(undefined);

    if (isLoading) return <Loader />;

    return (
        <div className="container">
            <BrowserRouter>
                <Routes>
                    <Route path="login" element={<Login />} />
                    <Route path="register" element={<Register />} />
                    <Route path="/" element={authenticated ? <Dashboard /> : <Navigate to="/login" />} />
                </Routes>
            </BrowserRouter>
        </div>
    );
}

export default App;

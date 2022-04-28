import React from 'react';
import { useSelector } from 'react-redux';
import { BrowserRouter, Routes, Route, Link } from 'react-router-dom';

import { RootState } from 'store';
import { useGetSessionQuery, useLogoutMutation } from 'services';

import { Import } from 'modules/import';
import { Dashboard } from 'modules/graph';
import { Login, Register } from 'modules/auth';
import { NavigationBar } from 'modules/navigation';
import { Loader, ProtectedRoute } from 'modules/common';
import { ImportEvents, Templates } from 'modules/import-events';

import 'App.css';

function App() {
    const authenticated = useSelector((state: RootState) => state.auth.authenticated);

    const { data, isLoading: isSessionLoading }    = useGetSessionQuery(undefined);
    const [logout, { isLoading: isLogoutLoading }] = useLogoutMutation(undefined);

    if (isSessionLoading || isLogoutLoading) return <Loader />;

    return (
        <div className="container wide">
            <BrowserRouter>
                <div className="container horizontal wide">
                    {authenticated &&
                        <NavigationBar>
                            <div className="container horizontal wide">
                                <div className="item">
                                    <span><b>Diagraph</b></span>
                                    <button className="button red"
                                            style={{
                                                width: "fit-content",
                                                padding: "5px 10px 5px 10px",
                                                float: "right",
                                                marginLeft: "70%"
                                            }}
                                            onClick={logout}>
                                        Log out
                                    </button>
                                    <span>Hello, {data?.userName}</span>
                                </div>
                                <div className="container wide item">
                                    <Link className="item" to="/">Dashboard</Link>
                                    <Link className="item" to="/import">Import</Link>
                                    <Link className="item" to="/import-events">Import Events</Link>
                                </div>
                            </div>
                        </NavigationBar>
                    }
                    <Routes>
                        <Route path="login" element={<Login />} />
                        <Route path="register" element={<Register />} />

                        <Route index element={
                            <ProtectedRoute condition={authenticated} fallback="/login">
                                <Dashboard />
                            </ProtectedRoute>} />
                        <Route path="import" element={
                            <ProtectedRoute condition={authenticated} fallback="/login">
                                <Import />
                            </ProtectedRoute>} />
                        <Route path="import-events" element={
                            <ProtectedRoute condition={authenticated} fallback="/login">
                                <ImportEvents />
                            </ProtectedRoute>
                        } />
                        <Route path="templates" element={
                            <ProtectedRoute condition={authenticated} fallback="/login">
                                <Templates />
                            </ProtectedRoute>
                        } />
                    </Routes>
                </div>
            </BrowserRouter>
        </div>
    );
}

export default App;

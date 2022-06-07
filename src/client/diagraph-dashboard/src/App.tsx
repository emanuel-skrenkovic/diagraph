import React from 'react';
import { useSelector } from 'react-redux';
import { BrowserRouter, Routes, Route, Link } from 'react-router-dom';

import { RootState } from 'store';
import { useLogoutMutation } from 'services';

import { Import } from 'modules/import';
import { Dashboard } from 'modules/graph';
import { Login, Register } from 'modules/auth';
import { NavigationBar } from 'modules/navigation';
import { ErrorBoundary, Loader, ProtectedRoute } from 'modules/common';
import { ImportEvents, Templates, EditTemplate } from 'modules/import-events';
import { GoogleIntegration, GoogleIntegrationConfirm } from 'modules/google-integration';

import 'App.css';

function App() {
    const authenticated = useSelector((state: RootState) => state.auth.authenticated);

    const [logout, { isLoading: isLogoutLoading }] = useLogoutMutation(undefined);
    if (isLogoutLoading) return <Loader />;

    return (
        <div className="container wide">
            <ErrorBoundary>
                <BrowserRouter>
                    <div className="container vertical wide">
                        {authenticated &&
                            <NavigationBar>
                                <div className="container vertical wide">
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
                                        {/*<span>Hello, {data?.userName}</span>*/}
                                    </div>
                                    <div className="container wide item">
                                        <Link className="item" to="/">Dashboard</Link>
                                        <Link className="item" to="/import">Import</Link>
                                        <Link className="item" to="/import-events">Import Events</Link>
                                        <Link className="item" to="/integrations/google">Google integration</Link>
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
                            <Route path="templates/add" element={
                                <ProtectedRoute condition={authenticated} fallback="/login">
                                    <Templates />
                                </ProtectedRoute>
                            } />
                            <Route path="templates/edit" element={
                                <ProtectedRoute condition={authenticated} fallback="/login">
                                    <EditTemplate />
                                </ProtectedRoute>
                            } />
                            <Route path="integrations/google" element={
                                <ProtectedRoute condition={authenticated} fallback="/login">
                                    <GoogleIntegration />
                                </ProtectedRoute>
                            } />
                            <Route path="integrations/google/confirm" element={
                                <GoogleIntegrationConfirm />
                                // <ProtectedRoute condition={authenticated} fallback="/login">
                                // </ProtectedRoute>
                            } />
                        </Routes>
                    </div>
                </BrowserRouter>
            </ErrorBoundary>
        </div>
    );
}

export default App;

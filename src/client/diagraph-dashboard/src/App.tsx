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

import 'App.css';

function App() {
    const authenticated = useSelector((state: RootState) => state.auth.authenticated);

    const { data, isLoading: isSessionLoading }    = useGetSessionQuery(undefined);
    const [logout, { isLoading: isLogoutLoading }] = useLogoutMutation(undefined);

    if (isSessionLoading || isLogoutLoading) return <Loader />;

    return (
        <div className="container">
            <BrowserRouter>
                <div className="container horizontal">
                    {authenticated &&
                        <NavigationBar>
                            <div className="container horizontal" style={{width: "100%"}}>
                                <h1 className="item">Diagraph</h1>
                                <div className="container item">
                                    <Link className="item" to="/">Dashboard</Link>
                                    <Link className="item" to="/import">Import</Link>
                                </div>
                                <div className="item">
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
                    </Routes>
                </div>
            </BrowserRouter>
        </div>
    );
}

export default App;

import React from 'react';
import { useSelector } from 'react-redux';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';

import { RootState } from 'store';
import { Dashboard } from 'modules/graph';
import { useGetSessionQuery, useLogoutMutation } from 'services';
import { Loader } from 'modules/common';
import { Login, Register } from 'modules/auth';
import { NavigationBar } from 'modules/navigation';

import 'App.css';

function App() {
    const authenticated = useSelector((state: RootState) => state.auth.authenticated);
    const { data, isLoading, isSuccess } = useGetSessionQuery(undefined);
    const [logout, { isLoading: isLogoutLoading }] = useLogoutMutation(undefined);

    if (isLoading || isLogoutLoading) return <Loader />;

    return (
        <div className="container">
            <BrowserRouter>
                <div className="container horizontal">
                    {authenticated &&
                        <NavigationBar>
                            <div className="container horizontal" style={{width: "100%"}}>
                                <h1 className="item">Diagraph</h1>
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
                                    <span>Hello, {isSuccess && data.userName}</span>
                                </div>
                            </div>
                        </NavigationBar>
                    }
                    <Routes>
                        <Route path="login" element={<Login />} />
                        <Route path="register" element={<Register />} />
                        <Route path="/"
                               element={authenticated ? <Dashboard /> : <Navigate to="/login" />} />
                    </Routes>
                </div>
            </BrowserRouter>
        </div>
    );
}

export default App;

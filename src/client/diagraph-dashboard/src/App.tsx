import React from 'react';
import { useSelector } from 'react-redux';
import { BrowserRouter, Routes, Route, Link } from 'react-router-dom';

import { RedButton, Container, Item, Title, Divider } from 'styles';
import { RootState } from 'store';
import { useLogoutMutation } from 'services';

import { Import } from 'modules/import';
import { Dashboard } from 'modules/graph';
import { Login, Register } from 'modules/auth';
import { NavigationBar } from 'modules/navigation';
import { ErrorBoundary, Loader, ProtectedRoute } from 'modules/common';
import { ImportEvents, Templates, EditTemplate } from 'modules/import-events';
import { GoogleIntegration, GoogleIntegrationConfirm } from 'modules/google-integration';

function App() {
    const authenticated = useSelector((state: RootState) => state.auth.authenticated);

    const [logout, { isLoading: isLogoutLoading }] = useLogoutMutation(undefined);
    if (isLogoutLoading) return <Loader />;

    return (
            <ErrorBoundary>
                <BrowserRouter>
                    <Container vertical wide>
                        <Title>Diagraph</Title>
                        <Divider style={{margin:"initial",width:"6em"}}/>
                        {authenticated &&
                            <>
                                <RedButton style={{marginLeft:"90%", whiteSpace:"nowrap"}} onClick={logout}>
                                    Log out
                                </RedButton>
                                <NavigationBar>
                                    <Container wide>
                                        <Item as={Link} to="/">Dashboard</Item>
                                        <Item as={Link} to="/import">Import</Item>
                                        <Item as={Link} to="/import-events">Import Events</Item>
                                        <Item as={Link} to="/integrations/google">Google integration</Item>
                                    </Container>
                                </NavigationBar>
                            </>
                        }
                        <Container>
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
                                    <ProtectedRoute condition={authenticated} fallback="/login">
                                        <GoogleIntegrationConfirm />
                                    </ProtectedRoute>
                                } />
                            </Routes>
                        </Container>
                    </Container>
                </BrowserRouter>
            </ErrorBoundary>
    );
}

export default App;

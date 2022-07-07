import React from 'react';
import { BrowserRouter, Routes, Route, Link } from 'react-router-dom';

import { DangerButton, Container, Item, Title, Divider } from 'styles';
import { useLogoutMutation } from 'services';

import { Import } from 'modules/import';
import { Dashboard } from 'modules/graph';
import { Login, Register } from 'modules/auth';
import { NavigationBar } from 'modules/navigation';
import { ErrorBoundary, Toaster, Loader, useAppSelector, withAuth } from 'modules/common';
import { ImportEvents, Templates, EditTemplate } from 'modules/import-events';
import { GoogleIntegration, GoogleIntegrationConfirm } from 'modules/google-integration';

const DashboardWithAuth                = withAuth(Dashboard);
const ImportWithAuth                   = withAuth(Import);
const ImportEventsWithAuth             = withAuth(ImportEvents);
const TemplatesWithAuth                = withAuth(Templates);
const EditTemplateWithAuth             = withAuth(EditTemplate);
const GoogleIntegrationWithAuth        = withAuth(GoogleIntegration);
const GoogleIntegrationConfirmWithAuth = withAuth(GoogleIntegrationConfirm);

function App() {
    const authenticated = useAppSelector(state => state.auth.authenticated);

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
                                <DangerButton style={{marginLeft:"90%", whiteSpace:"nowrap"}} onClick={logout}>
                                    Log out
                                </DangerButton>
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
                                <Route index element={<DashboardWithAuth />} />
                                <Route path="import" element={<ImportWithAuth />} />
                                <Route path="import-events" element={<ImportEventsWithAuth />} />
                                <Route path="templates/add" element={<TemplatesWithAuth />} />
                                <Route path="templates/edit" element={<EditTemplateWithAuth />} />
                                <Route path="integrations/google" element={<GoogleIntegrationWithAuth />} />
                                <Route path="integrations/google/confirm" element={<GoogleIntegrationConfirmWithAuth />} />
                            </Routes>
                        </Container>
                        <Toaster />
                    </Container>
                </BrowserRouter>
            </ErrorBoundary>
    );
}

export default App;

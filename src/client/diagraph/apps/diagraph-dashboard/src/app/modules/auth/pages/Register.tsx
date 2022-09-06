import React from 'react';
import { Link } from 'react-router-dom';

import { Box, Container, Item } from 'diagraph/styles';
import { Loader } from 'diagraph/app/modules/common';
import { RegisterForm } from 'diagraph/app/modules/auth';
import { useRegisterMutation } from 'diagraph/app/services';

export const Register = () => {
    const [register, { isLoading, isSuccess }] = useRegisterMutation();

    if (isLoading) return <Loader />;

    return (
        <Container vertical>
            <Box>
                <RegisterForm onSubmit={(email, password) => register({ email, password })} />
                {isSuccess &&
                    <Item>
                        <span>{isSuccess && 'Thank you for registering, an email has been sent to your account for confirmation.'}</span>
                        <br/>
                        <Link to="/login">Log in.</Link>
                    </Item>
                }
            </Box>
        </Container>
    );
};

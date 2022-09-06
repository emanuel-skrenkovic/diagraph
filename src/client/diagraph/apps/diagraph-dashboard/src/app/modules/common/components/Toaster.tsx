import React, { useState, useEffect } from 'react';
import ReactDOM from 'react-dom';

import { Container, Item, Toast } from 'diagraph/styles';
import { useAppSelector } from 'diagraph/app/modules/common';

type ToastStatus = {
    message: string;
    active: boolean;
}

export const Toaster = () => {
    const toasts = useAppSelector(state => state.shared.shownToasts);
    const [shownToasts, setShownToasts] = useState<ToastStatus[]>([]);

    useEffect(() => {
        const activeToasts = toasts
            .filter(t => !shownToasts.map(st => st.message).includes(t))
            .map(t => ({ message: t, active: true } as ToastStatus));

        const inactive = shownToasts.map(t => {
            return {
                message: t.message,
                active: toasts.includes(t.message)
            } as ToastStatus;
        })

        setShownToasts(activeToasts.concat(inactive));
    }, [toasts]);

    const removeToast = (toast: string) => setShownToasts(
        shownToasts.filter(t => toast != t.message)
    );

    const element = document.getElementById("toast");
    if (element == null) return null;

    return ReactDOM.createPortal(
        (
            <div style={{position:"fixed", bottom:"10%", right:"10%"}}>
                <Container vertical>
                    {shownToasts.map(t => (
                        <Item>
                            <Toast fadingOut={!t.active}
                                   key={t.message}
                                   onTransitionEnd={() => removeToast(t.message)}>
                                {t.message}
                            </Toast>
                        </Item>
                    ))}
                </Container>
            </div>
        ),
        element
    )
};

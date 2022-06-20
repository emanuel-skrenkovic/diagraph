import React, { useState, useEffect, MouseEvent } from 'react';

import { PrimaryButton, Container, Input } from 'styles';

export type TagProps = {
    value: string;
    disabled?: boolean;
    onChange?: (value: string) => void;
}

export const Tag = ({ value, disabled, onChange }: TagProps) => {
    const [isEditing, setIsEditing] = useState(!disabled ?? false);

    useEffect(() => setIsEditing(!disabled ?? false), [disabled])

    const onEdit = (e: MouseEvent<HTMLButtonElement>) => {
        e.preventDefault();
        setIsEditing(!isEditing);
    }

    return (
        <Container>
           <Input type="text"
                  style={{width:"100%"}}
                  value={value}
                  disabled={!isEditing}
                  onChange={e => onChange && onChange(e.currentTarget.value)}/>
            <PrimaryButton onClick={onEdit}>
                {isEditing ? 'Save' : 'Edit'}
            </PrimaryButton>
        </Container>
    )
};
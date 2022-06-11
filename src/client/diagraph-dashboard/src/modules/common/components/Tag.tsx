import React, { useState, useEffect, MouseEvent } from 'react';

import { BlueButton, Input } from 'styles';

export interface TagProps {
    value: string;
    disabled?: boolean;
    onChange?: (value: string) => void;
}

export const Tag: React.FC<TagProps> = ({ value, disabled, onChange }) => {
    const [isEditing, setIsEditing] = useState(!disabled ?? false);

    useEffect(() => setIsEditing(!disabled ?? false), [disabled])

    function onEdit(e: MouseEvent<HTMLButtonElement>) {
        e.preventDefault();
        setIsEditing(!isEditing);
    }

    return (
        <span className="container button">
                <Input type="text"
                       style={{width:"100%"}}
                       value={value}
                       disabled={!isEditing}
                       onChange={e => onChange && onChange(e.currentTarget.value)}/>
                <BlueButton onClick={onEdit}>
                    Edit
                </BlueButton>
        </span>
    )
};
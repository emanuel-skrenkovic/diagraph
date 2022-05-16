import React, { useState, useEffect, MouseEvent } from 'react';

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
                <input type="text"
                       style={{width:"100%"}}
                       value={value}
                       disabled={!isEditing}
                       placeholder="..."
                       onChange={e => onChange && onChange(e.currentTarget.value)}/>
                <button className="button blue" onClick={onEdit}>
                    E
                </button>
        </span>
    )
};
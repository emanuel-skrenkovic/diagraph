// noinspection JSDeprecatedSymbols

import React, { useState, useEffect, MouseEvent } from 'react';

import { EventTag } from 'types';
import { useGetTagsQuery } from 'services';
import { For, Loader, MultiSelectForm, Tag } from 'modules/common';

export interface TagSelectorProps {
    initialSelectedTags: EventTag[];
    onChange: (selectedTags: EventTag[]) => void;
}

export const TagSelector: React.FC<TagSelectorProps> = ({ initialSelectedTags, onChange }) => {
    const [availableTags, setAvailableTags] = useState<EventTag[]>([]);
    const [selectedTags, setSelectedTags]   = useState<EventTag[]>(initialSelectedTags);

    const {
        data:      tagsData,
        isLoading: isTagsLoading,
        isSuccess: isTagsSuccess,
        isError:   isTagsError,
        error:     tagsError
    } = useGetTagsQuery(undefined);

    useEffect(() => {
        if (!tagsData) return;
        const newAvailableTags = tagsData.filter(
            t => !selectedTags.map(st => st.name).includes(t.name)
        );

        setAvailableTags(newAvailableTags);
    }, [isTagsSuccess, tagsData, selectedTags]);

    function updateSelectedTags(updated: EventTag[]) {
        setSelectedTags(updated);
        setAvailableTags(
            tagsData!.filter(
                t => !updated.map(st => st.name).includes(t.name)
            )
        );

        onChange(updated);
    }

    function addNewTags(newTags: EventTag[]) {
        const updated = [
            ...selectedTags,
            ...newTags.filter(
                t => !selectedTags.map(st => st.name).includes(t.name)
        )];

        updateSelectedTags(updated);
    }

    function removeTag(e: MouseEvent<HTMLButtonElement>, index: number) {
        e.preventDefault();

        const updated = [...selectedTags];
        updated.splice(index, 1);

        updateSelectedTags(updated);
    }

    function newTagForm(e: MouseEvent<HTMLButtonElement>) {
        e.preventDefault();
        setSelectedTags([...selectedTags, { name: '' } as EventTag]);
    }

    function onTagChanged(newValue: string, index: number) {
        const updated = [...selectedTags];
        updated[index] = { name: newValue } as EventTag;

        setSelectedTags(updated);
    }

    if (isTagsLoading) return <Loader />;
    if (isTagsError) console.error(tagsError); // TODO

    return (
        <div className="container vertical">
            <label>Available Tags</label>
            <div className="item" style={{width:"200px"}}>
                <MultiSelectForm options={availableTags}
                                 keySelector={(t: EventTag) => t.name}
                                 onAdd={tags => addNewTags(tags)} />
            </div>
            <div className="item">
                <label>Selected Tags</label>
                <For each={selectedTags} onEach={(tag, index) => (
                    <div className="container" key={index}>
                        <Tag value={tag.name} onChange={newValue => onTagChanged(newValue, index)} disabled />
                        <button className="button blue" onClick={e => removeTag(e, index)}>
                            X
                        </button>
                    </div>
                )} />
            </div>
            <button className="button blue item" onClick={newTagForm}>
                +
            </button>
        </div>
    );
};
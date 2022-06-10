// noinspection JSDeprecatedSymbols

import React, { useState, useEffect, MouseEvent } from 'react';
import { useSelector } from 'react-redux';

import { EventTag } from 'types';
import { RootState } from 'store';
import { useGetTagsQuery} from 'services';
import { Container, Item, For, Loader, MultiSelectForm, Tag } from 'modules/common';

export interface TagSelectorProps {
    initialSelectedTags: EventTag[];
    onChange: (selectedTags: EventTag[]) => void;
}

export const TagSelector: React.FC<TagSelectorProps> = ({ initialSelectedTags, onChange }) => {
    const tags = useSelector((state: RootState) => state.shared.tags);

    const [availableTags, setAvailableTags] = useState<EventTag[]>([]);
    const [selectedTags, setSelectedTags]   = useState<EventTag[]>(initialSelectedTags);

    useEffect(() => {
        if (!tags) return;
        const newAvailableTags = tags.filter(
            t => !selectedTags.map(st => st.name).includes(t.name)
        );

        setAvailableTags(newAvailableTags);
        setSelectedTags(initialSelectedTags);
    }, [initialSelectedTags, tags, selectedTags]);

    function updateSelectedTags(updated: EventTag[]) {
        setSelectedTags(updated);
        setAvailableTags(
            tags!.filter(
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

        const updated = [...selectedTags, { name: '' } as EventTag];
        setSelectedTags(updated);
        onChange(updated);
    }

    function onTagChanged(newValue: string, index: number) {
        const updated = [...selectedTags];
        updated[index] = { name: newValue } as EventTag;

        setSelectedTags(updated);
        onChange(updated);
    }

    {
        const { isLoading, isError, error } = useGetTagsQuery(undefined);

        if (isError)   console.error(error);
        if (isLoading) return <Loader/>;
    }

    return (
        <Container vertical>
            <label>Available Tags</label>
            <div className="item" style={{width:"inherit"}}>
                <MultiSelectForm options={availableTags}
                                 keySelector={(t: EventTag) => t.name}
                                 onAdd={tags => addNewTags(tags)} />
            </div>
            <Item>
                <label>Selected Tags</label>
                <For each={selectedTags} onEach={(tag, index) => (
                    <div className="container" key={index}>
                        <Tag value={tag.name}
                             onChange={newValue => onTagChanged(newValue, index)}
                             disabled />
                        <button className="button red" onClick={e => removeTag(e, index)}>
                            X
                        </button>
                    </div>
                )} />
            </Item>
            <button className="button blue item" onClick={newTagForm}>
                Add
            </button>
        </Container>
    );
};
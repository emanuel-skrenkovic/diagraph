// noinspection JSDeprecatedSymbols

import React, { useState, useEffect, MouseEvent } from 'react';

import { PrimaryButton, DangerButton, Container, Item } from 'styles';
import { EventTag } from 'types';
import { useGetTagsQuery} from 'services';
import { For, Loader, MultiSelectForm, Tag, useAppSelector } from 'modules/common';

export type TagSelectorProps = {
    initialSelectedTags: EventTag[];
    onChange: (selectedTags: EventTag[]) => void;
}

export const TagSelector = ({ initialSelectedTags, onChange }: TagSelectorProps) => {
    const tags = useAppSelector(state => state.shared.tags);

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

    const { isLoading } = useGetTagsQuery(undefined);
    if (isLoading) return <Loader/>;

    return (
        <Container vertical>
            <label>Available Tags</label>
            <div style={{width:"inherit"}}>
                <MultiSelectForm options={availableTags}
                                 keySelector={(t: EventTag) => t.name}
                                 onAdd={tags => addNewTags(tags)} />
            </div>
            <label>Selected Tags</label>
            <For each={selectedTags} onEach={(tag, index) => (
                <Container key={index}>
                    <Tag value={tag.name}
                         onChange={newValue => onTagChanged(newValue, index)}
                         disabled />
                    <DangerButton onClick={e => removeTag(e, index)}>
                        X
                    </DangerButton>
                </Container>
            )} />
            <Item as={PrimaryButton} onClick={newTagForm}>Add</Item>
        </Container>
    );
};
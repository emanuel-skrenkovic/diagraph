import React from 'react';

import { Container, DangerButton, Item, PrimaryButton } from 'styles';
import { For } from 'modules/common';
import { TemplateHeaderMapping } from 'types';

export type HeaderMappingsTableProps = {
    values: TemplateHeaderMapping[];
    onEdit: (m: TemplateHeaderMapping) => void;
    onRemove: (m: TemplateHeaderMapping) => void;
};

export const HeaderMappingsTable = ({ values, onEdit, onRemove }: HeaderMappingsTableProps) => (
    <table>
        <thead>
        <tr>
            <th>Header</th>
            <th></th>
        </tr>
        </thead>
        <tbody>
        <For each={values ?? []} onEach={(mapping, index) => (
            <tr key={index}>
                <td>{mapping.header}</td>
                <td>
                    <Container>
                        <Item as={PrimaryButton}
                              onClick={() => {onEdit(mapping)}}>
                            Edit
                        </Item>
                        <Item as={DangerButton}
                              onClick={() => onRemove(mapping)}>
                            Remove
                        </Item>
                    </Container>
                </td>
            </tr>
        )} />
        </tbody>
    </table>
);

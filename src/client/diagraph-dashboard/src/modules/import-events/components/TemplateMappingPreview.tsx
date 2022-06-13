// noinspection JSDeprecatedSymbols

import React, { useState, useEffect } from 'react';

import { ScrollBar } from 'styles';
import { Event } from 'types';
import { CsvPreview } from 'modules/import-events';
import { For, Loader } from 'modules/common';
import { useImportEventsDryRunMutation } from 'services';

export interface TemplateMappingPreviewProps {
    csvFile: File;
    template: string;
}

export const TemplateMappingPreview: React.FC<TemplateMappingPreviewProps> = ({ csvFile, template }) => {
    const [events, setEvents]           = useState<Event[]>([])
    const [fulfilledAt, setFulfilledAt] = useState(0);

    const [importEventsDryRun, importEventsDryRunResult]     = useImportEventsDryRunMutation(undefined);
    const { data, isLoading, isSuccess, fulfilledTimeStamp } = importEventsDryRunResult;

    useEffect(() => {
        importEventsDryRun({ file: csvFile, templateName: template });
    }, [csvFile, template, importEventsDryRun]);

    if (isSuccess && fulfilledTimeStamp > fulfilledAt) {
        setEvents(data);
        setFulfilledAt(fulfilledTimeStamp);
    }

    if (isLoading)                       return <Loader />;
    if (!csvFile || events.length === 0) return null;

    return (
        <>
            <h3>Csv data</h3>
            <CsvPreview csvFile={csvFile}/>
            <h3>Mapped events</h3>
            <ScrollBar style={{maxHeight:"500px"}}>
                <table>
                    <thead>
                    <tr>
                        <th>Occurred At</th>
                        <th>Text</th>
                        <th>Tags</th>
                    </tr>
                    </thead>
                    <tbody>
                    <For each={events.slice(0, 100)} onEach={(event, eventIndex) => (
                        <tr key={eventIndex}>
                            <td>{event.occurredAtUtc.toString()}</td>
                            <td>{event.text}</td>
                            <td>
                                [<For each={event.tags ?? []} onEach={(tag, tagIndex) => (
                                <span key={tagIndex}>{`${tag.name} `}</span>
                            )} />]
                            </td>
                        </tr>
                    )}/>
                    </tbody>
                </table>
            </ScrollBar>
        </>
    );
};
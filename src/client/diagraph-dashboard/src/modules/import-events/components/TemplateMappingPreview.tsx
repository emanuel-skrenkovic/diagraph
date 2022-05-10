import React, { useState, useEffect } from 'react';

import { Event } from 'types';
import { CsvPreview } from 'modules/import-events';
import { For, ScrollBar, Loader } from 'modules/common';
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

    if (isLoading) return <Loader />;

    return (
        <>
            {csvFile && events.length > 0 && (
                <div className="item">
                    <h3>Csv data</h3>
                    <CsvPreview csvFile={csvFile}/>
                </div>
            )}
            {csvFile && events.length > 0 && (
                <div className="item">
                    <h3>Mapped events</h3>
                    <ScrollBar heightPx={500}>
                        <table>
                            <thead>
                            <tr>
                                <th>Occurred At</th>
                                <th>Text</th>
                                <th>Tags</th>
                            </tr>
                            </thead>
                            <tbody>
                            <For each={events.slice(0, 100)} onEach={(e, i) => (
                                <tr key={i}>
                                    <td>{e.occurredAtUtc}</td>
                                    <td>{e.text}</td>
                                    <td>
                                        [<For each={e.tags ?? []} onEach={(t, i) => (
                                            <span key={i}>{`${t.name} `}</span>
                                        )} />]
                                    </td>
                                </tr>
                            )}/>
                            </tbody>
                        </table>
                    </ScrollBar>
                </div>
            )}
        </>
    );
};
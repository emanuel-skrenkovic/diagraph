import React, { useState, ChangeEvent } from 'react';
import { Link } from 'react-router-dom';
import * as Papa from 'papaparse';

import { Event } from 'types';
import { FileUploadForm, For, Loader } from 'modules/common';
import { useGetImportTemplatesQuery, useImportEventsDryRunMutation } from 'services';

import 'App.css';

export const ImportEvents = () => {
    const { data, isLoading, isError, error } = useGetImportTemplatesQuery(undefined);
    const [
        importEventsDryRun,
        { data: dryRunData, isLoading: isDryRunLoading }
    ] = useImportEventsDryRunMutation(undefined);

    const [csvData, setCsvData] = useState<string[][]>([]);
    const [exampleEvents, setExampleEvents] = useState<Event[]>(dryRunData ?? []);

    const [file, setFile] = useState<File | undefined>();
    const [selectedTemplate, setSelectedTemplate] = useState<string | undefined>(undefined);

    async function checkTemplateMapping() {
        if (!file) return;

        const csvData = Papa.parse(await file.text());
        if (csvData) {
            setCsvData(csvData.data.slice(0, 6) as string[][]);
        }

        if (selectedTemplate) importEventsDryRun({ file, templateName: selectedTemplate });
    }

    if (isLoading || isDryRunLoading) return <Loader />
    if (isError) console.error(error); // TODO

    return (
        <div className="container horizontal">
            <Link to="/templates">Create Import Template</Link>
            <div className="container">
                <div className="item">
                    <h3>Data</h3>
                    <FileUploadForm onSubmit={() => console.log('Uploading...')}
                                    onSelect={setFile}/>
                </div>
                <div className="item">
                    <h3>Templates</h3>
                    <form className="container horizontal">
                        <label htmlFor="newTemplateName">Name</label>
                        <input id="newTemplateName" type="text" />
                        <label htmlFor="selectTemplate">Templates</label>
                        <select id="selectTemplate" onChange={e => setSelectedTemplate(e.currentTarget.value)}>
                            <option key={undefined}></option>
                            <For each={data ?? []} onEach={t => (
                                <option key={t.id}>
                                    {t.name}
                                </option>
                            )} />
                        </select>
                    </form>
                </div>
            </div>
            <button className="button blue" onClick={checkTemplateMapping}>Check template mapping</button>
            <div className="item">
                {csvData.length > 0 && (
                    <>
                        <h3>Csv data</h3>
                        <table>
                            <thead>
                            <tr>
                                <For each={csvData[0]} onEach={h => <th>{h}</th>}/>
                            </tr>
                            </thead>
                            <tbody>
                            <For each={csvData.slice(1, 6)} onEach={row => (
                                    <tr>
                                        <For each={row} onEach={val => <td>{val}</td>} />
                                    </tr>
                                )} />
                            </tbody>
                        </table>
                    </>
                )}
            </div>
            <div className="item">
                {exampleEvents.length > 0 && (
                    <>
                    <h3>Mapped events</h3>
                    <table>
                        <thead>
                        <tr>
                            <th>Occurred At</th>
                            <th>Text</th>
                        </tr>
                        </thead>
                        <tbody>
                            <For each={exampleEvents} onEach={e => (
                                <tr>
                                    <td>{e.occurredAtUtc}</td>
                                    <td>{e.text}</td>
                                </tr>
                            )}/>
                        </tbody>
                    </table>
                </>
                )}
            </div>
        </div>
    )
}
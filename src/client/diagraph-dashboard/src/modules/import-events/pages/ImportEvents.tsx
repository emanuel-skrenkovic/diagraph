import React, { useState } from 'react';
import { useNavigate } from 'react-router';
import { Link } from 'react-router-dom';
import * as Papa from 'papaparse';

import { Event } from 'types';
import { CsvPreview } from 'modules/import-events';
import { FileUploadForm, For, Loader } from 'modules/common';
import { useGetImportTemplatesQuery, useImportEventsDryRunMutation } from 'services';

import 'App.css';

export const ImportEvents = () => {
    const { data, isLoading, isError, error } = useGetImportTemplatesQuery(undefined);
    const [importEventsDryRun, importEventsResult] = useImportEventsDryRunMutation(undefined);

    const navigate = useNavigate();

    const [csvData, setCsvData]             = useState<string[][]>([]);
    const [exampleEvents, setExampleEvents] = useState<Event[]>(importEventsResult.data ?? []);

    const [file, setFile] = useState<File | undefined>();

    const [selectedTemplate, setSelectedTemplate] = useState<string | undefined>(undefined);

    async function onCheckTemplateMapping() {
        if (!file) return;

        const csvData = Papa.parse(await file.text());
        if (csvData) {
            setCsvData(csvData.data.slice(0, 6) as string[][]);
        }

        if (selectedTemplate) importEventsDryRun({ file, templateName: selectedTemplate });
    }

    if (isLoading || importEventsResult.isLoading) return <Loader />
    if (isError) console.error(error); // TODO

    return (
        <div className="container horizontal">
            <Link to="/templates">Create Import Template</Link>
            <div className="container">
                <div className="item">
                    <FileUploadForm onSubmit={() => console.log('Uploading...')}
                                    onSelect={setFile}/>
                </div>
                <div className="item container horizontal">
                    <label htmlFor="selectTemplate">Templates</label>
                    <select id="selectTemplate"
                            value={selectedTemplate}
                            onChange={e => setSelectedTemplate(e.currentTarget.value)}>
                        <option key={undefined}></option>
                        <For each={data ?? []} onEach={t => (
                            <option key={t.id}>
                                {t.name}
                            </option>
                        )} />
                    </select>
                    <button className={`button blue item ${!!selectedTemplate ? '' : 'disabled'}`}
                            onClick={() => {
                                const templateId = data?.find(t => t.name === selectedTemplate)?.id;
                                if (!templateId) return;
                                navigate(`/templates/edit?template_id=${templateId}`)
                            }}>
                        Edit Template
                    </button>
                    <button className="button blue item"
                            onClick={onCheckTemplateMapping}>
                        Check template mapping
                    </button>
                </div>
            </div>
            {csvData.length > 0 && (
                <div className="item">
                    <h3>Csv data</h3>
                    <CsvPreview data={csvData.slice(0, 6)}/>
                </div>
            )}
            {exampleEvents.length > 0 && (
                <div className="item">
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
            </div>
            )}
        </div>
    )
}
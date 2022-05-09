import React, { useState } from 'react';
import { useNavigate } from 'react-router';
import { Link } from 'react-router-dom';

import { Event } from 'types';
import { CsvPreview } from 'modules/import-events';
import { FileUploadForm,  For, Loader, ScrollBar } from 'modules/common';
import { useGetImportTemplatesQuery, useImportEventsDryRunMutation } from 'services';

import 'App.css';

export const ImportEvents = () => {
    const { data, isLoading, isError, error } = useGetImportTemplatesQuery(undefined);
    const [importEventsDryRun, importEventsDryRunResult] = useImportEventsDryRunMutation(undefined);

    const navigate = useNavigate();

    const [file, setFile]                         = useState<File | undefined>();
    const [fulfilled, setFulfilled]               = useState(0);
    const [exampleEvents, setExampleEvents]       = useState<Event[]>([]);
    const [selectedTemplate, setSelectedTemplate] = useState<string | undefined>(undefined);

    async function onCheckTemplateMapping() {
        if (!file) return;

        setFile(file);
        if (selectedTemplate) importEventsDryRun({ file, templateName: selectedTemplate });
    }

    function onEditTemplate() {
        const templateId = data?.find(t => t.name === selectedTemplate)?.id;
        if (!templateId) return;
        navigate(`/templates/edit?template_id=${templateId}`)
    }

    {
        const { data, isSuccess, fulfilledTimeStamp } = importEventsDryRunResult;

        if (isSuccess && fulfilledTimeStamp > fulfilled) {
            setExampleEvents(data);
            setFulfilled(fulfilledTimeStamp);
        }
    }

    if (isLoading || importEventsDryRunResult.isLoading) return <Loader />
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
                        <For each={data ?? []} onEach={({ name, id }) => (
                            <option key={id}>
                                {name}
                            </option>
                        )} />
                    </select>
                    <button className={`button blue item ${!!selectedTemplate ? '' : 'disabled'}`}
                            onClick={onEditTemplate}>
                        Edit Template
                    </button>
                    <button className="button blue item"
                            onClick={onCheckTemplateMapping}>
                        Check template mapping
                    </button>
                </div>
            </div>
            {file && exampleEvents.length > 0 && (
                <div className="item">
                    <h3>Csv data</h3>
                    <CsvPreview csvFile={file}/>
                </div>
            )}
            {file && exampleEvents.length > 0 && (
                <div className="item">
                <h3>Mapped events</h3>
                <ScrollBar heightPx={500}>
                    <table>
                        <thead>
                        <tr>
                            <th>Occurred At</th>
                            <th>Text</th>
                        </tr>
                        </thead>
                        <tbody>
                        <For each={exampleEvents.slice(0, 100)} onEach={(e, i) => (
                            <tr key={i}>
                                <td>{e.occurredAtUtc}</td>
                                <td>{e.text}</td>
                            </tr>
                        )}/>
                        </tbody>
                    </table>
                </ScrollBar>
            </div>
            )}
        </div>
    )
}
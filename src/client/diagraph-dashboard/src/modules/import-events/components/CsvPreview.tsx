import React from 'react';
import { For } from 'modules/common';

import './CsvPreview.css';

export interface CsvPreviewProps {
    data: string[][];
}

export const CsvPreview: React.FC<CsvPreviewProps> = ({ data }) => {
    return (
        <div className="wrapper">
            <table className="csv-table">
                <thead>
                <tr>
                    <th className="block padding"/>
                    <For each={data[0]}
                         onEach={(h, i) => <th className="block" key={i}>{h}</th>}/>
                </tr>
                </thead>
                <tbody>
                <For each={data} onEach={(row, i) => (
                    <tr key={i}>
                        <td className="block padding">{i}</td>
                        <For each={row} onEach={(val, j)=> (
                            <td key={j}>{val}</td>
                        )} />
                    </tr>
                )} />
                </tbody>
            </table>
        </div>
    );
};
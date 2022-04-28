import React, { useState, useRef, DragEvent, ChangeEvent, FormEvent } from 'react';

import './FileUploadForm.css';

export interface FileUploadProps {
    onSubmit: (file: File) => void;
    onSelect?: (file: File) => void;
}

export const FileUploadForm: React.FC<FileUploadProps> = ({ onSubmit, onSelect }) => {
    // TODO: convert this to a form,
    // create a FileUpload page, and add pages to the
    // navigation bar - e.g. | Dashboard | | Upload |

    const fileUploadInput = useRef<HTMLInputElement>(null);

    const [selectedFile, setSelectedFile] = useState<File | null>(null);
    const [error, setError]               = useState('');

    const onSelectFile = (e: ChangeEvent<HTMLInputElement>) => {
        const file = e.currentTarget.files![0];
        if (!file) {
            setSelectedFile(null);
            return;
        }

        setSelectedFile(file);
        if (error) setError('');

        if (onSelect) onSelect(file);
    };

    const onClickSubmit = (e: FormEvent<HTMLButtonElement>) => {
        e.preventDefault();

        if (!selectedFile) {
            setError('Please select file before uploading.');
            return;
        }

        onSubmit(selectedFile);
    };

    const handleDrop = (e: DragEvent<HTMLDivElement>) => {
        e.preventDefault()
        e.stopPropagation()

        if (!e.dataTransfer.files[0]) return;

        setSelectedFile(e.dataTransfer.files[0]);
    }

    return (
        <div className="container horizontal box"
             onDragOver={e => e.preventDefault()}
             onDrop={handleDrop}>
            <form className="container horizontal box item"
                  onClick={() => fileUploadInput.current!.click()}>
                <label onClick={e => e.stopPropagation()}
                       htmlFor="fileUploadInput">
                        {!!selectedFile
                            ? selectedFile.name
                            : 'Drop file or click to upload'}
                </label>
                <input id="fileUploadInput"
                       type="file"
                       hidden
                       ref={fileUploadInput}
                       style={{display: "none"}}
                       onChange={onSelectFile}/>
            </form>
            <button className="button blue item"
                    style={{width: "40%", marginLeft: "30%"}}
                    type="submit"
                    onClick={onClickSubmit}>
                Upload
            </button>
            <span>{error}</span>
        </div>
    );
};
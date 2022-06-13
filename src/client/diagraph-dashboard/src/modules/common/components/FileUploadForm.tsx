import React, { useState, useRef, DragEvent, ChangeEvent, FormEvent } from 'react';

import { Box, BlueButton, Container, Input, Item } from 'styles';
import './FileUploadForm.css';

export interface FileUploadProps {
    onSubmit: (file: File) => void;
    onSelect?: (file: File) => void;
}

export const FileUploadForm: React.FC<FileUploadProps> = ({ onSubmit, onSelect }) => {
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
        if (error)    setError('');
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
        <Box style={{padding:"0.5em"}}>
            <Container vertical onDragOver={e => e.preventDefault()} onDrop={handleDrop}>
                <Box>
                    <form onClick={() => fileUploadInput.current!.click()}>
                        <Item style={{padding:"1em"}}>
                            <label htmlFor="fileUploadInput" onClick={e => e.stopPropagation()}>
                                {!!selectedFile
                                    ? selectedFile.name
                                    : 'Drop file or click to upload'}
                            </label>
                            <Input id="fileUploadInput"
                                   type="file"
                                   hidden
                                   ref={fileUploadInput}
                                   style={{display: "none"}}
                                   onChange={onSelectFile}/>
                        </Item>
                    </form>

                </Box>
                <Item as={Container} style={{margin:"0.5em"}}>
                    <BlueButton type="submit" onClick={onClickSubmit}>
                        Upload
                    </BlueButton>
                    <span>{error}</span>
                </Item>
            </Container>
        </Box>

    );
};
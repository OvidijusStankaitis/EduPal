import React, { useState, useEffect } from 'react';
import './UserPanel.css';

export const UserPanel = () => {
    const [files, setFiles] = useState([]);
    const [selectedFile, setSelectedFile] = useState(null);

    const handleFileChange = (event) => {
        const fileList = Array.from(event.target.files).map(file => {
            return {
                name: file.name,
                data: file,
                isSelected: false
            };
        });
        setFiles([...files, ...fileList]);
    };

    const handleFileClick = (file) => {
        setSelectedFile(file.data);

        if (file !== selectedFile) {
            files.forEach(f => {
                if (f.name === file.name) {
                    f.isSelected = true;
                } else {
                    f.isSelected = false;
                }
            });
            setFiles([...files]);
        }
    };

    return (
        <div className="user-panel">
            <h1>User File Upload</h1>

            <div className="main-content">
                {/* Upload & Files list */}
                <div className="file-section">
                    <button onClick={() => document.getElementById('fileInput').click()}>Upload</button>
                    <input type="file" id="fileInput" accept=".pdf" style={{display: 'none'}} onChange={handleFileChange} multiple />

                    <ul className="files-list">
                        {files.length > 0 ? (
                            files.map((file, index) => (
                                <li
                                    key={index}
                                    onClick={() => handleFileClick(file)}
                                    className={file.isSelected ? 'selected' : ''}
                                >
                                    {file.name}
                                </li>
                            ))
                        ) : (
                            <li className="empty">Empty list</li>
                        )}
                    </ul>
                </div>
                
                {/* PDF Viewer */}
                <div className="pdf-viewer">
                    {selectedFile && <iframe src={URL.createObjectURL(selectedFile)} type="application/pdf" />}
                </div>
            </div>
        </div>
    );
};

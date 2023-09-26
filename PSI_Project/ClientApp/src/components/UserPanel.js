import React, { useState, useEffect } from 'react';
import './UserPanel.css';

export const UserPanel = () => {
    const [files, setFiles] = useState([]);
    const [selectedFile, setSelectedFile] = useState(null);

    useEffect(() => {
        fetch('https://localhost:7283/Conspectus/list')
            .then(response => response.json())
            .then(data => {
                const fileList = data.map(fileObj => {
                    const fullPath = fileObj.path;
                    const fileName = fullPath.split('\\').pop();
                    return {
                        name: fileName,
                        isSelected: false
                    };
                });
                setFiles(fileList);
            })
            .catch(error => console.error('Error fetching files:', error));
    }, []);

    const handleFileChange = (event) => {
        const fileList = Array.from(event.target.files).map(file => {
            return {
                name: file.name,
                data: file,
                isSelected: false
            };
        });
        
        const formData = new FormData();
        fileList.forEach(file => formData.append('files', file.data));

        fetch('https://localhost:7283/Conspectus/upload', {
            method: 'POST',
            body: formData
        })
            .then(response => response.json())
            .then(data => {
                const updatedFiles = data.map(fileObj => {
                    const fullPath = fileObj.path;
                    const fileName = fullPath.split('\\').pop();
                    return {
                        name: fileName,
                        isSelected: false
                    };
                });
                setFiles(updatedFiles);
            })
            .catch(error => console.error('Error uploading files:', error));
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
                                <li key={index}>
                                    <span className="file-name">
                                        {file.name} {/* Ensure you're accessing the name property */}
                                    </span>
                                    <button className="small-button download-button">Download</button>
                                    <button className="small-button delete-button">Delete</button>
                                </li>
                            ))
                        ) : (
                            <li className="empty">Empty list</li>
                        )}
                    </ul>

                </div>

                {/* PDF Viewer */}
                <div className="pdf-viewer">
                </div>
            </div>
        </div>
    );
};

import React, { useState, useEffect, useRef } from 'react';
import './Conspectus.css';
import {useParams} from "react-router-dom";

export const Conspectus = () => {
    const { topicId } = useParams();
    const [topicName, setTopicName] = useState("");
    const [files, setFiles] = useState([]);
    const iframeRef = useRef(null);

    useEffect(() => {
        fetch(`https://localhost:7283/Topic/get/${topicId}`)
            .then(response => response.json())
            .then(data => setTopicName(data.name))
            .catch(error => console.error('Error getting topic name:', error))
        
        fetch(`https://localhost:7283/Conspectus/list/${topicId}`)
            .then(response => response.json())
            .then(data => {
                const fileList = data.map(fileObj => {
                    let fileName = fileObj.path.split('\\').pop();
                    fileName = fileName.length > 10 ? fileName.substring(0, 10) + "..." : fileName;
                    return {
                        id: fileObj.id,
                        name: fileName,
                        isSelected: false
                    };
                });
                setFiles(fileList);
            })
            .catch(error => console.error('Error fetching files:', error));
    }, [topicId]);

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

        fetch(`https://localhost:7283/Conspectus/upload/${topicId}`, {
            method: 'POST',
            body: formData
        })
            .then(response => response.json())
            .then(data => {
                const updatedFiles = data.map(fileObj => {
                    const fullPath = fileObj.path;
                    const fileName = fullPath.split('\\').pop();
                    return {
                        id: fileObj.id,
                        name: fileName,
                        isSelected: false
                    };
                });
                setFiles(updatedFiles);
            })
            .catch(error => console.error('Error uploading files:', error));
    };

    const handleFileClick = (fileId) => {
        fetch(`https://localhost:7283/Conspectus/get/${fileId}`)
            .then(response => response.blob())
            .then(blob => {
                const url = window.URL.createObjectURL(blob);
                if (iframeRef.current) {
                    iframeRef.current.src = url;
                }
            })
            .catch(error => {
                console.error('Error fetching PDF:', error);
            });
    };

    const handleFileDownload = (fileId) => {
        const fileToDownload = files.find(file => file.id === fileId);
        const downloadLink = document.createElement('a');
        downloadLink.href = `https://localhost:7283/Conspectus/download/${fileId}`;
        downloadLink.download = fileToDownload ? fileToDownload.name : ''; // Set the desired filename
        document.body.appendChild(downloadLink);
        downloadLink.click();
        document.body.removeChild(downloadLink);
    };

    const handleFileDelete = (fileId) => {
        fetch(`https://localhost:7283/Conspectus/${fileId}/delete`, {
            method: 'DELETE'
        })
            .then(response => {
                if (response.ok) {
                    const updatedFiles = files.filter(file => file.id !== fileId);
                    setFiles(updatedFiles);
                    window.location.reload();
                } else {
                    console.error('Error deleting file:', response.statusText);
                }
            })
            .catch(error => console.error('Error deleting file:', error));
    };

    return (
        <div className="user-panel">
            <h1>{topicName}</h1>
            <div className="main-content">
                <div className="file-section">
                    <button onClick={() => document.getElementById('fileInput').click()}>Upload</button>
                    <input type="file" id="fileInput" accept=".pdf" style={{display: 'none'}} onChange={handleFileChange} multiple />
                    <ul className="files-list">
                        {files.length > 0 ? (
                            files.map((file, index) => (
                                <li key={index}>
                                    <button className="small-button file-name" onClickCapture={() => handleFileClick(file.id)}>
                                        {file.name}
                                    </button>
                                    <button className="small-button download-button" onClick={() => handleFileDownload(file.id)}>
                                        Download
                                    </button>
                                    <button className="small-button delete-button" onClick={() => handleFileDelete(file.id)}>
                                        Delete
                                    </button>
                                </li>
                            ))
                        ) : (
                            <li className="empty">Empty list</li>
                        )}
                    </ul>
                </div>
                <div className="pdf-viewer">
                    <iframe ref={iframeRef} type="application/pdf"/>
                </div>
            </div>
        </div>
    );
};

import React, { useState, useEffect, useRef} from 'react';
import './Conspectus.css';
import { useParams } from "react-router-dom";
import { UserComponent } from "./UserComponent";
import { PomodoroDialog } from './PomodoroDialog';
import { OpenAIDialogue } from './OpenAIDialogue';
import { Comments } from "./Comments";
import { Note } from "./Note";
import Notes from "../assets/Notes.webp";
export const Conspectus = () => {
    const { topicId } = useParams();
    const [topicName, setTopicName] = useState("");
    const [truncatedTopicName, setTruncatedTopicName] = useState("");
    const [files, setFiles] = useState([]);
    const iframeRef = useRef(null);
    const [showPomodoroDialog, setShowPomodoroDialog] = useState(false);
    const [showOpenAIDialog, setShowOpenAIDialog] = useState(false);
    const [showComments, setShowComments] = useState(false);
    const [showNote, setShowNote] = useState(false);

    useEffect(() => {
        fetch(`https://localhost:7283/Topic/get/${topicId}`)
            .then(response => response.json())
            .then(data => {
                setTopicName(data.name);
                setTruncatedTopicName(data.name.length > 11 ? data.name.substring(0, 11) + "..." : data.name);
            })
            .catch(error => console.error('Error getting topic name:', error));
    }, []);

    useEffect(() => {        
        fetch(`https://localhost:7283/Conspectus/list/${topicId}`)
            .then(response => response.json())
            .then(data => {
                const fileList = data.map(fileObj => {
                    let fileName = fileObj.path.split('\\').pop();
                    return {
                        id: fileObj.id,
                        name: fileName,
                        truncatedName: fileName.length > 11 ? fileName.substring(0, 11) + "..." : fileName,
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
                        truncatedName: fileName.length > 11 ? fileName.substring(0, 11) + "..." : fileName, // add truncatedName here
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
            <div className="header">
                <h1 title={topicName} className="truncated-text">{truncatedTopicName}</h1>
                <img className="notes" src={Notes} alt="notes" onClick={() => setShowNote(true)}/>
                <UserComponent 
                    setShowPomodoroDialog={setShowPomodoroDialog}
                    setShowOpenAIDialog={setShowOpenAIDialog}
                />
            </div>
            <div className="main-content">
                <div className="file-section">
                    <div className="button-group">
                        <button onClick={() => document.getElementById('fileInput').click()}>Upload</button>
                        <button onClick={() => setShowComments(true)}>Comments</button>
                    </div>
                    <input type="file" id="fileInput" accept=".pdf" style={{display: 'none'}} onChange={handleFileChange} multiple />
                    <ul className="files-list">
                        {files.length > 0 ? (
                            files.map((file, index) => (
                                <li key={index}>
                                    <button
                                        className="small-button file-name"
                                        onClickCapture={() => handleFileClick(file.id)}
                                        title={file.name}
                                    >
                                        {file.truncatedName}
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
                <PomodoroDialog
                    show={showPomodoroDialog}
                    onClose={() => setShowPomodoroDialog(false)}
                />
                <OpenAIDialogue 
                    show={showOpenAIDialog} 
                    onClose={() => setShowOpenAIDialog(false)}
                />
                <Comments
                    show={showComments}
                    onClose={() => setShowComments(false)}
                    topicId={topicId}
                />
                <Note
                    show={showNote}
                    onClose={() => setShowNote(false)}
                    topicId={topicId}
                />
            </div>
        </div>
    );
};

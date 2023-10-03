import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import './Subjects.css';

export const Subjects = () => {
    const [subjects, setSubjects] = useState([]);
    const [showDialog, setShowDialog] = useState(false);
    const [refreshSubjects, setRefreshSubjects] = useState(false);
    const [newSubjectName, setNewSubjectName] = useState('');

    useEffect(() => {
        const fetchSubjects = async () => {
            const response = await fetch('https://localhost:7283/Subject/list');
            const data = await response.json();
            console.log("Fetched subjects:", data);
            setSubjects(data.map(subject => subject.name));
        };

        fetchSubjects();
    }, [refreshSubjects]);

    const handleAddSubject = async () => {
        if (newSubjectName) {
            const requestBody = {
                subjectName: newSubjectName,
                subjectDescription: ""  // default empty description
            };
            console.log(requestBody);
            const response = await fetch('https://localhost:7283/Subject/upload', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(requestBody)
            });

            if (response.ok) {
                const data = await response.json();
                setSubjects(data.map(subject => subject.name));
                setNewSubjectName('');
                setShowDialog(false);
                setRefreshSubjects(prev => !prev);  // Toggle the state to trigger re-fetching
            } else {
                console.error("Error uploading subject:", await response.text());
            }
        }
    };

    return (
        <div className="subjects-page-container">
            <div className="subjects-container">
                <h1>Subjects</h1>
                <div className="subjects-grid">
                    {subjects.map((subject, index) => (
                        <Link to={`/Subjects/${subject}-Topics`} key={index} className="subject-grid-item">
                            <h2>{subject}</h2>
                        </Link>
                    ))}
                    <div className="subject-grid-item add-subject" onClick={() => setShowDialog(true)}>
                        <span className="plus-icon">+</span>
                    </div>
                </div>
                {showDialog && (
                    <div className="dialog-s">
                        <input
                            type="text"
                            placeholder="Subject Name"
                            value={newSubjectName}
                            onChange={(e) => setNewSubjectName(e.target.value)}
                        />
                        <button onClick={handleAddSubject}>Create</button>
                        <button onClick={() => setShowDialog(false)}>Cancel</button>
                    </div>
                )}
            </div>
        </div>
    );
};

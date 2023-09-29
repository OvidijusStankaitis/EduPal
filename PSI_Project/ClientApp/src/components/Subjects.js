import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import './Subjects.css';

export const Subjects = () => {
    const [subjects, setSubjects] = useState([]);
    const [showDialog, setShowDialog] = useState(false);
    const [newSubjectName, setNewSubjectName] = useState('');

    const handleAddSubject = () => {
        if (newSubjectName) {
            setSubjects([newSubjectName, ...subjects]);
            setNewSubjectName('');
            setShowDialog(false);
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

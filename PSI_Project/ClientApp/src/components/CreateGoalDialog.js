import React, { useState, useEffect } from 'react';
import './CreateGoalDialog.css';

export const CreateGoalDialog = ({ show, onClose }) => {
    const [subjects, setSubjects] = useState([]);
    const [showDropdown, setShowDropdown] = useState(false);
    const [checkedSubjects, setCheckedSubjects] = useState({});
    const [goalTime, setGoalTime] = useState('');

    // Simulate fetching subjects from your database
    useEffect(() => {
        // This should be replaced with your actual fetching logic
        const fetchSubjects = async () => {
            const fetchedSubjects = [
                { id: '1', name: 'Math' },
                { id: '2', name: 'Biology' },
                { id: '3', name: 'Biolog' },
                { id: '4', name: 'Biolo' },
                { id: '5', name: 'Biol' },
                // ...other subjects
            ];
            setSubjects(fetchedSubjects);
        };
        fetchSubjects();
    }, []);

    const handleCheck = (subjectId) => {
        setCheckedSubjects((prevCheckedSubjects) => ({
            ...prevCheckedSubjects,
            [subjectId]: !prevCheckedSubjects[subjectId],
        }));
    };

    const handleSubmit = () => {
        const selectedSubjects = Object.entries(checkedSubjects)
            .filter(([_, checked]) => checked)
            .map(([id, _]) => subjects.find((subject) => subject.id === id));

        console.log("Selected subjects:", selectedSubjects);
        console.log("Goal time:", goalTime);
        // Here you would typically send the data to your backend
        onClose();
    };

    if (!show) return null;

    return (
        <div className="create-goal-dialog">
            <div className="create-goal-dialog-content">
                <h3>Create New Goal</h3>
                <div className="dropdown">
                    <button onClick={() => setShowDropdown(!showDropdown)}>
                        Select subjects
                    </button>
                    {showDropdown && (
                        <div className="dropdown-menu">
                            {subjects.map((subject) => (
                                <div key={subject.id} className="dropdown-item">
                                    <input className="subject-list-input"
                                        id={`subject-${subject.id}`}
                                        type="checkbox"
                                        checked={!!checkedSubjects[subject.id]}
                                        onChange={() => handleCheck(subject.id)}
                                    />
                                    <label htmlFor={`subject-${subject.id}`}>{subject.name}</label>
                                </div>
                            ))}
                        </div>
                    )}
                </div>
                <input className="input-hours"
                    type="text"
                    placeholder="Goal Time (e.g., 2 hours)"
                    value={goalTime}
                    onChange={(e) => setGoalTime(e.target.value)}
                />
                <button onClick={handleSubmit}>Create Goal</button>
                <button onClick={onClose}>Cancel</button>
            </div>
        </div>
    );
};

import React, { useState, useEffect } from 'react';
import './CreateGoalDialog.css';
import { useUserContext } from "../UserContext"; // Assuming the UserContext is in the right path

export const CreateGoalDialog = ({ show, onClose }) => {
    const { userId } = useUserContext();
    const [subjects, setSubjects] = useState([]);
    const [showDropdown, setShowDropdown] = useState(false);
    const [checkedSubjects, setCheckedSubjects] = useState({});
    const [goalTime, setGoalTime] = useState('');

    useEffect(() => {
        const fetchSubjects = async () => {
            try {
                const response = await fetch('https://localhost:7283/goals/subjects');
                if (!response.ok) {
                    throw new Error('Failed to fetch subjects');
                }
                const subjects = await response.json();
                setSubjects(subjects);
            } catch (error) {
                console.error('Error fetching subjects:', error);
            }
        };

        fetchSubjects();
    }, []);

    const handleCheck = (subjectId) => {
        setCheckedSubjects((prevCheckedSubjects) => ({
            ...prevCheckedSubjects,
            [subjectId]: !prevCheckedSubjects[subjectId],
        }));
    };

    const handleSubmit = async () => {
        const selectedSubjectIds = Object.entries(checkedSubjects)
            .filter(([_, checked]) => checked)
            .map(([id, _]) => id);

        // Replace commas with dots and then parse as float
        const formattedGoalTime = goalTime.replace(',', '.');

        const goalData = {
            userId: userId,
            subjectIds: selectedSubjectIds,
            goalTime: parseFloat(formattedGoalTime)
        };

        try {
            const response = await fetch('https://localhost:7283/Goals/create', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(goalData),
            });

            if (!response.ok) {
                throw new Error('Failed to create goal');
            }

            onClose();
            setCheckedSubjects({});
            setGoalTime('');
        } catch (error) {
            console.error('Error creating goal:', error);
        }
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
                    placeholder="Target Time (e.g. 2.2/2,2 hours)"
                    value={goalTime}
                    onChange={(e) => setGoalTime(e.target.value)}
                />
                <button onClick={handleSubmit}>Create Goal</button>
                <button onClick={onClose}>Cancel</button>
            </div>
        </div>
    );
};

import React, { useState, useEffect } from 'react';
import './CreateGoalDialog.css';
import { useUserContext } from "../UserContext"; // Assuming the UserContext is in the right path

export const CreateGoalDialog = ({ show, onClose }) => {
    const userId = localStorage.getItem('userId');
    const [subjects, setSubjects] = useState([]);
    const [showDropdown, setShowDropdown] = useState(false);
    const [checkedSubjects, setCheckedSubjects] = useState({});
    const [goalTime, setGoalTime] = useState('');
    const [error, setError] = useState('');

    useEffect(() => {
        const fetchSubjects = async () => {
            try {
                const response = await fetch('https://localhost:7283/Goals/subjects');
                if (!response.ok) {
                    throw new Error('Failed to fetch subjects');
                }
                const subjects = await response.json();
                setSubjects(subjects);
            } catch (error) {
                console.error('Error fetching subjects:', error);
            }
        };

        if (show) {
            fetchSubjects();
        }
    }, [show]);

    const handleCheck = (subjectId) => {
        setCheckedSubjects((prevCheckedSubjects) => ({
            ...prevCheckedSubjects,
            [subjectId]: !prevCheckedSubjects[subjectId],
        }));
    };

    const validateGoalTime = (time) => {
        const number = parseFloat(time.replace(',', '.'));
        if (isNaN(number) || number < 0 || number > 24) {
            return { valid: false, message: 'Please enter a value between 0 and 24 hours.' };
        }
        return { valid: true, message: '' };
    };

    const handleSubmit = async () => {
        setError('');

        const selectedSubjectIds = Object.entries(checkedSubjects)
            .filter(([_, checked]) => checked)
            .map(([id, _]) => id);
        if (selectedSubjectIds.length === 0) {
            setError('Please select at least one subject.');
            return;
        }

        const { valid, message } = validateGoalTime(goalTime);
        if (!valid) {
            setError(message);
            return;
        }

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
                const errorData = await response.json();
                setError(errorData.message || 'Failed to create goal. Please try again.');
                return;
            }
            onClose();
            setCheckedSubjects({});
            setGoalTime('');
        } catch (error) {
            console.error('Error creating goal:', error);
            setError('Failed to create goal. Please try again.');
        }
        window.location.reload();
    };

    if (!show) return null;

    return (
        <div className="create-goal-dialog">
            <div className="create-goal-dialog-content">
                <h3>Create New Goal</h3>
                <div className="dropdown">
                    <button className="select-subjects-dropdown-button" onClick={() => setShowDropdown(!showDropdown)}>
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
                {error && <div className="error-message">{error}</div>}
                <button className="create-goal-button" onClick={handleSubmit}>Create Goal</button>
                <button onClick={onClose}>Cancel</button>
            </div>
        </div>
    );
};
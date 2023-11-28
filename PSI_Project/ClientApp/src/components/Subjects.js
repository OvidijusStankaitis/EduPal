import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import './Subjects.css';
import { UserComponent } from "./UserComponent";
import { PomodoroDialog } from './PomodoroDialog';
import {OpenAIDialogue} from "./OpenAIDialogue";
import { CreateGoalDialog } from './CreateGoalDialog';

export const Subjects = () => {
    const [subjects, setSubjects] = useState([]);
    const [subjectsDisplayNames, setSubjectsDisplayNames] = useState([]);
    const [showDialog, setShowDialog] = useState(false);
    const [refreshSubjects, setRefreshSubjects] = useState(false);
    const [newSubjectName, setNewSubjectName] = useState('');
    const [showPomodoroDialog, setShowPomodoroDialog] = useState(false);
    const [showOpenAIDialog, setShowOpenAIDialog] = useState(false);
    const [showCreateGoalDialog, setShowCreateGoalDialog] = useState(false);
    const navigate = useNavigate();

    useEffect(() => {
        const fetchSubjects = async () => {
            const response = await fetch('https://localhost:7283/Subject/list');

            if(response.ok) {
                const data = await response.json();
                console.log("Fetched subjects:", data);

                const subjectsData = data.map(subject => {
                    return {
                        id: subject.id,
                        name: subject.name
                    };
                });

                setSubjects(subjectsData);

                const displayNames = subjectsData.map(subject => {
                    return subject.name.length > 9
                        ? subject.name.substring(0, 9) + "..."
                        : subject.name;
                });

                setSubjectsDisplayNames(displayNames);
            }
            else {
                console.error("Error fetching subjects: ", await response.text());
            }
        };

        fetchSubjects();
    }, [refreshSubjects]);

    const handleAddSubject = async () => {
        if (newSubjectName) {
            const requestBody = {
                subjectName: newSubjectName
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
                
                subjects.push({
                    id: data.id,
                    name: data.name
                });
                setSubjects(subjects);
                
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
                <div className="headersub">
                    <h1>Subjects</h1>
                    <UserComponent 
                        setShowPomodoroDialog={setShowPomodoroDialog}
                        setShowOpenAIDialog={setShowOpenAIDialog}
                        setShowCreateGoalDialog={setShowCreateGoalDialog}
                    />
                </div>
                <div className="subjects-grid">
                    {subjects.map((subject, index) => (
                        <div
                            key={index}
                            className="subject-grid-item"
                            title={subject.name}
                            onClick={() => navigate(`/Subjects/${subject.id}-Topics`)}
                        >
                            <h2>{subjectsDisplayNames[index]}</h2>
                        </div>
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
                <PomodoroDialog
                    show={showPomodoroDialog}
                    onClose={() => setShowPomodoroDialog(false)}
                />
                <OpenAIDialogue
                    show={showOpenAIDialog}
                    onClose={() => setShowOpenAIDialog(false)}
                />
                <CreateGoalDialog // Render the CreateGoalDialog
                    show={showCreateGoalDialog}
                    onClose={() => setShowCreateGoalDialog(false)}
                />
            </div>
        </div>
    );
};

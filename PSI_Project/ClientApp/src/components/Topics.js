import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import './Topics.css';
import {UserComponent} from "./UserComponent";
import { PomodoroDialog } from './PomodoroDialog';
import {OpenAIDialogue} from "./OpenAIDialogue";

export const Topics = () => {
    const { subjectId } = useParams();
    const [subjectName, setSubjectName] = useState("");
    const [topics, setTopics] = useState([]);
    const [topicsDisplayNames, setTopicsDisplayNames] = useState([]);
    const [showDialog, setShowDialog] = useState(false);
    const [refreshTopics, setRefreshTopics] = useState(false);
    const [newTopicName, setNewTopicName] = useState('');
    const [showPomodoroDialog, setShowPomodoroDialog] = useState(false)
    const [showOpenAIDialog, setShowOpenAIDialog] = useState(false);
    const navigate = useNavigate();

    useEffect(() => {
        fetch(`https://localhost:7283/Subject/get/${subjectId}`)
            .then(response => response.json())
            .then(data => setSubjectName(data.name))
            .catch(error => console.error('Error getting subject name:', error))
    }, []);

    useEffect(() => {
        const fetchTopics = async () => {
            const response = await fetch(`https://localhost:7283/Topic/list/${subjectId}`);

            if(response.ok) {
                const data = await response.json();
                console.log("Fetched topics:", data);

                const topicsData = data.map(topic => {
                    return {
                        id: topic.id,
                        name: topic.name
                    };
                });

                setTopics(topicsData);

                const displayNames = topicsData.map(topic => {
                    return topic.name.length > 9
                        ? topic.name.substring(0, 9) + "..."
                        : topic.name;
                });

                setTopicsDisplayNames(displayNames);
            }
            else {
                console.error("Error fetching topics: ", await response.text());
            }
        };

        fetchTopics();
    }, [refreshTopics, subjectId]);

    const handleAddTopic = async () => {
        if (newTopicName) {
            const requestBody = {
                topicName: newTopicName,
                subjectId: subjectId
            };
            console.log(requestBody);
            const response = await fetch('https://localhost:7283/Topic/upload', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(requestBody)
            });

            if (response.ok) {
                const data = await response.json();
                
                topics.push({
                    id: data.id,
                    name: data.name
                });
                setTopics(topics);
                
                setNewTopicName('');
                setShowDialog(false);
                setRefreshTopics(prev => !prev);  // Toggle the state to trigger re-fetching
            } else {
                console.error("Error uploading topic:", await response.text());
            }
        }
    };

    const handleTopicClick = (topicId) => {
        navigate(`/Subjects/${subjectId}-Topics/${topicId}-Conspectus`);
    };

    return (
        <div className="topics-page-container">
            <div className="topics-container">
                <div className="headert">
                    <h1>{subjectName}</h1>
                    <UserComponent
                        setShowPomodoroDialog={setShowPomodoroDialog}
                        setShowOpenAIDialog={setShowOpenAIDialog}
                    />
                </div>
                <div className="topics-grid">
                    {topics.map((topic, index) => (
                        <div
                            key={index}
                            className="topic-grid-item"
                            onClick={() => handleTopicClick(topic.id)}
                            title={topic.name} // Tooltip with full topic name
                        >
                            <h2>{topicsDisplayNames[index]}</h2>
                        </div>
                    ))}
                    <div className="topic-grid-item add-topic" onClick={() => setShowDialog(true)}>
                        <span className="plus-icon">+</span>
                    </div>
                </div>
                {showDialog && (
                    <div className="dialog-t">
                        <input
                            type="text"
                            placeholder="Topic Name"
                            value={newTopicName}
                            onChange={(e) => setNewTopicName(e.target.value)}
                        />
                        <button onClick={handleAddTopic}>Create</button>
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
            </div>
        </div>
    );
};

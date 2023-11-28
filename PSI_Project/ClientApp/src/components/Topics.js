import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import './Topics.css';
import {UserComponent} from "./UserComponent";
import { PomodoroDialog } from './PomodoroDialog';
import {OpenAIDialogue} from "./OpenAIDialogue";
import { CreateGoalDialog } from './CreateGoalDialog';

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
    const [showCreateGoalDialog, setShowCreateGoalDialog] = useState(false);
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

            if (response.ok) {
                const data = await response.json();
                console.log("Fetched topics:", data);

                const topicsData = data.map(topic => {
                    return {
                        id: topic.id,
                        name: topic.name,
                        knowledgeRating: topic.knowledgeRating
                    };
                });

                setTopics(topicsData);

                const displayNames = topicsData.map(topic => {
                    return topic.name.length > 9
                        ? topic.name.substring(0, 9) + "..."
                        : topic.name;
                });

                setTopicsDisplayNames(displayNames);
            } else {
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

    const handleKnowledgeLevelChange = async (topicId, knowledgeLevel) => {
        // Send a request to update the knowledge level on the server
        const requestBody = {
            topicId,
            knowledgeLevel
        };

        try {
            const response = await fetch('https://localhost:7283/Topic/update-knowledge-level', {
                method: 'PUT', // Use the appropriate HTTP method for updates
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(requestBody)
            });

            if (response.ok) {
                // Update the component state with the new knowledge level
                const updatedTopics = topics.map(topic => {
                    if (topic.id === topicId) {
                        return { ...topic, knowledgeRating: knowledgeLevel };
                    }
                    return topic;
                });

                setTopics(updatedTopics);
            } else {
                console.error("Error updating knowledge level:", await response.text());
            }
        } catch (error) {
            console.error("Error updating knowledge level:", error);
        }
    };

    return (
        <div className="topics-page-container">
            <div className="topics-container">
                <div className="headert">
                    <h1>{subjectName}</h1>
                    <UserComponent
                        setShowPomodoroDialog={setShowPomodoroDialog}
                        setShowOpenAIDialog={setShowOpenAIDialog}
                        setShowCreateGoalDialog={setShowCreateGoalDialog}
                    />
                </div>
                <div className="topics-grid">
                    {topics.map((topic, index) => (
                        <div className="topic-grid-item" key={index} onClick={() => handleTopicClick(topic.id)}>
                            <div className="topic-content" title={topic.name}>
                                <h2>{topicsDisplayNames[index]}</h2>
                                <div className="dropdown-container" onClick={(e) => e.stopPropagation()}>
                                    <select
                                        value={topic.knowledgeRating}
                                        onChange={(e) => handleKnowledgeLevelChange(topic.id, e.target.value)}
                                    >
                                        <option value="2">Poor</option>
                                        <option value="1">Average</option>
                                        <option value="0">Good</option>
                                    </select>
                                </div>
                            </div>
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
                <CreateGoalDialog // Render the CreateGoalDialog
                    show={showCreateGoalDialog}
                    onClose={() => setShowCreateGoalDialog(false)}
                />
            </div>
        </div>
    );
};

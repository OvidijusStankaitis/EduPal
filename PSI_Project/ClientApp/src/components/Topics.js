import React, { useState, useEffect } from 'react';
import { Link, useParams } from 'react-router-dom';
import './Topics.css';
import {UserComponent} from "./UserComponent";
import { PomodoroDialog } from './PomodoroDialog';

export const Topics = () => {
    const { subjectId } = useParams();
    const [subjectName, setSubjectName] = useState("");
    const [topics, setTopics] = useState([]);
    const [showDialog, setShowDialog] = useState(false);
    const [refreshTopics, setRefreshTopics] = useState(false);
    const [newTopicName, setNewTopicName] = useState('');
    const [showPomodoroDialog, setShowPomodoroDialog] = useState(false);

    
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
                
                setTopics(data.map(topic => {
                    return {
                        id: topic.id,
                        name: topic.name
                    };
                }));
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

    return (
        <div className="topics-page-container">
            <div className="topics-container">
                <div className="headert">
                    <h1>{subjectName}</h1>
                    <UserComponent setShowPomodoroDialog={setShowPomodoroDialog} />
                </div>
                <div className="topics-grid">
                    {topics.map((topic, index) => (
                        <Link to={`/Subjects/${subjectId}-Topics/${topic.id}-Conspectus`} key={index} className="topic-grid-item">
                            <h2>{topic.name}</h2>
                        </Link>
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
            </div>
        </div>
    );
};

import React, { useState, useEffect } from 'react';
import { Link, useParams } from 'react-router-dom';
import './Topics.css';

export const Topics = () => {
    const { subjectName } = useParams();
    const [topics, setTopics] = useState([]);
    const [showDialog, setShowDialog] = useState(false);
    const [refreshTopics, setRefreshTopics] = useState(false);
    const [newTopicName, setNewTopicName] = useState('');

    useEffect(() => {
        const fetchTopics = async () => {
            const response = await fetch(`https://localhost:7283/Topic/list/${subjectName}`);
            const data = await response.json();
            console.log("Fetched topics:", data);
            setTopics(data.map(topic => topic.name));
        };

        fetchTopics();
    }, [refreshTopics, subjectName]);

    const handleAddTopic = async () => {
        if (newTopicName) {
            const requestBody = {
                topicName: newTopicName,
                topicDescription: "",  // default empty description
                subjectName: subjectName
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
                setTopics(data.map(topic => topic.name));
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
                <h1>{subjectName}</h1>
                <div className="topics-grid">
                    {topics.map((topic, index) => (
                        <Link to={`/Subjects/${subjectName}-Topics/${topic}-Conspectus`} key={index} className="topic-grid-item">
                            <h2>{topic}</h2>
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
            </div>
        </div>
    );
};

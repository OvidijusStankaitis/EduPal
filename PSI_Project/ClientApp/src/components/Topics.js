import React, { useState } from 'react';
import {Link, useParams} from "react-router-dom";
import './Topics.css';

export const Topics = () => {
    const { subjectName } = useParams();
    const [topics, setTopics] = useState([]);
    const [showDialog, setShowDialog] = useState(false);
    const [newTopicName, setNewTopicName] = useState('');

    const handleAddTopic = () => {
        if (newTopicName) {
            setTopics([newTopicName, ...topics]);
            setNewTopicName('');
            setShowDialog(false);
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

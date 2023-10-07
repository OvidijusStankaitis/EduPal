import React, { useState, useEffect } from 'react';
import { Link, useParams } from 'react-router-dom';
import './Topics.css';

export const Topics = () => {
    const { subjectId } = useParams();
    const [subjectName, setSubjectName] = useState("");
    const [topicIds, setTopicIds] = useState([]);
    const [topicNames, setTopicNames] = useState([]);
    const [showDialog, setShowDialog] = useState(false);
    const [refreshTopics, setRefreshTopics] = useState(false);
    const [newTopicName, setNewTopicName] = useState('');
    
    useEffect(() => {
        fetch(`https://localhost:7283/Subject/get/${subjectId}`)
            .then(response => response.json())
            .then(data => setSubjectName(data.name))
            .catch(error => console.error('Error getting subject name:', error))        
        
        const fetchTopics = async () => {
            const response = await fetch(`https://localhost:7283/Topic/list/${subjectId}`);
            const data = await response.json();
            console.log("Fetched topics:", data);            
            setTopicIds(data.map(topic => topic.id));
            setTopicNames(data.map(topic => topic.name));
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

                var updatedIdsArr = topicIds;
                updatedIdsArr.push(data.id);
                setTopicIds(updatedIdsArr);

                var updatedNamesArr = topicNames;
                updatedNamesArr.push(data.name);
                setTopicNames(updatedNamesArr);
                
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
                    {topicNames.map((topic, index) => (
                        <Link to={`/Subjects/${subjectId}-Topics/${topicIds.at(index)}-Conspectus`} key={index} className="topic-grid-item">
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

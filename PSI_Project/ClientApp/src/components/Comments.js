import React, { useState, useEffect, useRef } from 'react';
import './Comments.css';

export const Comments = ({ show, onClose, topicId }) => {
    const [comments, setComments] = useState([]);
    const [currentComment, setCurrentComment] = useState('');
    const chatContentRef = React.createRef();

    useEffect(() => {
        if (show) {
            fetch(`https://localhost:7283/Comment/get/${topicId}`)
                .then(response => response.json())
                .then(data => setComments(data))
                .catch(error => console.error("Error fetching comments:", error));
        }
        if (chatContentRef.current) {
            chatContentRef.current.scrollTop = chatContentRef.current.scrollHeight;
        }
    }, [show, topicId]);

    const handleSend = () => {
        if (currentComment.trim() !== '') {
            const newComment = {
                topicId: topicId,
                commentText: currentComment
            };

            fetch(`https://localhost:7283/Comment/upload`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(newComment)
            })
                .then(response => response.json())
                .then(data => {
                    setComments(prevComments => [...prevComments, data]);
                    setCurrentComment('');
                })
                .catch(error => console.error("Error posting comment:", error));
        }
        if (chatContentRef.current) {
            chatContentRef.current.scrollTop = chatContentRef.current.scrollHeight;
        }
    };

    const handleDelete = (commentId) => {
        fetch(`https://localhost:7283/Comment/delete/${commentId}`, {
            method: 'DELETE',
        })
            .then(response => {
                if(response.ok) {
                    setComments(prevComments => prevComments.filter(comment => comment.id !== commentId));
                } else {
                    console.error("Error deleting comment:", response.statusText);
                }
            })
            .catch(error => console.error("Error deleting comment:", error));
    };

    if (!show) return null;

    return (
        <div className="comments">
            <div className="comments-content">
                <div className="greeting-comments">
                    <h3>Comments</h3>
                </div>
                <div className="comments-chat">
                    <div className="comments-chat-content" ref={chatContentRef}>
                        {comments.map((comment, index) => (
                            <div key={index} className="comment">
                                <div className="comment-text-content">
                                    {comment.commentText}
                                </div>
                                <button
                                    className="delete-button1"
                                    onClick={() => handleDelete(comment.id)}
                                >
                                    🗑️
                                </button>
                            </div>
                        ))}

                    </div>
                    <div className="comments-chat-input">
                        <input
                            value={currentComment}
                            onChange={(e) => setCurrentComment(e.target.value)}
                            placeholder="Type your message..."
                        />
                        <button onClick={handleSend}>Send</button>
                    </div>
                </div>
                <button onClick={onClose}>Cancel</button>
            </div>
        </div>
    );
};

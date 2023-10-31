import React, { useState, useEffect, useRef } from 'react';
import './Comments.css';
import {HttpTransportType, HubConnectionBuilder} from "@microsoft/signalr";

export const Comments = ({ show, onClose, topicId }) => {
    const [comments, setComments] = useState([]);
    const [currentComment, setCurrentComment] = useState('');
    const chatContentRef = React.createRef();
    const [connection, setConnection] = useState(null);

    useEffect(() => {
        if (show) {
            initWSConnection();
            
            fetch(`https://localhost:7283/Comment/get/${topicId}`)
                .then(response => response.json())
                .then(data => setComments(data))
                .catch(error => console.error("Error fetching comments:", error));
        }

        if (chatContentRef.current) {
            chatContentRef.current.scrollTop = chatContentRef.current.scrollHeight;
        }
    }, [show, topicId]);
    
    const initWSConnection = () => {
        const newConnection = new HubConnectionBuilder()
            .withUrl("https://localhost:7283/chat-hub", {
                skipNegotiation: true,
                transport: HttpTransportType.WebSockets
            })
            .withAutomaticReconnect()
            .build();

        newConnection.on("ReceiveMessage", (id, messageContent) => {
            const newComment = {
                id: id,
                commentText: messageContent
            }
            setComments(prevComments => [...prevComments,newComment])
        });

        newConnection.on("DeleteMessage", (messageId) => {
            setComments(prevComments => prevComments.filter(comment => comment.id !== messageId));
        });
        
        newConnection.start()
            .then(() => {
                newConnection.invoke("AddToBroadcastGroup", topicId);
                setConnection(newConnection);
            });
    }
    
    const handleSend = async () => {
        if (currentComment.trim() !== '') {            
            await connection.invoke("SendMessage", "27d9bf74-21aa-40ca-9790-43ae1d602e43", topicId, currentComment);
            setCurrentComment('');
        }
        if (chatContentRef.current) {
            chatContentRef.current.scrollTop = chatContentRef.current.scrollHeight;
        }
    };

    const handleDelete = async (commentId) => {
        if (commentId != null) {
            await connection.invoke("DeleteMessage",  String(commentId));
        }
    };

    if (!show) {
        if (connection) {
            connection.stop().then(() => {
                setConnection(connection);
            });
        }
        
        return null;   
    }

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

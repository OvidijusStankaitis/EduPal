import React, { useState, useEffect, useRef } from 'react';
import './Comments.css';
import { HttpTransportType, HubConnectionBuilder } from "@microsoft/signalr";

export const Comments = ({ show, onClose, topicId }) => {
    const [comments, setComments] = useState([]);
    const [currentComment, setCurrentComment] = useState('');
    const chatContentRef = useRef(null);
    const connectionRef = useRef(null);

    const fetchComments = () => {
        fetch(`https://localhost:7283/Comment/get/${topicId}`, {
            method: 'GET',
            credentials: 'include'
        })
            .then(response => response.json())
            .then(data => {
                setComments(data)}
            )
            .catch(error => console.error("Error fetching comments:", error));
    }

    useEffect(() => {
        const initializeConnection = async () => {
            const newConnection = new HubConnectionBuilder()
                .withUrl("https://localhost:7283/chat-hub", {
                    skipNegotiation: true,
                    transport: HttpTransportType.WebSockets
                })
                .withAutomaticReconnect()
                .build();

            newConnection.on("ReceiveMessage", (realId, messageContent, timeStamp, isFromCurrentUser) => {
                if (isFromCurrentUser) {
                    setComments(prevComments => {
                        return prevComments.map(comment => {
                            if (comment.isOptimistic && comment.isFromCurrentUser && comment.content === messageContent) {
                                return { ...comment, id: realId, timeStamp: timeStamp, isOptimistic: false };
                            }

                            return comment;
                        })
                    })
                }
                else {
                    const newComment = {
                        id: realId,
                        content: messageContent,
                        timeStamp: timeStamp,
                        isFromCurrentUser: isFromCurrentUser,
                        isOptimistic: false
                    }

                    setComments(prevComments => [...prevComments, newComment]);
                }

                console.log(comments)
            });

            newConnection.on("DeleteMessage", (messageId) => {
                setComments(prevComments => prevComments.filter(comment => comment.id !== messageId));
            });

            newConnection.start()
                .then(() => {
                    newConnection.invoke("AddToBroadcastGroup", topicId);
                    connectionRef.current = newConnection;
                    console.log("KONESHION: ", connectionRef.current)
                })
                .catch(e => console.log('Connection failed: ', e));

            fetchComments();
        }

        if (show && !connectionRef.current) {
            initializeConnection();
        }

        return () => {
            if(connectionRef.current && connectionRef.current.state === 'Connected') {
                connectionRef.current.stop()
                    .then(() => {
                        connectionRef.current = null;
                    })
                    .catch(e => console.log('Failed to stop connection', e));
            }
        }

    }, [show, topicId]);
    
    useEffect(() => {
        if (chatContentRef.current) {
            chatContentRef.current.scrollTop = chatContentRef.current.scrollHeight;
        }
    }, [comments]);

    const handleSend = async () => {
        if (currentComment.trim() !== '' && connectionRef.current.state === 'Connected') {
            const optimisticId = `temp-${Date.now()}`;
            const optimisticComment = {
                id: optimisticId,
                content: currentComment,
                timeStamp: Date.now(),
                isFromCurrentUser: true,
                isOptimistic: true
            };

            console.log("optimistic:: ", optimisticComment)
            setComments(prevComments => [...prevComments, optimisticComment]);

            try {
                await connectionRef.current.invoke("SendMessage", topicId, currentComment);
                setCurrentComment('');
            } catch (error) {
                setComments(prevComments => prevComments.filter(comment => comment.id !== optimisticId));
            }
        }
    };

    const handleDelete = async (commentId) => {
        if (commentId && !commentId.startsWith('temp-') && connectionRef.current && connectionRef.current.state === "Connected") {
            try {
                await connectionRef.current.invoke("DeleteMessage", commentId);
                setComments(prevComments => prevComments.filter(comment => comment.id !== commentId));
            } catch (error) {
                console.error("Error invoking 'DeleteMessage':", error);
            }
        } else {
            console.error("Cannot delete an optimistic or non-existing comment.");
        }
    };

    if (!show) {
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
                                    {comment.content}
                                </div>

                                { comment.isFromCurrentUser
                                    ? (<button
                                        className="delete-button1"
                                        onClick={() => handleDelete(comment.id)}>
                                        🗑️
                                    </button>)
                                    : (<div></div>)
                                }
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

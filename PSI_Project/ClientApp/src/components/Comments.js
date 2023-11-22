import React, { useState, useEffect, useRef } from 'react';
import './Comments.css';
import { HttpTransportType, HubConnectionBuilder } from "@microsoft/signalr";
import { useUserContext } from '../contexts/UserContext';

export const Comments = ({ show, onClose, topicId }) => {
    const { userId } = useUserContext();
    const [comments, setComments] = useState([]);
    const [currentComment, setCurrentComment] = useState('');
    const chatContentRef = useRef(null);
    const [connection, setConnection] = useState(null);

    // Initialize WebSocket connection
    useEffect(() => {
        const initializeConnection = async () => {
            const newConnection = new HubConnectionBuilder()
                .withUrl("https://localhost:7283/chat-hub", {
                    skipNegotiation: true,
                    transport: HttpTransportType.WebSockets
                })
                .withAutomaticReconnect()
                .build();

            newConnection.on("ReceiveMessage", (realId, senderId, messageContent) => {
                setComments(prevComments => {
                    // Replace the optimistic comment with the real one from the server
                    return prevComments.map(comment => {
                        if (comment.isOptimistic && comment.userId === userId && comment.commentText === messageContent) {
                            return { ...comment, id: realId, isOptimistic: false };
                        }
                        return comment;
                    });
                });
            });

            newConnection.on("DeleteMessage", (messageId) => {
                setComments(prevComments => prevComments.filter(comment => comment.id !== messageId));
            });

            newConnection.start()
                .then(() => {
                    newConnection.invoke("AddToBroadcastGroup", topicId);
                    setConnection(newConnection);
                })
                .catch(e => console.log('Connection failed: ', e));

            // Fetch initial comments
            fetch(`https://localhost:7283/Comment/get/${topicId}`, {
                method: 'GET',
                credentials: 'include'
            })
                .then(response => response.json())
                .then(data => setComments(data))
                .catch(error => console.error("Error fetching comments:", error));
        }

        if (show && (!connection || connection.state !== "Connected")) {
            initializeConnection()
                .then(newConnection => {
                    setConnection(newConnection); // Set the new connection
                })
                .catch(e => console.error('Connection initialization failed: ', e));
        }


        // Clean up function to close WebSocket connection
        return () => {
            if (connection) {
                connection.stop()
                    .then(() => {
                        console.log('Connection stopped');
                        setConnection(null); // Reset the connection state to null
                    })
                    .catch(e => console.log('Failed to stop connection', e));
            }
        };
    }, [show, connection, topicId]); // Adjust the dependencies as needed

    // Scroll to the bottom of the chat whenever comments update
    useEffect(() => {
        if (chatContentRef.current) {
            chatContentRef.current.scrollTop = chatContentRef.current.scrollHeight;
        }
    }, [comments]);

    const handleSend = async () => {
        if (currentComment.trim() !== '' && connection) {
            const optimisticId = `temp-${Date.now()}`;
            const optimisticComment = {
                id: optimisticId,
                userId: userId,
                commentText: currentComment,
                isOptimistic: true // Mark this comment as optimistic
            };

            // Add the optimistic comment to the local state
            setComments(prevComments => [...prevComments, optimisticComment]);

            try {
                // Send the message to the server
                await connection.invoke("SendMessage", userId, topicId, currentComment);
                // Clear the input field
                setCurrentComment('');
            } catch (error) {
                // If there's an error sending the message, remove the optimistic comment
                setComments(prevComments => prevComments.filter(comment => comment.id !== optimisticId));
            }
        }
    };

    const handleDelete = async (commentId) => {
        // Ensure we're using real IDs when attempting to delete
        if (commentId && !commentId.startsWith('temp-') && connection && connection.state === "Connected") {
            try {
                await connection.invoke("DeleteMessage", commentId);
                // Optimistically remove the comment from the local state
                setComments(prevComments => prevComments.filter(comment => comment.id !== commentId));
            } catch (error) {
                console.error("Error invoking 'DeleteMessage':", error);
            }
        } else {
            console.error("Cannot delete an optimistic or non-existing comment.");
        }
    };

    // If the component is not supposed to show, don't render anything
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
                                    {comment.commentText}
                                </div>

                                {comment.userId === userId
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

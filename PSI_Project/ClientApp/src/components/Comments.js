import React, { useState, useEffect, useRef } from 'react';
import './Comments.css';
import { HttpTransportType, HubConnectionBuilder } from "@microsoft/signalr";
import { useUserContext } from "../UserContext";

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

            newConnection.on("ReceiveMessage", (id, senderId, messageContent) => {
                // Ignore messages from the current user
                if (senderId !== userId) {
                    const newComment = {
                        id: id,
                        userId: senderId,
                        commentText: messageContent
                    };
                    setComments(prevComments => [...prevComments, newComment]);
                }
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
            fetch(`https://localhost:7283/Comment/get/${topicId}`)
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
            // Create a temporary unique ID for the new comment
            const tempId = Date.now(); // Or any other method to generate a unique ID

            const newComment = {
                id: tempId,
                userId: userId,
                commentText: currentComment
            };

            // Optimistically add the new comment to the local state
            setComments(prevComments => [...prevComments, newComment]);

            // Send the message
            await connection.invoke("SendMessage", userId, topicId, currentComment);

            setCurrentComment('');
        }
    };

    const handleDelete = async (commentId) => {
        if (commentId != null && connection && connection.state === "Connected") {
            try {
                await connection.invoke("DeleteMessage", commentId);
                // Possibly update state to reflect the deletion
            } catch (error) {
                console.error("Error invoking 'DeleteMessage':", error);
                // Handle the error appropriately in the UI
            }
        } else {
            console.error("Connection is not in a Connected state or commentId is null");
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

import React, {useEffect, useRef, useState} from 'react';
import './OpenAIDialogue.css';
import gpt from "../assets/gpt.webp";
import {useUserContext} from '../contexts/UserContext';

export const OpenAIDialogue = ({ show, onClose }) => {
    const { userEmail } = useUserContext();
    const [messages, setMessages] = useState([]);
    const [userMessage, setUserMessage] = useState("");
    const [isLoading, setIsLoading] = useState(false);

    const fetchMessages = async () => {
        try {
            const response = await fetch(`https://localhost:7283/OpenAI/get-messages`, {
                method: 'GET',
                credentials: 'include'
            });
            if (response.ok) {
                const data = await response.json();
                setMessages(data);
            } else {
                console.error('Failed to fetch messages:', await response.text());
            }
        } catch (error) {
            console.error('Error fetching messages:', error);
        }
    };

    useEffect(() => {
        if (show) {
            fetchMessages();
        }
    }, [show]);

    const handleSendMessage = async () => {
        console.log(userMessage);
        if (userMessage.trim()) {
            setMessages(prevMessages => [...prevMessages, { sender: 'user', text: userMessage.trim() }]);
            setUserMessage("");
            setIsLoading(true);

            try {
                const response = await fetch(`https://localhost:7283/OpenAI/send-message`, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    credentials: 'include',
                    body: JSON.stringify(userMessage),
                });
                if (response.ok) {
                    const data = await response.json();
                    setMessages(prevMessages => [...prevMessages, { sender: 'chatgpt', text: data.response }]);
                    setIsLoading(false);  
                } else {
                    console.error('Failed to send message:', await response.text());
                    setIsLoading(false);  
                }
            } catch (error) {
                console.error('Error sending message:', error);
                setIsLoading(false);  
            }
        }
    };

    const chatContentRef = useRef(null);

    useEffect(() => {
        const chatContentElement = chatContentRef.current;
        if (chatContentElement) {
            chatContentElement.scrollTop = chatContentElement.scrollHeight;
        }
    }, [messages]);

    if (!show) return null;

    return (
        <div className="openai-dialog">
            <div className="openai-dialog-content">
                <div className="greetingGPT">
                    <img src={gpt} alt="gpt" className="gpt-logo2" />
                    <h3>Hello, learner! I'm ChatGPT, what do you need help with?</h3>
                </div>
                <div className="gpt-chat">
                    <div className="gpt-chat-content" ref={chatContentRef}>
                        {messages.map((message, index) => (
                            <div key={index} className={message.sender === 'user' ? 'user' : 'chatgpt'}>
                                {message.text}
                            </div>
                        ))}
                        {isLoading && (
                            <div className="chatgpt">
                                <div className="spinner"></div> 
                            </div>
                        )}
                    </div>
                    <div className="gpt-chat-input">
                        <input
                            value={userMessage}
                            onChange={e => setUserMessage(e.target.value)}
                            placeholder="Type your message..."
                            onKeyPress={e => e.key === "Enter" && handleSendMessage()}
                        />
                        <button onClick={handleSendMessage}>Send</button>
                    </div>
                </div>
                <button onClick={onClose}>Cancel</button>
            </div>
        </div>
    );
};

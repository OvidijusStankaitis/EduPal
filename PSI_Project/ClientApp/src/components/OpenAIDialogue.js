import React, { useState } from 'react';
import './OpenAIDialogue.css';
import gpt from "../assets/gpt.webp";
import {useUserContext} from "../UserContext";

export const OpenAIDialogue = ({ show, onClose }) => {
    const [messages, setMessages] = useState([]);
    const [userMessage, setUserMessage] = useState("");

    const handleSendMessage = () => {
        if (userMessage.trim()) {
            setMessages([...messages, { sender: 'user', text: userMessage.trim() }]);
            setUserMessage("");
        }
    };
    
    if (!show) return null;
    
    return (
        <div className="openai-dialog">
            <div className="openai-dialog-content">
                <div className="greetingGPT">
                    <img src={gpt} alt="gpt" className="gpt-logo2" />
                    <h3>Hello, learner! I'm ChatGPT, what do you need help with?</h3>
                </div>
                <div className="gpt-chat">
                    <div className="gpt-chat-content">
                        {messages.map((message, index) => (
                            <div key={index} className={message.sender === 'user' ? 'user' : 'chatgpt'}>
                                {message.text}
                            </div>
                        ))}
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

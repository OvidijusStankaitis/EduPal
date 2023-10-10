import React, { useEffect } from 'react';
import './UserComponent.css';
import tomato from "../assets/tomato.webp";
import gpt from "../assets/gpt.webp";
import user from "../assets/user.webp";
import { useUserContext } from '../UserContext'; // Import the hook

export const UserComponent = () => {
    const { userEmail, setUsername, username } = useUserContext(); // Access user-related information

    useEffect(() => {
        const fetchUsername = async () => {
            try {
                const response = await fetch(`https://localhost:7283/User/get-name?email=${userEmail}`);
                if (response.ok) {
                    const data = await response.json();
                    console.log(data.name); // This should log the user's name
                    setUsername(data.name); // Set the username in the context
                } else {
                    console.error("Failed to fetch username.");
                }
            } catch (error) {
                console.error("An error occurred:", error);
            }
        };

        fetchUsername();
    }, [userEmail, setUsername]);

    return (
        <div className="user-component">
            <span className="username">{username}</span>
            <img src={user} alt="User" className="user-picture" />
            <span className="pomodoro-time">00:00</span>
            <img src={tomato} alt="Start Pomodoro" className="tomato" />
            <img src={gpt} alt="GPT Logo" className="gpt-logo" />
        </div>
    );
};

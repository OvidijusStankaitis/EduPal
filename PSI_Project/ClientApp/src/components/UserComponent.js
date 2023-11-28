import React, { useState, useEffect } from 'react';
import './UserComponent.css';
import tomato from "../assets/tomato.webp";
import gpt from "../assets/gpt.webp";
import user from "../assets/user.webp";
import { useUserContext } from '../UserContext';
import { useNavigate, useLocation} from "react-router-dom";

export const UserComponent = ({ setShowPomodoroDialog, setShowOpenAIDialog }) => {
    const { userEmail, setUsername, username, setUserEmail } = useUserContext();
    const [remainingTime, setRemainingTime] = useState(0);
    const [mode, setMode] = useState('study');
    const navigate = useNavigate();
    const location = useLocation();

    const fetchTimerState = async () => {
        try {
            const response = await fetch(`https://localhost:7283/Pomodoro/get-timer-state?userEmail=${encodeURIComponent(userEmail)}`);
            if (response.ok) {
                const data = await response.json();
                const { remainingTime, mode } = data;
                setRemainingTime(remainingTime);
                console.log("Remaining time: ", remainingTime);
                setMode(mode);
                if (mode === 'Short Break') {
                    navigate('/ShortBreak');
                    sessionStorage.setItem('previousPath', location.pathname);
                } else if (mode === 'Long Break') {
                    navigate('/LongBreak');
                    sessionStorage.setItem('previousPath', location.pathname);
                }
                console.log("Location: ", location.pathname);
            }
        } catch (error) {
            console.error("Error fetching timer state: ", error);
        }
    };

    useEffect(() => {
        const intervalId = setInterval(() => {
            (async () => {
                await fetchTimerState();
            })();
        }, 1000);

        return () => clearInterval(intervalId);
    }, [userEmail, fetchTimerState]); 

    const fetchUsername = async () => {
        try {
            const cachedUsername = localStorage.getItem('username');
            if (cachedUsername) {
                console.log("Setting username from cache: ", cachedUsername);
                setUsername(cachedUsername);
            } else {
                console.log("Fetching username from server");
                const response = await fetch(`https://localhost:7283/User/get-name?email=${userEmail}`);
                if (response.ok) {
                    const data = await response.json();
                    setUsername(data.name);
                    localStorage.setItem('username', data.name);
                }
            }
        } catch (error) {
            console.error("Error fetching username: ", error);
        }
    };

    useEffect(() => {
        if (!userEmail) {
            const cachedEmail = localStorage.getItem('userEmail');
            if (cachedEmail) {
                console.log("Setting userEmail from cache: ", cachedEmail);
                setUserEmail(cachedEmail); // Set it in the context
            }
        }

        if (userEmail) {
            console.log("User email exists, fetching username");
            fetchUsername();
        } else {
            console.log("No user email found");
        }
    }, [userEmail, setUsername, setUserEmail]);
    
    const handleStartPomodoro = () => {
        setShowPomodoroDialog(true);
    };

    const handleStartOpenAI = () => {
        setShowOpenAIDialog(true);
    };

    const formatTime = (time) => {
        if (typeof time !== 'number' || isNaN(time)) {
            // Return a default or placeholder value if time is not a number
            return "00:00";
        }

        const minutes = Math.floor(time / 60);
        const seconds = time % 60;
        console.log("Minutes: ", minutes);
        console.log("Seconds: ", seconds);
        return `${String(minutes).padStart(2, '0')}:${String(seconds).padStart(2, '0')}`;
    };

    return (
        <div className="user-component">
            <span className="username">{username}</span>
            <img src={user} alt="User" className="user-picture" />
            <span className="pomodoro-time">{formatTime(remainingTime)}</span>
            <img src={tomato} alt="Start Pomodoro" className="tomato" onClick={handleStartPomodoro} />
            <img src={gpt} alt="GPT Logo" className="gpt-logo" onClick={handleStartOpenAI}/>
        </div>
    );
};

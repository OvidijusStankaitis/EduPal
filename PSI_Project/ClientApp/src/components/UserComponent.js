import React, { useState, useEffect } from 'react';
import './UserComponent.css';
import tomato from "../assets/tomato.webp";
import gpt from "../assets/gpt.webp";
import user from "../assets/user.webp";
import { useUserContext } from '../UserContext';

const DURATIONS = {
    Low: { study: 5, shortBreak: 2, longBreak: 3 },
    Medium: { study: 6, shortBreak: 3, longBreak: 4 },
    High: { study: 7, shortBreak: 4, longBreak: 5 },
};

export const UserComponent = ({ setShowPomodoroDialog, setShowOpenAIDialog }) => {
    const { userEmail, setUsername, username, setUserEmail } = useUserContext();
    const [remainingTime, setRemainingTime] = useState(
        parseInt(localStorage.getItem('remainingTime'), 10) || 0
    );
    const [isActive, setIsActive] = useState(
        localStorage.getItem('shouldStartTimer') === 'true' ||
        localStorage.getItem('isActive') === 'true'
    );
    const [mode, setMode] = useState('study');
    const [completedStudySessions, setCompletedStudySessions] = useState(0);


    const fetchUsername = async () => {
        try {
            const cachedUsername = localStorage.getItem('username');
            if (cachedUsername) {
                console.log("Setting username from cache: ", cachedUsername);
                setUsername(cachedUsername);
            } else {
                console.log("Fetching username from server");
                
                const response = await fetch(`https://localhost:7283/User/get-name?email=${userEmail}`, {
                    method: 'GET',
                    credentials: 'include'
                });
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


    useEffect(() => {
        startTimerBasedOnLocalStorage();
        window.addEventListener('storage', startTimerBasedOnLocalStorage);
        return () => {
            window.removeEventListener('storage', startTimerBasedOnLocalStorage);
        };
    }, []);

    const startTimerBasedOnLocalStorage = () => {
        const shouldStart = localStorage.getItem('shouldStartTimer') === 'true';
        const storedRemainingTime = parseInt(localStorage.getItem('remainingTime'), 10);
        const intensity = localStorage.getItem('pomodoroIntensity');

        if (storedRemainingTime) {
            setRemainingTime(storedRemainingTime);
            setIsActive(true);
        } else if (shouldStart) {
            if (intensity) {
                setMode('study');
                setRemainingTime(DURATIONS[intensity].study);
                setIsActive(true);
                localStorage.setItem('shouldStartTimer', 'false');
            }
        } else if (intensity && !shouldStart && !isActive && remainingTime === 0) {
            setRemainingTime(DURATIONS[intensity].study);
        }
    };

    useEffect(() => {
        if (!isActive) return;

        const interval = setInterval(() => {
            setRemainingTime(prev => {
                if (prev <= 0) {
                    localStorage.setItem('isActive', 'false');
                    switch (mode) {
                        case 'study':
                            if (completedStudySessions >= 3) {
                                setCompletedStudySessions(0);
                                setMode('longBreak');
                                return DURATIONS[localStorage.getItem('pomodoroIntensity')].longBreak;
                            } else {
                                setCompletedStudySessions(prevCount => prevCount + 1);
                                setMode('shortBreak');
                                return DURATIONS[localStorage.getItem('pomodoroIntensity')].shortBreak;
                            }
                        case 'shortBreak':
                            setMode('study');
                            return DURATIONS[localStorage.getItem('pomodoroIntensity')].study;
                        case 'longBreak':
                            setMode('study');
                            return DURATIONS[localStorage.getItem('pomodoroIntensity')].study;
                        default:
                            setIsActive(false);
                            return 0;
                    }
                }
                return prev - 1;
            });
        }, 1000);

        return () => clearInterval(interval);
    }, [isActive, mode, completedStudySessions]);

    useEffect(() => {
        if (isActive) {
            localStorage.setItem('remainingTime', remainingTime);
            localStorage.setItem('isActive', 'true');
        }
    }, [remainingTime, isActive]);

    const handleStartPomodoro = () => {
        setShowPomodoroDialog(true);
    };

    const handleStartOpenAI = () => {
        setShowOpenAIDialog(true);
    };
    
    const formatTime = (time) => {
        const minutes = Math.floor(time / 60);
        const seconds = time % 60;
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

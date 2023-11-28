import React, { useState, useEffect } from 'react';
import './UserComponent.css';
import tomato from "../assets/tomato.webp";
import gpt from "../assets/gpt.webp";
import user from "../assets/user.webp";
import {setUserName, useUserContext} from '../contexts/UserContext';
import { useNavigate, useLocation} from "react-router-dom";

export const UserComponent = ({ setShowPomodoroDialog, setShowOpenAIDialog }) => {
    const { setUserName, userName } = useUserContext();
    const [remainingTime, setRemainingTime] = useState(0);
    const [mode, setMode] = useState('study');
    const navigate = useNavigate();
    const location = useLocation();

    const fetchTimerState = async () => {
        try {
            const response = await fetch(`https://localhost:7283/Pomodoro/get-timer-state`, {
                method: 'GET',
                credentials: 'include'
            });      
            
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
    }, [fetchTimerState]);

    useEffect(() => {
        const intervalId = setInterval(() => {
            (async () => {
                await fetchTimerState();
            })();
        }, 1000);

        return () => clearInterval(intervalId);
    }, [fetchTimerState]);

    useEffect(() => {
       fetchUserName()
    }, [userName]);

    const fetchUserName = async () => {
        const userName = localStorage.getItem('userName')
        if (userName != null) {
            setUserName(userName)
            return
        }

        try {
            const response = await  fetch(`https://localhost:7283/User/get-user-name`, {
                method: 'GET',
                credentials: 'include'
            });

            const data = await response.json()
            localStorage.setItem('userName', data.name)
            setUserName(data.name)
        } catch(err) {
            console.error('error fetching user name: ' + err)
        }
    }
        
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
            <span className="username">{userName}</span>
            <img src={user} alt="User" className="user-picture" />
            <span className="pomodoro-time">{formatTime(remainingTime)}</span>
            <img src={tomato} alt="Start Pomodoro" className="tomato" onClick={handleStartPomodoro} />
            <img src={gpt} alt="GPT Logo" className="gpt-logo" onClick={handleStartOpenAI}/>
        </div>
    );
};

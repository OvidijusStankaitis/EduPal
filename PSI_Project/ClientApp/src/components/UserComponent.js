import React, { useState, useEffect, useRef } from 'react';
import ReactDOM from 'react-dom';
import './UserComponent.css';
import tomato from "../assets/tomato.webp";
import gpt from "../assets/gpt.webp";
import user from "../assets/user.webp";
import { useUserContext } from '../UserContext';

export const UserComponent = ({ setShowPomodoroDialog, setShowOpenAIDialog, setShowCreateGoalDialog, setShowViewGoalsDialog }) => {
    const { userEmail, setUsername, username, setUserEmail } = useUserContext();
    const [remainingTime, setRemainingTime] = useState(0);
    const [mode, setMode] = useState('study');
    const [dropdownPosition, setDropdownPosition] = useState({});
    const userIconRef = useRef(null);
    const [showDropdown, setShowDropdown] = useState(false);
    const toggleDropdown = () => setShowDropdown(prev => !prev);
    const userComponentRef = useRef(null);


    const fetchTimerState = async () => {
        try {
            const response = await fetch(`https://localhost:7283/Pomodoro/get-timer-state?userEmail=${encodeURIComponent(userEmail)}`);
            if (response.ok) {
                const data = await response.json();
                const { remainingTime, mode } = data;
                setRemainingTime(remainingTime);
                console.log("Remaining time: ", remainingTime);
                setMode(mode);
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
            return "00:00";
        }

        const minutes = Math.floor(time / 60);
        const seconds = time % 60;
        console.log("Minutes: ", minutes);
        console.log("Seconds: ", seconds);
        return `${String(minutes).padStart(2, '0')}:${String(seconds).padStart(2, '0')}`;
    };

    const handleMouseLeave = () => {
        setShowDropdown(false);
    };

    const handleCreateGoal = () => {
        setShowCreateGoalDialog(true);
        setShowDropdown(false);
    };

    const handleViewGoals = () => {
        setShowViewGoalsDialog(true);
        setShowDropdown(false);
    };

    const updateDropdownPosition = () => {
        if (userComponentRef.current) {
            const rect = userComponentRef.current.getBoundingClientRect();
            setDropdownPosition({
                top: `${rect.bottom + window.scrollY}px`,
                left: `${rect.left}px`,
                width: `${rect.width}px`,
                margin: `${rect.margin}px`
            });
        }
    };

    useEffect(() => {
        updateDropdownPosition();
        window.addEventListener('resize', updateDropdownPosition);
        window.addEventListener('scroll', updateDropdownPosition, true);

        return () => {
            window.removeEventListener('resize', updateDropdownPosition);
            window.removeEventListener('scroll', updateDropdownPosition, true);
        };
    }, []);

    useEffect(() => {
        const handleOutsideClick = (event) => {
            if (showDropdown && userIconRef.current && !userIconRef.current.contains(event.target)) {
                setShowDropdown(false);
            }
        };

        document.addEventListener('mousedown', handleOutsideClick);

        return () => {
            document.removeEventListener('mousedown', handleOutsideClick);
        };
    }, [showDropdown]);

    const dropdown = showDropdown ? (
        <div className="user-dropdown" style={{ ...dropdownPosition, position: 'fixed', zIndex: 1000 }}>
            <div className="dropdown-item" onMouseDown={handleCreateGoal}>Create goal</div>
            <div className="dropdown-item" onMouseDown={handleViewGoals}>View goals</div>
        </div>
    ) : null;

    return (
        <div ref={userComponentRef} className="user-component" onMouseLeave={handleMouseLeave}>
            <span className="username">{username}</span>
            <img ref={userIconRef} src={user} alt="User" className="user-picture" onClick={toggleDropdown} />
            {dropdown && ReactDOM.createPortal(
                dropdown,
                document.getElementById('portal-root')
            )}
            <span className="pomodoro-time">{formatTime(remainingTime)}</span>
            <img src={tomato} alt="Start Pomodoro" className="tomato" onClick={handleStartPomodoro} />
            <img src={gpt} alt="GPT Logo" className="gpt-logo" onClick={handleStartOpenAI}/>
        </div>
    );
};
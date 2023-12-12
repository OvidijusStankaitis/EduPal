import React, { useState, useEffect, useRef } from 'react';
import './UserComponent.css';
import tomato from "../assets/tomato.webp";
import gpt from "../assets/gpt.webp";
import user from "../assets/user.webp";
import {setUserName, useUserContext} from '../contexts/UserContext';
import { useNavigate, useLocation} from "react-router-dom";

export const UserComponent = ({ setShowPomodoroDialog, setShowOpenAIDialog, setShowCreateGoalDialog, setShowViewGoalsDialog }) => {
    const { setUserName, userName } = useUserContext();
    const [remainingTime, setRemainingTime] = useState(0);
    const [mode, setMode] = useState('study');
    const navigate = useNavigate();
    const location = useLocation();
    const userIconRef = useRef(null);
    const [showDropdown, setShowDropdown] = useState(false);
    const toggleDropdown = () => setShowDropdown(prev => !prev);
    const userComponentRef = useRef(null);
    const [currentSubjectId, setCurrentSubjectId] = useState('');

    // Retrieve userId from localStorage directly
    const userId = localStorage.getItem('userId');

    useEffect(() => {
        const fetchCurrentSubject = async () => {
            const response = await fetch(`https://localhost:7283/Goals/current-subject`, {
                method: 'GET',
                credentials: 'include'
            });
            if (response.ok) {
                const data = await response.json();
                setCurrentSubjectId(data.currentSubjectId);
            }
        };

        fetchCurrentSubject();
    }, [userId]);

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

    const updateStudyTime = async () => {
        console.log('Updated study time');
        if (!currentSubjectId) return;

        try {
            await fetch('https://localhost:7283/Goals/update-study-time', {
                method: 'POST',
                credentials: 'include',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    UserId: userId,
                    SubjectId: currentSubjectId,
                    ElapsedHours: 0.0167 // Equivalent to one minute
                }),
            });
        } catch (error) {
            console.error('Error updating study time:', error);
        }
    };

    useEffect(() => {
        const intervalId = setInterval(() => {
            updateStudyTime();
        }, 1000); // 60000 for one minute and 1000 for one second

        return () => clearInterval(intervalId);
    }, [userId, currentSubjectId]);


    const dropdown = showDropdown ? (
        <div className="user-dropdown">
            <div className="dropdown-item" onMouseDown={handleCreateGoal}>Create goal</div>
            <div className="dropdown-item" onMouseDown={handleViewGoals}>View goals</div>
        </div>
    ) : null;

    return (
        <div ref={userComponentRef} className="user-component" onMouseLeave={handleMouseLeave}>
            <span className="username">{userName}</span>
            <img ref={userIconRef} src={user} alt="User" className="user-picture" onClick={toggleDropdown} />
            {dropdown}
            <span className="pomodoro-time">{formatTime(remainingTime)}</span>
            <img src={tomato} alt="Start Pomodoro" className="tomato" onClick={handleStartPomodoro} />
            <img src={gpt} alt="GPT Logo" className="gpt-logo" onClick={handleStartOpenAI}/>
        </div>
    );
};
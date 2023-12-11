import React, { useEffect, useState, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import './Break.css';
import sunsetImage from '../assets/sunset.webp';

export const LongBreakComponent = () => {
    const [remainingTime, setRemainingTime] = useState(0);
    const navigate = useNavigate();

    const formatTime = (time) => {
        if (typeof time !== 'number' || isNaN(time)) {
            return "00:00";
        }
        const minutes = Math.floor(time / 60);
        const seconds = time % 60;
        return `${String(minutes).padStart(2, '0')}:${String(seconds).padStart(2, '0')}`;
    };

    const fetchTimerState = useCallback(async () => {
        try {
            const response = await fetch(`https://localhost:7283/Pomodoro/get-timer-state`, {
                method: 'GET',
                credentials: 'include'
            });
            if (response.ok) {
                const data = await response.json();
                setRemainingTime(data.remainingTime);

                if (data.mode !== 'Long Break') {
                    const previousPath = sessionStorage.getItem('previousPath') || '/';
                    navigate(previousPath);
                }
            }
        } catch (error) {
            console.error("Error fetching timer state: ", error);
        }
    }, [navigate]);

    useEffect(() => {
        const intervalId = setInterval(() => {
            (async () => {
                await fetchTimerState();
            })();
        }, 1000);

        return () => clearInterval(intervalId);
    }, [fetchTimerState]);

    return (
        <div className="break-container">
            <img src={sunsetImage} alt="Long Break" />
            <p>Relax for a while...</p>
            <p>Time Left - {formatTime(remainingTime)}</p>
        </div>
    );
};

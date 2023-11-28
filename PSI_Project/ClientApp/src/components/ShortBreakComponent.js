import React, { useEffect, useState, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import './Break.css';
import coffeeImage from '../assets/coffee.webp';
import { useUserContext } from '../UserContext';

export const ShortBreakComponent = () => {
    const { userEmail } = useUserContext();
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
            const response = await fetch(`https://localhost:7283/Pomodoro/get-timer-state?userEmail=${encodeURIComponent(userEmail)}`);
            if (response.ok) {
                const data = await response.json();
                setRemainingTime(data.remainingTime);

                if (data.mode !== 'Short Break') {
                    const previousPath = sessionStorage.getItem('previousPath') || '/';
                    navigate(previousPath);
                }
            }
        } catch (error) {
            console.error("Error fetching timer state: ", error);
        }
    }, [userEmail, navigate]);

    useEffect(() => {
        const intervalId = setInterval(() => {
            (async () => {
                await fetchTimerState();
            })();
        }, 1000);

        return () => clearInterval(intervalId);
    }, [userEmail, fetchTimerState]);

    return (
        <div className="break-container">
            <img src={coffeeImage} alt="Coffee Break" />
            <p>Take a break...</p>
            <p>Time Left - {formatTime(remainingTime)}</p>
        </div>
    );
};

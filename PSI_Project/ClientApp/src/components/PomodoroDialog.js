import React, { useState } from 'react';
import './PomodoroDialog.css';

export const PomodoroDialog = ({ show, onClose, }) => {
    const [intensity, setIntensity] = useState(localStorage.getItem('pomodoroIntensity') || '');

    const handleIntensityChange = (event) => {
        setIntensity(event.target.value);
    };

    const startTimer = async (intensity) => {
        try {
            const response = await fetch('https://localhost:7283/Pomodoro/start-timer', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                credentials: 'include',
                body: JSON.stringify({ intensity })
            });

            if (!response.ok) {
                const errorData = await response.json();
                console.error("Error starting timer: ", errorData);
            }
        } catch (error) {
            console.error("Error: ", error);
        }
    };

    const stopTimer = async () => {
        try {
            const response = await fetch('https://localhost:7283/Pomodoro/stop-timer', {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json'
                },
                credentials: 'include'
            });

            if (!response.ok) {
                const errorData = await response.json();
                console.error("Error stopping timer: ", errorData);
            }
        } catch (error) {
            console.error("Error: ", error);
        }
    };

    const handleConfirm = async () => {
        await startTimer(intensity);
        onClose();
    };

    const handleStop = async () => {
        await stopTimer();
        onClose();
    };

    if (!show) return null;

    return (
        <div className="pomodoro-dialog">
            <div className="pomodoro-dialog-content">
                <h3>Select Intensity</h3>
                <select value={intensity} onChange={handleIntensityChange}>
                    <option value="" disabled>Select an option...</option>
                    <option value="Low">Low</option>
                    <option value="Medium">Medium</option>
                    <option value="High">High</option>
                </select>
                <div>
                    <button onClick={handleConfirm}>Start Timer</button>
                    <button onClick={handleStop}>Stop Timer</button>
                    <button onClick={onClose}>Cancel</button>
                </div>
                <p className="time-desc">
                    Low - Study: 15 minutes, Short Break: 3 minutes, Long Break: 10 minutes<br/><br/>
                    Medium - Study: 25 minutes, Short Break: 5 minutes, Long Break: 15 minutes<br/><br/>
                    High - Study: 30 minutes, Short Break: 5 minutes, Long Break: 20 minutes<br/><br/>
                </p>
            </div>
        </div>
    );
};

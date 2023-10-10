import React, { useState } from 'react';
import './PomodoroDialog.css';

export const PomodoroDialog = ({ show, onClose }) => {
    const [intensity, setIntensity] = useState(localStorage.getItem('pomodoroIntensity') || '');

    const handleIntensityChange = (event) => {
        setIntensity(event.target.value);
    };

    const handleConfirm = () => {
        localStorage.setItem('pomodoroIntensity', intensity);
        localStorage.setItem('shouldStartTimer', 'true');
        onClose();
        window.location.reload();  // force reload to start the timer right away
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
                    <button onClick={handleConfirm}>Confirm</button>
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

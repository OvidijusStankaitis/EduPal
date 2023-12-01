import React, { useState, useEffect } from 'react';
import './ViewGoalsDialog.css';
import { useUserContext } from "../UserContext"; // Update the path as necessary

export const ViewGoalsDialog = ({ show, onClose }) => {
    const { userId } = useUserContext();
    const [goals, setGoals] = useState([]);
    const [error, setError] = useState('');

    useEffect(() => {
        const fetchGoals = async () => {
            if (show) {
                try {
                    const response = await fetch(`https://localhost:7283/goals/view-all/${userId}`);
                    if (!response.ok) {
                        throw new Error('Failed to fetch goals');
                    }
                    const data = await response.json();
                    setGoals(data);
                } catch (error) {
                    console.error('Error fetching goals:', error);
                    setError('Failed to fetch goals. Please try again.');
                }
            }
        };

        fetchGoals();
    }, [show, userId]);

    const renderDuration = (totalHours) => {
        const hours = Math.floor(totalHours);
        const minutes = Math.round((totalHours - hours) * 60);
        return `${hours}h ${minutes}m`;
    };

    if (!show) return null;

    return (
        <div className="view-goals-dialog">
            <div className="view-goals-dialog-content">
                <h3>Your Goals</h3>
                <div className="table-scrollable">
                    <table>
                        <thead>
                        <tr>
                            <th>Date Created</th>
                            <th>Target Time</th>
                            <th>Studied Time</th>
                        </tr>
                        </thead>
                        <tbody>
                        {goals.map((goal) => (
                            <tr key={goal.id}>
                                <td>{new Date(goal.goalDate).toLocaleDateString()}</td>
                                <td>{renderDuration(goal.targetHours)}</td>
                                <td>{renderDuration(goal.actualHoursStudied)}</td>
                            </tr>
                        ))}
                        </tbody>
                    </table>
                </div>
                {error && <div className="error-message">{error}</div>}
                <button onClick={onClose}>Close</button>
            </div>
        </div>
    );
};

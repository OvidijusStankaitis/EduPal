import React, {useEffect, useState} from 'react';
import { useNavigate } from 'react-router-dom';
import './LogInPanel.css';

export const LogInPanel = () => {
    const [showDialog, setShowDialog] = useState(false);
    const [targetRoute, setTargetRoute] = useState('');
    const [password, setPassword] = useState('');
    const [repeatPassword, setRepeatPassword] = useState('');
    const navigate = useNavigate();

    useEffect(() => {
        const numberOfStars = 100;
        const stars = []; // To keep track of the stars for cleanup

        for (let i = 0; i < numberOfStars; i++) {
            const star = document.createElement('div');
            star.classList.add('star');

            // Random position
            star.style.top = `${Math.random() * 100}vh`;
            star.style.left = `${Math.random() * 100}vw`;

            // Random direction
            const angle = Math.random() * 2 * Math.PI;
            star.style.setProperty('--dx', Math.cos(angle));
            star.style.setProperty('--dy', Math.sin(angle));

            // Random animation duration
            star.style.animationDuration = `${2 + Math.random() * 5}s`;

            document.body.appendChild(star);
            stars.push(star); // Add the star to the stars array
        }

        return () => {
            // Cleanup: Remove the stars when the component unmounts
            stars.forEach(star => document.body.removeChild(star));
        };
    }, []);

    const handleLoginSubmit = (e) => {
        e.preventDefault();
        navigate(targetRoute);
    };

    const handleRegisterSubmit = (e) => {
        e.preventDefault();
        if (password !== repeatPassword) {
            alert('Passwords do not match!');
            return;
        }
        navigate(targetRoute);
    };

    const showLoginDialog = (route) => {
        setTargetRoute(route);
        setShowDialog(true);
    };

    return (
        <div className="container">
            <h1 className="edu">EduPal</h1>
            <span className="buttons">
                <button onClick={() => showLoginDialog('/Subjects')}>START LEARNING!</button>
            </span>
            {showDialog &&
                <div className="dialog">
                    <div className="dialog-content">
                        <div className="form-section">
                            <h1 className="form-name">Log In</h1>
                            <form onSubmit={handleLoginSubmit}>
                                <input type="email" placeholder="Email" required />
                                <input type="password" placeholder="Password" required />
                                <button type="submit">Log In</button>
                            </form>
                        </div>
                        <div className="form-section">
                            <h1 className="form-name">Register</h1>
                            <form onSubmit={handleRegisterSubmit}>
                                <input type="text" placeholder="Name" required />
                                <input type="text" placeholder="Surname" required />
                                <input type="email" placeholder="Email" required />
                                <input type="password" placeholder="Password" required value={password} onChange={e => setPassword(e.target.value)} />
                                <input type="password" placeholder="Repeat Password" required value={repeatPassword} onChange={e => setRepeatPassword(e.target.value)} />
                                <button type="submit">Register</button>
                            </form>
                        </div>
                    </div>
                    <div className="dialog-footer">
                        <button type="button" onClick={() => setShowDialog(false)}>Cancel</button>
                    </div>
                </div>
            }
        </div>
    );
};

import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import './LogInPanel.css';
import { useUserContext } from '../UserContext'; // Import the hook

export const LogInPanel = () => {
    const { setUserEmail } = useUserContext(); // Access setUserEmail from the context
    const [showDialog, setShowDialog] = useState(false);
    const [targetRoute, setTargetRoute] = useState('');
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [repeatPassword, setRepeatPassword] = useState('');
    const [name, setName] = useState('');
    const [surname, setSurname] = useState('');
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

    const handleLoginSubmit = async (e) => {
        e.preventDefault();
        // Clear the stored username when a new user logs in
        localStorage.removeItem('username');
        const response = await fetch(`https://localhost:7283/User/login`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ email, password }),
        });
        const data = await response.json();
        if (data.success) {
            localStorage.clear();
            setUserEmail(email); // Set the user's email directly in the parent component
            localStorage.setItem('userEmail', email);
            navigate(targetRoute);
        }
    };

    const handleRegisterSubmit = async (e) => {
        e.preventDefault();
        if (password !== repeatPassword) {
            return;
        }
        const response = await fetch(`https://localhost:7283/User/register`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ name, surname, email, password })
        });
        const data = await response.json();
        if (data.success) {
            localStorage.clear();
            setUserEmail(email);
            localStorage.setItem('userEmail', email);
            navigate(targetRoute);
        }
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
                            <form onSubmit={handleLoginSubmit} autoComplete={"on"}>
                                <input type="email" placeholder="Email" required onChange={e => setEmail(e.target.value)} />
                                <input type="password" placeholder="Password" required onChange={e => setPassword(e.target.value)} />
                                <button type="submit">Log In</button>
                            </form>
                        </div>
                        <div className="form-section">
                            <h1 className="form-name">Register</h1>
                            <form onSubmit={handleRegisterSubmit} autoComplete={"on"}>
                                <input type="text" placeholder="Name" required onChange={e => setName(e.target.value)} />
                                <input type="text" placeholder="Surname" required onChange={e => setSurname(e.target.value)} />
                                <input type="email" placeholder="Email" required onChange={e => setEmail(e.target.value)} />
                                <input type="password" placeholder="Password" required onChange={e => setPassword(e.target.value)} />
                                <input type="password" placeholder="Repeat Password" required onChange={e => setRepeatPassword(e.target.value)} />
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

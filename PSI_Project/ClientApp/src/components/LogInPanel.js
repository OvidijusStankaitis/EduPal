import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import './LogInPanel.css';

export const LogInPanel = () => {
    const [showDialog, setShowDialog] = useState(false);
    const [targetRoute, setTargetRoute] = useState('');
    const [password, setPassword] = useState('');
    const [repeatPassword, setRepeatPassword] = useState('');
    const navigate = useNavigate();

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
        <div className={showDialog ? "container blurred" : "container"}>
            <h1 className="edu">EduPal</h1>
            <span className="buttons">
                <button onClick={() => showLoginDialog('/user')}>USER</button>
                <button onClick={() => showLoginDialog('/admin')}>ADMIN</button>
            </span>
            {showDialog &&
                <div className="dialog">
                    <div className="dialog-header">Login/Register</div>
                    <div className="dialog-content">
                        <div className="form-section">
                            <form onSubmit={handleLoginSubmit}>
                                <input type="email" placeholder="Email" required />
                                <input type="password" placeholder="Password" required />
                                <button type="submit">Log In</button>
                            </form>
                        </div>
                        <div className="form-section">
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

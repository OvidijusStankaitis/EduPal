import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import './LogInPanel.css';

export const LogInPanel = () => {
    const [showDialog, setShowDialog] = useState(false);
    const [targetRoute, setTargetRoute] = useState('');
    const navigate = useNavigate();

    const handleLoginSubmit = (e) => {
        e.preventDefault();
        navigate(targetRoute);
    };

    const showLoginDialog = (route) => {
        setTargetRoute(route);
        setShowDialog(true);
    };

    return (
        <div className={showDialog ? "container blurred" : "container"}>
            <span className="buttons">
                <a onClick={() => showLoginDialog('/user')} className="btn-flip user" data-back="LOG IN" data-front="USER"></a>
                <a onClick={() => showLoginDialog('/admin')} className="btn-flip admin" data-back="LOG IN" data-front="ADMIN"></a>
            </span>
            {showDialog &&
                <div className="dialog">
                    <form onSubmit={handleLoginSubmit}>
                        <input type="text" placeholder="Username" required />
                        <input type="password" placeholder="Password" required />
                        <button type="submit">Log In</button>
                        <button type="button" onClick={() => setShowDialog(false)}>Close</button>
                    </form>
                </div>
            }
        </div>
    );
};

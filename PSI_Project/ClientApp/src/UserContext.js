import { createContext, useContext, useState, useEffect } from 'react';

const UserContext = createContext();

export const useUserContext = () => {
    return useContext(UserContext);
};

export const UserProvider = ({ children }) => {
    // Initialize state from localStorage
    const [userId, setUserId] = useState(localStorage.getItem('userId') || '');
    const [userEmail, setUserEmail] = useState(localStorage.getItem('userEmail') || '');
    const [username, setUsername] = useState(localStorage.getItem('username') || '');

    // Update localStorage when values change
    useEffect(() => {
        localStorage.setItem('userId', userId);
        localStorage.setItem('userEmail', userEmail);
        localStorage.setItem('username', username);
    }, [userId, userEmail, username]);

    return (
        <UserContext.Provider value={{ userId, setUserId, userEmail, setUserEmail, username, setUsername }}>
            {children}
        </UserContext.Provider>
    );
};

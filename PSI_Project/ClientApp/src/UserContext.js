import { createContext, useContext, useState } from 'react';

const UserContext = createContext();

export const useUserContext = () => {
    return useContext(UserContext);
};

export const UserProvider = ({ children }) => {
    const [userId, setUserId] = useState('')
    const [userEmail, setUserEmail] = useState('');
    const [username, setUsername] = useState('');

    return (
        <UserContext.Provider value={{ userId, setUserId, userEmail, setUserEmail, username, setUsername }}>
            {children}
        </UserContext.Provider>
    );
};

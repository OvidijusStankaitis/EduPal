import { createContext, useContext, useState } from 'react';

const UserContext = createContext(null);

export const useUserContext = () => {
    return useContext(UserContext);
};

export const UserProvider = ({ children }) => {
    const [userName, setUserName] = useState('');
    const [userEmail, setUserEmail] = useState('');

    return (
        <UserContext.Provider value={{ userName, setUserName, userEmail, setUserEmail }}>
            {children}
        </UserContext.Provider>
    );
};

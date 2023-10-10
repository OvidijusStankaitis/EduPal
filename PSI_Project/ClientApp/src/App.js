import React from 'react';
import './App.css';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { LogInPanel } from './components/LogInPanel';
import { UserComponent } from './components/UserComponent';
import { Conspectus } from './components/Conspectus';
import { Subjects } from './components/Subjects';
import { Topics } from './components/Topics';
import { UserProvider } from './UserContext'; // Import UserProvider

function App() {
    return (
        <UserProvider> {/* Wrap your app with UserProvider */}
            <BrowserRouter>
                <Routes>
                    <Route path="/" element={<LogInPanel />} />
                    <Route path="/Subjects" element={<Subjects />} />
                    <Route path="/Subjects/:subjectId-Topics" element={<Topics />} />
                    <Route path="/Subjects/:subjectId-Topics/:topicId-Conspectus" element={<Conspectus />} />
                    <Route path="/User" element={<UserComponent />} />
                </Routes>
            </BrowserRouter>
        </UserProvider>
    );
}

export default App;

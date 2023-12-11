import React from 'react';
import './App.css';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { LogInPanel } from './components/LogInPanel';
import { UserComponent } from './components/UserComponent';
import { Conspectus } from './components/Conspectus';
import { Subjects } from './components/Subjects';
import { Topics } from './components/Topics';
import { ShortBreakComponent } from "./components/ShortBreakComponent";
import { LongBreakComponent } from "./components/LongBreakComponent";
import { UserProvider } from './contexts/UserContext'; 

function App() {
    return (
        <UserProvider> 
            <BrowserRouter>
                <Routes>
                    <Route path="/" element={<LogInPanel />} />
                    <Route path="/Subjects" element={<Subjects />} />
                    <Route path="/Subjects/:subjectId-Topics" element={<Topics />} />
                    <Route path="/Subjects/:subjectId-Topics/:topicId-Conspectus" element={<Conspectus />} />
                    <Route path="/ShortBreak" element={<ShortBreakComponent />} />
                    <Route path="/LongBreak" element={<LongBreakComponent />} />
                    <Route path="/User" element={<UserComponent />} />
                </Routes>
            </BrowserRouter>
        </UserProvider>
    );
}

export default App;

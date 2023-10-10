import React from 'react';
import './App.css';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { LogInPanel } from './components/LogInPanel';
import { Conspectus } from './components/Conspectus';
import {Subjects} from "./components/Subjects";
import {Topics} from "./components/Topics";

function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<LogInPanel />} />
                <Route path="/Subjects" element={<Subjects />} />
                <Route path="/Subjects/:subjectId-Topics" element={<Topics />} />
                <Route path="/Subjects/:subjectId-Topics/:topicId-Conspectus" element={<Conspectus />} />
            </Routes>
        </BrowserRouter>
    );
}

export default App;

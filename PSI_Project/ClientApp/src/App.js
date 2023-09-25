import React from 'react';
import './App.css';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { LogInPanel } from './components/LogInPanel';
import { UserPanel } from './components/UserPanel';
import { AdminPanel } from './components/AdminPanel';

function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<LogInPanel />} />
                <Route path="/user" element={<UserPanel />} />
                <Route path="/admin" element={<AdminPanel />} />
            </Routes>
        </BrowserRouter>
    );
}

export default App;

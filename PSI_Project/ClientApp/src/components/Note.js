import React, { useState } from "react";
import "./Note.css";

export const Note = ({ show, onClose, topicId }) => {
    const [noteName, setNoteName] = useState("");
    const [noteContent, setNoteContent] = useState("");

    const handleExport = async () => {
        const response = await fetch("https://localhost:7283/Note/create-pdf", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({ Content: noteContent })
        });
        if (response.ok) {
            const blob = await response.blob();
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement("a");
            a.href = url;
            a.download = `${noteName}.pdf`;
            a.click();
        } else {
            alert("Error exporting PDF.");
        }
    };

    if (!show) return null;

    return (
        <div className="note">
            <h1>New Note</h1>
            <input
                className="note-name"
                placeholder="Name"
                value={noteName}
                onChange={e => setNoteName(e.target.value)}
            />
            <textarea
                className="note-content no-resize"
                placeholder="Content"
                value={noteContent}
                onChange={e => setNoteContent(e.target.value)}
            />
            <div className="button-group1">
                <button className="modify-note" onClick={handleExport}>Export</button>
                <button className="modify-note" onClick={onClose}>Cancel</button>
            </div>
        </div>
    );
}

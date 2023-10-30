import React, { useState } from "react";
import "./Note.css";

export const Note = ({ show, onClose, topicId }) => {
    const [noteName, setNoteName] = useState("");
    const [noteContent, setNoteContent] = useState("");
    const [showDialog, setShowDialog] = useState(false);
    const [savedNotes, setSavedNotes] = useState([]);

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

    const handleShowDialog = () => {
        setShowDialog(true);
    }

    const handleCloseDialog = () => {
        setShowDialog(false);
    };

    const handleSave = () => {
        setSavedNotes(prevNotes => [...prevNotes, { name: noteName, content: noteContent }]);
        setNoteName("");  // Clear current note name
        setNoteContent("");  // Clear current note content
    };

    const handleSelectNote = (selectedNote) => {
        setNoteName(selectedNote.name);
        setNoteContent(selectedNote.content);
        setShowDialog(false);  // Close the dialog after selecting a note
    };

    if (!show) return null;

    return (
        <div className="note">
            <div className="header2">
                <h1>New Note</h1>
                <button className="modify-note" onClick={handleShowDialog}>Saved Notes</button>
            </div>
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
                <button className="modify-note" onClick={handleSave}>Save</button>
                <button className="modify-note" onClick={handleExport}>Export</button>
                <button className="modify-note" onClick={onClose}>Cancel</button>
            </div>

            {showDialog &&
                <div className="dialog">
                    <ul className="notes-list">
                        {savedNotes.length > 0 ? (
                            savedNotes.map((note, index) => (
                                <li key={index} onClick={() => handleSelectNote(note)}>
                                    {note.name}
                                </li>
                            ))
                        ) : (
                            <li className="notes-empty">No saved notes</li>
                        )}
                    </ul>
                    <div className="dialog-footer">
                        <button className="close-note" onClick={handleCloseDialog}>Close</button>
                    </div>
                </div>}
        </div>
    );
}
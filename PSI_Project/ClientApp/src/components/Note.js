import React, { useState } from "react";
import "./Note.css";

export const Note = ({ show, onClose, topicId }) => {
    const [noteName, setNoteName] = useState("");
    const [noteContent, setNoteContent] = useState("");
    const [showDialog, setShowDialog] = useState(false);
    const [savedNotes, setSavedNotes] = useState([]);

    const saveNoteToServer = async (note) => {
        try {
            const response = await fetch("https://localhost:7283/Note", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(note)
            });

            if (!response.ok) throw new Error("Error saving note");
            const data = await response.json();
            return data;
        } catch (error) {
            console.error("Failed to save the note:", error);
        }
    };

    const loadNotesFromServer = async () => {
        try {
            const response = await fetch("https://localhost:7283/Note");
            if (!response.ok) throw new Error("Error loading notes");
            const data = await response.json();
            return data;
        } catch (error) {
            console.error("Failed to load notes:", error);
        }
    };

    const handleExport = async () => {
        const response = await fetch("https://localhost:7283/Note/create-pdf", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({ Content: noteContent, Name: noteName })
        });
        if (!response.ok) {
            console.log("Response Text:", await response.text());
            return;
        }
        const blob = await response.blob();
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement("a");
        a.href = url;
        a.download = `${noteName}.pdf`;
        a.click();
    };

    const handleUpload = async () => {
        try {
            const exportResponse = await fetch("https://localhost:7283/Note/create-pdf", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({ Content: noteContent, Name: noteName })
            });

            if (!exportResponse.ok) {
                throw new Error("Error creating PDF");
            }
            const blob = await exportResponse.blob();

            const formData = new FormData();
            formData.append('files', blob, `${noteName}.pdf`);

            const uploadResponse = await fetch(`https://localhost:7283/Conspectus/upload/${topicId}`, {
                method: 'POST',
                body: formData
            });

            if (!uploadResponse.ok) {
                throw new Error('Error uploading PDF');
            }
            const data = await uploadResponse.json();
            console.log('PDF uploaded successfully:', data);
            window.location.reload();
        } catch (error) {
            console.error('Failed to create or upload PDF:', error);
        }
    };

    const handleSave = async () => {
        const newNote = { name: noteName, content: noteContent };
        const savedNote = await saveNoteToServer(newNote);
        if (savedNote) {
            setSavedNotes(prevNotes => [...prevNotes, savedNote]);
            setNoteName("");  // Clear current note name
            setNoteContent("");  // Clear current note content
        }
    };

    const handleShowDialog = async () => {
        const notesFromServer = await loadNotesFromServer();
        if (notesFromServer) setSavedNotes(notesFromServer);
        setShowDialog(true);
    };

    const handleCloseDialog = () => {
        setShowDialog(false);
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
                <button className="modify-note" onClick={handleUpload}>Upload</button>
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
namespace PSI_Project.Exceptions;

public class NoteCreationException : Exception
{
    public NoteCreationException() { }

    public NoteCreationException(string message) : base(message) { }

    public NoteCreationException(string message, Exception inner) : base(message, inner) { }
}

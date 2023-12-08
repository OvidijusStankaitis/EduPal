namespace PSI_Project.Exceptions;

public class EntityCreationException : Exception
{
    public EntityCreationException(string message) : base(message)
    {
    }

    public EntityCreationException(string message, Exception inner) : base(message, inner)
    {
    }
}
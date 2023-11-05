namespace PSI_Project.Exceptions;

public class EntityDeletionException : Exception
{
    public EntityDeletionException()
    {
    }

    public EntityDeletionException(string message) : base(message)
    {
    }

    public EntityDeletionException(string message, Exception inner) : base(message, inner)
    {
    }
}
namespace PSI_Project.Exceptions;

public class EntityDeletionException : Exception
{
    public EntityDeletionException(string message, Exception inner) : base(message, inner)
    {
    }
}
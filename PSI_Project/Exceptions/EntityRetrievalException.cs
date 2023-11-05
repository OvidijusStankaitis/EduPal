namespace PSI_Project.Exceptions;

public class EntityRetrievalException : Exception
{
    public EntityRetrievalException()
    {
    }

    public EntityRetrievalException(string message) : base(message)
    {
    }

    public EntityRetrievalException(string message, Exception inner) : base(message, inner)
    {
    }
}
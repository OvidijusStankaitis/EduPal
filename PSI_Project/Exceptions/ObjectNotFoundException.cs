namespace PSI_Project.Exceptions;

public class ObjectNotFoundException : Exception
{
    public ObjectNotFoundException(string message) : base(message)
    { }
    public ObjectNotFoundException()
    { }
}
namespace PSI_Project;

public class IdGenerator
{
    private int _currId = -1;   // TODO: IdGenerator isn't working correctly - resolve

    public string GenerateId()
    {
        IncrementId();
        return (_currId).ToString();
    }

    public void IncrementId()
    {
        _currId += 1;
    }
}
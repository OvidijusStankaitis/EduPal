namespace PSI_Project;

public class IdGenerator
{
    private int _currId = -1;   // TODO: IdGenerator isn't working correctly - resolve

    public string GenerateId()
    {
        IncrementId(_currId.ToString());
        return _currId.ToString();
    }

    public void IncrementId(string id)
    {
        _currId = int.Max(int.Parse(id), _currId) + 1;
    }
}
namespace PSI_Project;

public class IdGenerator
{
    public static string CurrId = "-1";

    public string GenerateId()
    {
        IncrementId();
        return CurrId;
    }

    public void IncrementId()
    {
        CurrId = (Int32.Parse(CurrId) + 1).ToString();
    }
}
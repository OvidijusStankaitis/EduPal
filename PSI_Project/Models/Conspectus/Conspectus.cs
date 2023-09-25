using Microsoft.VisualBasic.CompilerServices;

namespace PSI_Project;

public class Conspectus
{
    public static string NextId = "0"; // TODO: delete line when database is added
    public string Id;
    public string Path { get; set; }
    
    public Conspectus(string id, string path)
    {
        Id = id;
        Path = path;
    }
    public Conspectus(string path)
    {
        Id = NextId;
        NextId = (Int32.Parse(NextId) + 1).ToString();
        
        Path = path;
    }
}
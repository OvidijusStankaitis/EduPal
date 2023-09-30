using Microsoft.VisualBasic.CompilerServices;
using PSI_Project.DAL;

namespace PSI_Project;

public class Conspectus : IStorable
{
    private IdGenerator _idGenerator = new IdGenerator();
    public string Id{ get; }
    public string Path { get; set; }
    
    public Conspectus(string id, string path)
    {
        Id = id;
        _idGenerator.IncrementId();
        
        Path = path;
    }
    public Conspectus(string path)
    {
        Id = _idGenerator.GenerateId();
        Path = path;
    }
}
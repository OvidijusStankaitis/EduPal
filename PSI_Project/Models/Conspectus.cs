using Microsoft.VisualBasic.CompilerServices;
using PSI_Project.DAL;

namespace PSI_Project;

public class Conspectus : IStorable
{
    private static IdGenerator _idGenerator = new IdGenerator();
    public string Id{ get; }
    public string TopicId;
    public string Path { get; set; }
    
    public Conspectus(string id, string topicId, string path)
    {
        Id = id;
        _idGenerator.IncrementId();
        TopicId = topicId;
        Path = path;
    }
    public Conspectus(string topicId, string path)
    {
        Id = _idGenerator.GenerateId();
        TopicId = topicId;
        Path = path;
    }
}
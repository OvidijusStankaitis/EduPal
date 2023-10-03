using Microsoft.VisualBasic.CompilerServices;
using PSI_Project.DAL;

namespace PSI_Project;

public class Conspectus : BaseEntity, IStorable
{
    private static IdGenerator _idGenerator = new IdGenerator();
    public string Id{ get; }
    public string TopicId;
    public string TopicName;
    public string Path { get; set; }
    
    public Conspectus(string id, string topicName, string path)
    {
        Id = id;
        _idGenerator.IncrementId(id);
        TopicName = topicName;
        Path = path;
    }
    public Conspectus(string topicName, string path)
    {
        Id = _idGenerator.GenerateId();
        TopicName = topicName;
        Path = path;
    }
}
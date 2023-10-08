namespace PSI_Project.Models;

public class Conspectus : BaseEntity
{
    public string TopicId {get; }
    public string Path { get; }

    public Conspectus(string topicId, string path)
    {
        TopicId = topicId;
        Path = path;
    }
}

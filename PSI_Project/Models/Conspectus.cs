namespace PSI_Project.Models;

public class Conspectus : BaseEntity
{
    public string TopicId { get; set; }
    public string Path { get; set; }

    public Conspectus(string topicId, string path)
    {
        TopicId = topicId;
        Path = path;
    }
}

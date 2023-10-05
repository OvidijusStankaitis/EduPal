namespace PSI_Project.Models;

public class Conspectus : BaseEntity
{
    public string TopicId { get; set; }
    public string TopicName { get; set; }
    public string Path { get; set; }

    public Conspectus(string topicName, string path)
    {
        TopicName = topicName;
        Path = path;
    }
}

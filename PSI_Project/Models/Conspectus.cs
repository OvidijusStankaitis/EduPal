namespace PSI_Project.Models;

public class Conspectus : BaseEntity
{
    public string TopicId { get; }
    public string Path { get; }
    public string Name { get; } // full name of a .pdf file

    public Conspectus(string topicId, string path, string name) : base()
    {
        TopicId = topicId;
        Path = path;
        Name = name;
    }
}

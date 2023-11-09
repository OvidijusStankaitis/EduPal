namespace PSI_Project.Models;

public class Note : BaseEntity
{
    public string Name { get; set; }
    public string Content { get; set; }

    public Note(string name, string content)
    {
        Name = name;
        Content = content;
    }
}
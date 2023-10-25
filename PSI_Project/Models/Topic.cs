namespace PSI_Project.Models;

public class Topic : BaseEntity
{   
    public string Name { get; set; }
    public KnowledgeLevel KnowledgeRating { get; set; }
    public Subject Subject { get; init; }

    public Topic()
    {
    }

    public Topic(string name, Subject subject, KnowledgeLevel knowledgeRating = KnowledgeLevel.Poor)
    {
        Subject = subject;
        KnowledgeRating = knowledgeRating;
        Name = name;
    }
}
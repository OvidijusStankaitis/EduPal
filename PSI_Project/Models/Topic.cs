namespace PSI_Project.Models;

public class Topic : BaseEntity
{
    public string SubjectId { get; set; }
    public KnowledgeLevel KnowledgeRating { get; set; } // 1: using enum
    public String Name { get; set; }

    public Topic(string name, string subjectId, KnowledgeLevel knowledgeRating = KnowledgeLevel.Poor)  // 3: using optional arguments
    {
        SubjectId = subjectId;
        KnowledgeRating = knowledgeRating;
        Name = name;
    }
}
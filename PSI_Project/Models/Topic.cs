namespace PSI_Project.Models;

public class Topic : BaseEntity
{
    public string SubjectId { get; set; }
    public KnowledgeLevel KnowledgeRating { get; set; } // 1: using enum

    public Topic(string name, string subjectId, KnowledgeLevel knowledgeRating = KnowledgeLevel.Poor) : base(name)  // 3: using optional arguments
    {
        SubjectId = subjectId;
        KnowledgeRating = knowledgeRating;
    }
}
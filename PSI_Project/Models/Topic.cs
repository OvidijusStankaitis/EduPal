using System.ComponentModel.DataAnnotations.Schema;

namespace PSI_Project.Models;

public class Topic : BaseEntity
{   
    public String Name { get; set; }
    public KnowledgeLevel KnowledgeRating { get; set; }
    
    public Subject Subject { get; init; }
    
    public Topic(string name, Subject subject, KnowledgeLevel knowledgeRating = KnowledgeLevel.Poor)
    {
        Subject = subject;
        KnowledgeRating = knowledgeRating;
        Name = name;
    }
}
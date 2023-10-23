using System.ComponentModel.DataAnnotations.Schema;

namespace PSI_Project.Models;

public class Topic : BaseEntity
{   
    public String Name { get; set; }
    public KnowledgeLevel KnowledgeRating { get; set; } // 1: using enum
    
    public Subject Subject { get; init; }



    // public Topic(string name, string subjectId, KnowledgeLevel knowledgeRating = KnowledgeLevel.Poor)  // 3: using optional arguments
    // {
    //     SubjectId = subjectId;
    //     KnowledgeRating = knowledgeRating;
    //     Name = name;
    // }
}
namespace PSI_Project.Models;

public class Topic : BaseEntity
{
    public string SubjectId { get; set; }

    public Topic(string name, string subjectId) : base(name)
    {
        SubjectId = subjectId;
    }
}
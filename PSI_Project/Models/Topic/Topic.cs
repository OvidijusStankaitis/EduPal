namespace PSI_Project;

public class Topic : BaseEntity
{
    public string SubjectName { get; set; } //the subject to which the topic is assigned
    public Topic(string topicName, string topicDescription, string subjectName) : base(topicName, topicDescription)
    {
        SubjectName = subjectName;
    }
}
namespace PSI_Project;

public class Topic
{
    public Topic(string topicName, string topicDescription, string subjectName)
    {
        Name = topicName;
        Description = topicDescription;
        SubjectName = subjectName;
    }
    public string Name {get; set;}
    public string Description {get; set;}
    public string SubjectName { get; set; } //the subject to which the topic is assigned
    
}
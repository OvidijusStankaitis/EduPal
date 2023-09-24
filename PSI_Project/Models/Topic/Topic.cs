namespace PSI_Project;

public class Topic
{
    public Topic(string topicName, string topicDescription, Subject subject)
    {
        Name = topicName;
        Description = topicDescription;
        Subject = subject;
    }
    public string Name {get; set;}
    public string Description {get; set;}
    public Subject Subject { get; set; } //the subject to which the topic is assigned
    
}
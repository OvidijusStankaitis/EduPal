using PSI_Project.DAL;

namespace PSI_Project;

public class Topic : BaseEntity, IStorable
{
    private IdGenerator _idGenerator = new IdGenerator();
    public string Id{ get; }
    public string SubjectName { get; set; } //the subject to which the topic is assigned
    public Topic(string id, string topicName, string topicDescription, string subjectName) : base(topicName, topicDescription)
    {
        Id = id;
        _idGenerator.IncrementId();
     
        SubjectName = subjectName;
    }
}
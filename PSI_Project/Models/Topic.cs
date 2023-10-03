using PSI_Project.DAL;

namespace PSI_Project;

public class Topic : BaseEntity, IStorable
{
    private static IdGenerator _idGenerator = new IdGenerator();
    public string Id{ get; }
    
    public string SubjectId;
    public string SubjectName;
    
    public Topic(string id, string subjectName, string name, string description) : base(name, description)
    {
        Id = id;
        _idGenerator.IncrementId(id);
        SubjectName = subjectName;
    }
    
    public Topic(string name, string subjectName, string description) : base(name, description)
    {
        Id = _idGenerator.GenerateId();
        SubjectName = subjectName;
    }
    
}
using PSI_Project.DAL;

namespace PSI_Project;

public class Topic : BaseEntity, IStorable
{
    private static IdGenerator _idGenerator = new IdGenerator();
    public string Id{ get; }
    public string Name;
    public string? Description;
    
    public string SubjectId;
    
    public Topic(string id, string subjectId, string name) : base(name)
    {
        Id = id;
        _idGenerator.IncrementId(id);
        
        SubjectId = subjectId;
    }
}
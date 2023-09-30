using PSI_Project.DAL;

namespace PSI_Project;

public class Subject : BaseEntity, IStorable, IComparable<Subject>, IEquatable<Subject>
{
    private IdGenerator _idGenerator = new IdGenerator();
    public string Id{ get; }

    public Subject(string id, string subjectName, string subjectDescription) : base(subjectName, subjectDescription)
    {
        Id = id;
        _idGenerator.IncrementId();
    }

    public int CompareTo(Subject other)
    {
        return Name.CompareTo(other.Name);
    }

    public bool Equals(Subject other)
    {
        if (other == null)
            return false;

        return Name == other.Name && Description == other.Description;
    }

    public override bool Equals(object obj)
    {
        if (obj is Subject)
            return Equals((Subject)obj);

        return false;
    }
}
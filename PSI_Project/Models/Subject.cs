namespace PSI_Project;

public class Subject : BaseEntity, IComparable<Subject>, IEquatable<Subject>
{
    public Subject(string subjectName, string subjectDescription) : base(subjectName, subjectDescription)
    {
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
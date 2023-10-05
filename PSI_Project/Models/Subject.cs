using PSI_Project.DAL;

namespace PSI_Project
{
    public class Subject : BaseEntity, IStorable, IComparable<Subject>, IEquatable<Subject>
    {
        public Subject(string name) : base(name)
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

            return Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            if (obj is Subject)
                return Equals((Subject)obj);

            return false;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
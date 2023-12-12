namespace PSI_Project.Models;

public class Conspectus : BaseEntity, IComparable<Conspectus>
{ 
    public string Name { get; init;  }
    public string Path { get; init; }
    public int Rating { get; set; }
    public Topic Topic { get; init; }

    public int CompareTo(Conspectus? other)
    {
        if (other == null)
        {
            return 0;
        }

        return Rating == other.Rating 
            ? String.Compare(Name, other.Name, StringComparison.Ordinal)
            : other.Rating - Rating;
    }
}

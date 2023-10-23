namespace PSI_Project.Models;

public class Subject : BaseEntity
{
    public string Name { get; init; }
    
    public Subject(string name)
    {
        Name = name;
    }
}
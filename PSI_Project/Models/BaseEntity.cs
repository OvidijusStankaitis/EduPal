namespace PSI_Project;

public class BaseEntity
{
    public string Name {get; set;}
    public string? Description {get; set;}

    public BaseEntity()
    {
        
    }
    public BaseEntity(string name)
    {
        Name = name;
    }
    public BaseEntity(string name, string description)
    {
        Name = name;
        Description = description;
    }
}
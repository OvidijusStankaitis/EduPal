namespace PSI_Project;

public class BaseEntity
{
    public string Name {get; set;}
    public string Description {get; set;}


    public BaseEntity(string name, string description)
    {
        Name = name;
        Description = description;
    }
}
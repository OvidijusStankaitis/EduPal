namespace PSI_Project.Models;

// 1: using our own class
public class BaseEntity
{
    public string Id { get; init; } // 2: property usage in class (get, init, set)
    public string Name { get; set; }
    
    public BaseEntity(string name)
    {
        Id = Guid.NewGuid().ToString();
        Name = name;
    }
}

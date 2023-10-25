using System.ComponentModel.DataAnnotations.Schema;

namespace PSI_Project.Models;

public class Conspectus : BaseEntity
{ 
    public string Name { get; init;  }
    public string Path { get; init; }
    public int Rating { get; set; }

    public Topic Topic { get; init; }
}

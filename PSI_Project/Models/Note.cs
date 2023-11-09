using System.ComponentModel.DataAnnotations;

namespace PSI_Project.Models;

public class Note : BaseEntity
{
    [Key]
    public string Id { get; } = Guid.NewGuid().ToString();
    public string Name { get; set; }
    public string Content { get; set; }
}
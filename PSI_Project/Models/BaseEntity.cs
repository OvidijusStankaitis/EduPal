using System.ComponentModel.DataAnnotations;

namespace PSI_Project.Models;

public class BaseEntity
{
    [Key]
    public string Id { get; init; } = Guid.NewGuid().ToString();
}

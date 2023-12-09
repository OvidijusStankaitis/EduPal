namespace PSI_Project.Interceptor;

public class SaveChangesAudit
{
    public int Id { get; set; }
    public Guid AuditId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool Succeeded { get; set; }
    public string ErrorMessage { get; set; }

    public ICollection<EntityAudit> Entities { get; } = new List<EntityAudit>();
}
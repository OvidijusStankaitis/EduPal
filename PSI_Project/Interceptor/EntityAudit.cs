using Microsoft.EntityFrameworkCore;
namespace PSI_Project.Interceptor;
public class EntityAudit
{
    public EntityState State { get; set; }
    public string AuditMessage { get; set; }
}
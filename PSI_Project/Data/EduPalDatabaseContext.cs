using Microsoft.EntityFrameworkCore;
using PSI_Project.Controllers;
using PSI_Project.Models;

namespace PSI_Project.Data;

public class EduPalDatabaseContext : DbContext
{
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<Topic?> Topics { get; set; }
    public DbSet<Conspectus> Conspectuses { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Goal> Goals { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<User> Users { get; set; }

    public EduPalDatabaseContext(DbContextOptions options) : base(options)
    {
        
    }
    
}
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PSI_Project.Data;
using PSI_Project.Models;

namespace PSI_Project.Repositories;
public class SubjectRepository : Repository<Subject>
{
    public EduPalDatabaseContext EduPalContext => Context as EduPalDatabaseContext;
    
    public SubjectRepository(EduPalDatabaseContext context) : base(context)
    {
    }
    
    public async Task<Subject?> GetAsync(string subjectId)
    {
        return await EduPalContext.Subjects.FindAsync(subjectId);
    }

    public async Task<List<Subject>> GetSubjectsListAsync()
    {
        return await EduPalContext.Subjects.ToListAsync();
    }

    public async Task<Subject?> CreateSubjectAsync(JsonElement request)
    {
        if (!request.TryGetProperty("subjectName", out var subjectNameProperty))
            return null;
        
        string? subjectName = subjectNameProperty.GetString();
        if (!subjectName.IsValidContainerName())
            return null;
        
        Subject newSubject = new Subject(subjectName);
        Add(newSubject);
        int changes = await EduPalContext.SaveChangesAsync();
       
        return changes > 0 ? newSubject : null;
    }
    
    public bool RemoveSubject(string subjectId)
    {
        Subject? subject = Get(subjectId);
        if (subject is null)
            return false;
        
        Remove(subject);
        int changes = EduPalContext.SaveChanges();

        return changes > 0;
    } 
}

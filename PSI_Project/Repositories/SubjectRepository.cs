using System.Text.Json;
using PSI_Project.Data;
using PSI_Project.Models;

namespace PSI_Project.Repositories;
public class SubjectRepository : Repository<Subject>
{
    public EduPalDatabaseContext EduPalContext => Context as EduPalDatabaseContext;
    
    public SubjectRepository(EduPalDatabaseContext context) : base(context)
    {
    }

    public List<Subject> GetSubjectsList()
    {
        return EduPalContext.Subjects.ToList();
    }

    public Subject? CreateSubject(string subjectName)
    {
        if (!subjectName.IsValidContainerName())
            return null;

        Subject newSubject = new Subject(subjectName);
        Add(newSubject);
        int changes = EduPalContext.SaveChanges();

        return changes > 0 ? newSubject : null;
    }
    
    public bool RemoveSubject(string subjectId)
    {
        Subject subject = Get(subjectId);
        Remove(subject);
        
        int changes = EduPalContext.SaveChanges();
        return changes > 0;
    } 
}

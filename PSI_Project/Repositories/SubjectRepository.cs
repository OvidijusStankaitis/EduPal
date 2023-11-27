using PSI_Project.Data;
using PSI_Project.Models;
using Microsoft.EntityFrameworkCore;

namespace PSI_Project.Repositories
{
    public class SubjectRepository : Repository<Subject>
    {
        public EduPalDatabaseContext EduPalContext => Context as EduPalDatabaseContext;
        
        public SubjectRepository(EduPalDatabaseContext context) : base(context)
        {
        }

        public List<Subject> GetSubjectsList()
        {
            //return await EduPalContext.Subjects.ToListAsync(); //need to be changed for tests
            return GetAll().ToList();
        }

        public Subject? CreateSubject(string subjectName)
        {
            if (!subjectName.IsValidContainerName())
                return null;

            Subject newSubject = new Subject(subjectName);
            int changes = Add(newSubject);
            //int changes = await EduPalContext.SaveChangesAsync();

            return changes > 0 ? newSubject : null;
        }

        public async Task<bool> RemoveSubjectAsync(string subjectId)
        {
            Subject subject = await GetAsync(subjectId);
            int changes = Remove(subject);
            //int changes = await EduPalContext.SaveChangesAsync();
          
            return changes > 0;
        }
    }
}

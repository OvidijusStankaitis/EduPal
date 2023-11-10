using PSI_Project.Models;
using PSI_Project.Data;

namespace PSI_Project.Repositories;

public class NoteRepository : Repository<Note>
{
    public readonly EduPalDatabaseContext EduPalContext;

    public NoteRepository(EduPalDatabaseContext context) : base(context)
    {
        EduPalContext = context;
    }

    public Note Add(Note note)
    {
        EduPalContext.Notes.Add(note);
        EduPalContext.SaveChanges();
        return note;
    }

    public Note GetById(int id)
    {
        return EduPalContext.Notes.Find(id);
    }

    public IQueryable<Note> GetAll()
    {
        return EduPalContext.Notes;
    }
}
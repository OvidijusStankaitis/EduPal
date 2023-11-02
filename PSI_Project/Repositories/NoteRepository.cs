using PSI_Project.Models;
using PSI_Project.Data;

namespace PSI_Project.Repositories;

public class NoteRepository
{
    private readonly EduPalDatabaseContext _context;

    public NoteRepository(EduPalDatabaseContext context)
    {
        _context = context;
    }

    public Note Add(Note note)
    {
        _context.Notes.Add(note);
        _context.SaveChanges();
        return note;
    }

    public Note GetById(int id)
    {
        return _context.Notes.Find(id);
    }

    public IQueryable<Note> GetAll()
    {
        return _context.Notes;
    }
}
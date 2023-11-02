using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using PSI_Project.Models;
using PSI_Project.Repositories;
using PSI_Project.Services;
using PSI_Project.Exceptions;

namespace PSI_Project.Controllers;

[ApiController]
[Route("[controller]")]
public class NoteController : ControllerBase
{
    private readonly NoteRepository _noteRepository;
    private readonly NoteService _noteService;

    public NoteController(NoteRepository noteRepository, NoteService noteService)  // Dependency injection
    {
        _noteService = noteService;
        _noteRepository = noteRepository;
    }

    [HttpGet("{id}")]
    public IActionResult GetNoteById(int id)
    {
        var note = _noteRepository.GetById(id);
        if (note == null) return NotFound();
        return Ok(note);
    }

    [HttpGet]
    public IActionResult GetAllNotes()
    {
        var notes = _noteRepository.GetAll();
        if (notes == null || !notes.Any()) return NotFound();
        return Ok(notes);
    }
    
    [HttpPost]
    public IActionResult AddNote([FromBody] Note note)
    {
        var savedNote = _noteRepository.Add(note);
        return CreatedAtAction(nameof(GetNoteById), new { id = savedNote.Id.ToString() }, savedNote);
    }

    [HttpPost("create-pdf")]
    public async Task<IActionResult> CreatePdfAsync([FromBody] Note note)
    {
        try
        {
            var pdfStream = await _noteService.CreatePdfAsync(note);
            var contentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = $"{note.Name}.pdf"
            };
            Response.Headers[HeaderNames.ContentDisposition] = contentDisposition.ToString();
            Console.WriteLine("Successfully created PDF");
            return File(pdfStream, "application/pdf");
        }
        catch (NoteCreationException ex)
        {
            Console.WriteLine($"NoteCreationException: {ex.Message}, StackTrace: {ex.StackTrace}");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}, StackTrace: {ex.StackTrace}");
            return StatusCode(500, "An error occurred while creating the PDF.");
        }
    }
}

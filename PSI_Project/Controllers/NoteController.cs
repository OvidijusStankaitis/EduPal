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
    private readonly ILogger<NoteController> _logger;

    public NoteController(ILogger<NoteController> logger, NoteRepository noteRepository,
        NoteService noteService) // Dependency injection
    {
        _logger = logger;
        _noteService = noteService;
        _noteRepository = noteRepository;
    }

    [HttpGet("{id}")]
    public IActionResult GetNoteById(int id)
    {
        try
        {
            var note = _noteRepository.GetById(id);
            return Ok(note);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Couldn't get note {noteId}", id);
            return StatusCode(500, "An error occured while getting note");
        }
    }

    [HttpGet]
    public IActionResult GetAllNotes()
    {
        try
        {
            var notes = _noteRepository.GetAll();
            return Ok(notes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Couldn't list notes");
            return StatusCode(500, "An error occured while listing notes");
        }
    }

    [HttpPost]
    public IActionResult AddNote([FromBody] Note note)
    {
        try
        {
            var savedNote = _noteRepository.Add(new Note(note.Name, note.Content));
            return CreatedAtAction(nameof(GetNoteById), new { id = savedNote.Id.ToString() }, savedNote);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Couldn't add note");
        }

        return StatusCode(500, "An error occured while creating new note");
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
        catch (EntityCreationException ex)
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
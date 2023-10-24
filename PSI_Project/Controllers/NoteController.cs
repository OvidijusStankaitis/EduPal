using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using PSI_Project.Models;
using PSI_Project.Services;
using PSI_Project.Exceptions;

namespace PSI_Project.Controllers;

[ApiController]
[Route("[controller]")]
public class NoteController : ControllerBase
{
    private readonly NoteService _noteService;

    public NoteController()
    {
        _noteService = new NoteService();
    }

    [HttpPost("create-pdf")]
    public async Task<IActionResult> CreatePdfAsync([FromBody] Note note)
    {
        try
        {
            var pdfStream = await _noteService.CreatePdfAsync(note);
            var contentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = $"{note.Content}.pdf"
            };
            Response.Headers[HeaderNames.ContentDisposition] = contentDisposition.ToString();
            return File(pdfStream, "application/pdf");
        }
        catch (NoteCreationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"NoteCreationException: {ex.Message}");
            return StatusCode(500, "An error occurred while creating the PDF.");
        }
    }

}

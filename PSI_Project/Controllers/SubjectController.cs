using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using PSI_Project.Models;
using PSI_Project.Repositories;

namespace PSI_Project.Controllers;

[ApiController]
[Route("[controller]")]
public class SubjectController : ControllerBase
{
    private readonly SubjectRepository _subjectRepository = new SubjectRepository();

    [HttpGet("list")]
    public IActionResult ListSubjects()
    {
        return Ok(_subjectRepository.Items);
    }
    
    [HttpPost("upload")]
    public IActionResult UploadSubject([FromBody] JsonElement request)
    {
        if (request.TryGetProperty("subjectName", out var subjectNameProperty))
        {
            string subjectName = subjectNameProperty.GetString();
            _subjectRepository.InsertItem(new Subject(subjectName));
            return Ok(_subjectRepository.Items);
        }
        return BadRequest("Invalid request body");
    }
    
    [HttpDelete("{subjectId}/delete")]
    public IActionResult RemoveSubject(string subjectId)
    { 
        return _subjectRepository.RemoveItemById(subjectId) 
            ? Ok("Subject has been successfully deleted") 
            : BadRequest("An error occured while deleting the subject");
    }
}
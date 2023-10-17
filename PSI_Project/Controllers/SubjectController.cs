using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using PSI_Project.Models;
using PSI_Project.Repositories;

namespace PSI_Project.Controllers;

[ApiController]
[Route("[controller]")]
public class SubjectController : ControllerBase
{
    private readonly SubjectRepository _subjectRepository;
    public SubjectController(SubjectRepository subjectRepository)
    {
        _subjectRepository = subjectRepository;
    }

    [HttpGet("get/{subjectId}")]
    public IActionResult GetSubject(string subjectId)
    {
        Subject? subject = _subjectRepository.GetItemById(subjectId);
        return subject == null
            ? NotFound(new { error = "Subject not found." })
            : Ok(subject);
    }
    
    [HttpGet("list")]
    public IActionResult ListSubjects()
    {
        return Ok(_subjectRepository.GetSubjectList());
    }
    
    [HttpPost("upload")]
    public IActionResult UploadSubject([FromBody] JsonElement request)
    {
        Subject? addedSubject = _subjectRepository.CreateSubject(request);
        return addedSubject == null
            ? BadRequest("Invalid request body")
            : Ok(addedSubject);
    }
    
    [HttpDelete("{subjectId}/delete")]
    public IActionResult RemoveSubject(string subjectId)
    { 
        return _subjectRepository.RemoveItemById(subjectId) 
            ? Ok("Subject has been successfully deleted") 
            : BadRequest("An error occured while deleting the subject");
    }
}
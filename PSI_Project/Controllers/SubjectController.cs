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
    public async Task<IActionResult> GetSubjectAsync(string subjectId)
    {
        Subject? subject = await _subjectRepository.GetAsync(subjectId);
        return subject == null
            ? NotFound(new { error = "Subject not found." })
            : Ok(subject);
    }
    
    [HttpGet("list")]
    public async Task<IActionResult> ListSubjectsAsync()
    {
        return Ok(await _subjectRepository.GetSubjectsListAsync());
    }
    
    [HttpPost("upload")]
    public async Task<IActionResult> UploadSubjectAsync([FromBody] JsonElement request)
    {
        Subject? addedSubject = await _subjectRepository.CreateSubjectAsync(request);
        return addedSubject == null
            ? BadRequest("Invalid request body")
            : Ok(addedSubject);
    }
    
    [HttpDelete("{subjectId}/delete")]
    public IActionResult RemoveSubject(string subjectId)
    { 
        return _subjectRepository.RemoveSubject(subjectId) 
            ? Ok("Subject has been successfully deleted") 
            : BadRequest("An error occured while deleting the subject");
    }
}

using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
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
    
    [HttpDelete("{subjectName}/delete")]
    public void RemoveSubject(string subjectName)
    {
        Subject? subjectToRemove = _subjectRepository.GetItemByName(subjectName);
        if (subjectToRemove != null)
        {
            _subjectRepository.RemoveItem(subjectToRemove.Id);
        }
    }
}
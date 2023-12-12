using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PSI_Project.DTO;
using PSI_Project.Exceptions;
using PSI_Project.Models;
using PSI_Project.Repositories;

namespace PSI_Project.Controllers;

[ApiController]
[Route("[controller]")]
public class SubjectController : ControllerBase
{
    private readonly SubjectRepository _subjectRepository;
    private readonly ILogger<SubjectController> _logger;

    public SubjectController(ILogger<SubjectController> logger, SubjectRepository subjectRepository)
    {
        _logger = logger;
        _subjectRepository = subjectRepository;
    }

    [Authorize]
    [HttpGet("get/{subjectId}")]
    public IActionResult GetSubject(string subjectId)
    {
        try
        {
            Subject subject = _subjectRepository.Get(subjectId);
            return Ok(subject);
        }
        catch (ObjectNotFoundException)
        {
            _logger.LogWarning("Tried to reach unavailable / non-existing subject {subjectId}", subjectId);
            return NotFound("There is no subject with such id");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Couldn't get subject {subjectId} information", subjectId);
            return BadRequest("An error occured while getting the subject");
        }
    }

    [Authorize]
    [HttpGet("list")]
    public IActionResult ListSubjects()
    {
        try
        {
            var subjects =  _subjectRepository.GetSubjectsList();
            return Ok(subjects);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Couldn't list subjects");
            return BadRequest("An error occurred while listing the subjects");
        }
    }

    [Authorize]
    [HttpPost("upload")]
    public IActionResult UploadSubjectAsync([FromBody] SubjectRequestDTO request)
    {
        try
        {
            Subject? addedSubject = _subjectRepository.CreateSubject(request.SubjectName);
            if (addedSubject != null)
            {
                return Ok(addedSubject);
            }

            return BadRequest("Invalid subject name");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Couldn't add new subject");
            return BadRequest("An error occurred while uploading the subject");
        }
    }

    [Authorize]
    [HttpDelete("delete/{subjectId}")]
    public async Task<IActionResult> RemoveSubjectAsync(string subjectId)
    {
        try
        {
            bool removed = await _subjectRepository.RemoveSubjectAsync(subjectId);
            if (removed)
            {
                return Ok("Subject has been successfully deleted");
            }

            return NotFound("Subject not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Couldn't delete subject {subjectId}", subjectId);
            return BadRequest("An error occurred while deleting the subject");
        }
    }
}
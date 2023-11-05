using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using PSI_Project.Exceptions;
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
        try
        {
            Subject subject = _subjectRepository.Get(subjectId);
            return Ok(subject);
        }
        catch (ObjectNotFoundException)
        {
            return NotFound("There is no subject with such id");
        }
        catch (Exception ex)
        {
            // TODO: log errors
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);

            return BadRequest("An error occured while getting the subject");
        }
    }
    
    [HttpGet("list")]
    public IActionResult ListSubjects()
    {
        try
        {
            return Ok(_subjectRepository.GetSubjectsList());
        }
        catch (Exception ex)
        {
            // TODO: log errors
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            
            return BadRequest("An error occured while listing the subjects");
        }
    }
    
    [HttpPost("upload")]
    public IActionResult UploadSubject([FromBody] JsonElement request)
    {
        try
        {
            Subject? addedSubject = _subjectRepository.CreateSubject(request);  // TODO: add validation
            if (addedSubject != null)
            {
                return Ok(addedSubject);
            }

            return BadRequest("Invalid subject name");
        }
        catch (Exception ex)
        {
            // TODO: log errors
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            
            return BadRequest("An error occured while uploading the subject");
        }
    }
    
    [HttpDelete("{subjectId}/delete")]
    public IActionResult RemoveSubject(string subjectId)
    {
        try
        {
            _subjectRepository.RemoveSubject(subjectId);
            return Ok("Subject has been successfully deleted");
        }
        catch (Exception ex)
        {
            // TODO: log errors
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            
            return BadRequest("An error occured while deleting the subject");
        }
    }
}
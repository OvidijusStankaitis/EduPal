using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PSI_Project.HelperFunctions;

namespace PSI_Project.Controllers;

[ApiController]
[Route("[controller]")]
public class SubjectController : ControllerBase
{
    private readonly SubjectHandler _subjectHandler = new SubjectHandler();

    [HttpGet("list")]
    public IActionResult ListSubjects()
    {
        return Ok(_subjectHandler.ItemList);
    }
    
    [HttpPost("upload")]
    public IActionResult UploadSubject([FromBody] JsonElement request)
    {
        if (request.TryGetProperty("subjectName", out var subjectNameProperty) && 
            request.TryGetProperty("subjectDescription", out var subjectDescriptionProperty))
        {
            string subjectName = subjectNameProperty.GetString();
            string subjectDescription = subjectDescriptionProperty.GetString() ?? " ";

            _subjectHandler.InsertItem(new Subject(subjectName, subjectDescription));
            return Ok(_subjectHandler.ItemList);
        }
        return BadRequest("Invalid request body");
    }
    
    [HttpDelete("{subjectName}/delete")] //should "delete/" be here?
    public void RemoveSubject(string subjectName)
    {
        Subject? subjectToRemove = _subjectHandler.CheckItemInList(subjectName);
        if (subjectToRemove != null)
        {
            _subjectHandler.RemoveItem(subjectToRemove.Id);
        }
    }
}

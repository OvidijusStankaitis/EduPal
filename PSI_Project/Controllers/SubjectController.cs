using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace PSI_Project.Controllers;

[ApiController]
[Route("[controller]")]
public class SubjectController : ControllerBase
{
    private readonly SubjectHandler _subjectHandler = new SubjectHandler();
    private readonly TopicHandler _topicHandler = new TopicHandler();

    [HttpGet]
    public IActionResult ListSubjects()
    {
        //TESTS
        // _subjectHandler.CreateSubject("a", "b");
        // _subjectHandler.CreateSubject("a1", "b1");
        // _subjectHandler.CreateSubject("a12", "b12");
        
        return Ok(_subjectHandler.ItemList);
    }
    
    [HttpGet("{subjectName}")]
    public IActionResult ListTopics(string subjectName)
    {
        List<Topic> subjectTopics = new List<Topic>();
        foreach (var topic in _topicHandler.ItemList)
        {
            if (topic.SubjectName == subjectName)
            {
                subjectTopics.Add(topic);
            }
        }
        return Ok(subjectTopics); 
    }
    
    [HttpPost("upload")]
    public IActionResult UploadSubject(string subjectName, string subjectDescription = " ") //how can this method get its parameters?
    {
        _subjectHandler.CreateItem(new Subject(subjectName, subjectDescription));
        return Ok(_subjectHandler.ItemList);
    }
    
    [HttpPost("{oldSubjectName}/modify")] // should the parameter in {} be only string or it can represent different types?
    public IActionResult ModifySubject(string oldSubjectName,string subjectName, string subjectDescription = " ")
    {
        Subject? oldSubject = _subjectHandler.CheckItemInList(oldSubjectName);
        if (oldSubject != null)
        {
            _subjectHandler.ModifyItem(oldSubject, new Subject(subjectName, subjectDescription));
            return Ok(_subjectHandler.ItemList);
        }

        return NotFound();
    }
    
    [HttpDelete("{subjectName}/delete")] //should "delete/" be here?
    public void RemoveSubject(string subjectName)
    {
        Subject? subjectToRemove = _subjectHandler.CheckItemInList(subjectName);
        if (subjectToRemove != null)
        {
            _subjectHandler.RemoveItem(subjectToRemove);
        }
    }
}

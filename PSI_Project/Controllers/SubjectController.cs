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
    public IActionResult UploadSubject(string subjectName, string subjectDescription)
    {
        _subjectHandler.CreateItem(new Subject(subjectName, subjectDescription));
        return Ok(_subjectHandler.ItemList);
    }
    
    [HttpPost("modify")]
    public IActionResult ModifySubject(Subject oldSubject,string subjectName, string subjectDescription) 
    {
        _subjectHandler.ModifyItem(oldSubject, new Subject(subjectName, subjectDescription));
        return Ok(_subjectHandler.ItemList);
    }
    
    [HttpDelete("delete/{subjectName}")] //should "delete/" be here?
    public void RemoveSubject(string subjectName)
    {
        Subject? subjectToRemove = null;
        foreach (var subject in _subjectHandler.ItemList)
        {
            if (subject.Name == subjectName)
            {
                subjectToRemove = subject;
                break;
            }
        }
        if (subjectToRemove != null)
        {
            _subjectHandler.RemoveItem(subjectToRemove);
        }
    }
}

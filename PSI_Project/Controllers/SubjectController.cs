using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace PSI_Project.Controllers;

[ApiController]
[Route("[controller]")]
public class SubjectController : ControllerBase
{
    private readonly SubjectHandler _subjectHandler;
    private readonly TopicHandler _topicHandler;

    public SubjectController()
    {
        _subjectHandler = new SubjectHandler();
        _topicHandler = new TopicHandler();
    }

    [HttpGet]
    public IActionResult ListSubjects()
    {
        //TESTS
        // _subjectHandler.CreateSubject("a", "b");
        // _subjectHandler.CreateSubject("a1", "b1");
        // _subjectHandler.CreateSubject("a12", "b12");
        
        return Ok(_subjectHandler.Items);
    }
    
    [HttpGet("{subjectName}")]
    public IActionResult ListTopics(string subjectName)
    {
        List<Topic> subjectTopics = new List<Topic>();
        foreach (var topic in _topicHandler.Items)
        {
            if (topic.SubjectName == subjectName)
            {
                subjectTopics.Add(topic);
            }
        }
        return Ok(subjectTopics); 
    }
}

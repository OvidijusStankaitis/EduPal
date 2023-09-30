using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PSI_Project.HelperFunctions;

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

    [HttpGet("list")]
    public IActionResult ListSubjects()
    {
        // TODO: delete comments
        // Subject subject = new Subject("0", "pewpew", "aaa");
        // _subjectHandler.CreateItem(subject);
        
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

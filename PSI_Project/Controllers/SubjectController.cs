using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PSI_Project.HelperFunctions;

namespace PSI_Project.Controllers;

[ApiController]
[Route("[controller]")]
public class SubjectController : ControllerBase
{
    private readonly SubjectHandler _subjectHandler = new();
    private readonly TopicHandler _topicHandler = new();

    [HttpGet("list")]
    public IActionResult ListSubjects()
    {
        // TODO: delete comments
        // Subject subject = new Subject("pewpew", "aaa");
        // _subjectHandler.InsertItem(subject);
        
        return Ok(_subjectHandler.GetSubjectList());
    }
    
    [HttpGet("{subjectId}")]
    public IActionResult ListTopics(string subjectId)
    {
        List<Topic> subjectTopics = new List<Topic>();
        foreach (var topic in _topicHandler.Items)
        {
            if (topic.Id == subjectId)
            {
                subjectTopics.Add(topic);
            }
        }
        return Ok(subjectTopics); 
    }
}

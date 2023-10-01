using Microsoft.AspNetCore.Mvc;
using PSI_Project.HelperFunctions;

namespace PSI_Project.Controllers;

[ApiController]
[Route("[controller]")]
public class TopicController : ControllerBase
{
    private readonly TopicHandler _topicHandler = new();

    [HttpGet("list/{subjectId}")]
    public IActionResult ListTopics(String subjectId)
    {
        // TODO: delete comments
        // Topic topic = new Topic("0", "0", "pewpew");
        // _topicHandler.InsertItem(topic);
        
        return Ok(_topicHandler.GetTopicListBySubjectId(subjectId));
    }
}
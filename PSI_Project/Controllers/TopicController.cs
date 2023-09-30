using Microsoft.AspNetCore.Mvc;
using PSI_Project.HelperFunctions;

namespace PSI_Project.Controllers;

[ApiController]
[Route("[controller]")]
public class TopicController : ControllerBase
{
    private readonly TopicHandler _topicHandler;

    public TopicController()
    {
        _topicHandler = new TopicHandler();
    }

    [HttpGet("list")]
    public IActionResult ListTopics()
    {
        // TODO: delete comments
        // Topic topic = new Topic("0", "pewpew", "aaa", "aaaa");
        // _topicHandler.CreateItem(topic);
        
        return Ok(_topicHandler.Items);
    }
}
using Microsoft.AspNetCore.Mvc;

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

    [HttpGet]
    public IActionResult ListTopics()
    {
        return Ok(_topicHandler.Items);
    }
}
using Microsoft.AspNetCore.Mvc;

namespace PSI_Project.Controllers;

[ApiController]
[Route("[controller]")]
public class TopicController : ControllerBase
{
    [HttpGet]
    public IActionResult ListTopics()
    {
        return Ok(TopicHandler.TopicList);
    }
}
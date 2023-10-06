using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PSI_Project.Models;
using PSI_Project.Repositories;

namespace PSI_Project.Controllers;

[ApiController]
[Route("[controller]")]
public class TopicController : ControllerBase
{
    private readonly TopicRepository _topicRepository = new TopicRepository();
    
    [HttpGet("list/{subjectName}")]
    public IActionResult ListTopics(string subjectName)
    {
        return Ok(_topicRepository.GetTopicsBySubjectName(subjectName)); 
    }
    
    [HttpPost("upload")]
    public IActionResult UploadTopic([FromBody] JsonElement request)
    {
        List<Topic>? topicList = _topicRepository.CreateTopic(request);
        return topicList == null
            ? BadRequest("Invalid request body")
            : Ok(topicList);
    }
    
    [HttpDelete("{topicId}/delete")]
    public IActionResult RemoveTopic(string topicId)
    {
        return _topicRepository.RemoveItemById(topicId) 
            ? Ok("Topic has been successfully deleted")
            : BadRequest("An error occured while deleting the topic");
    }
}

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
    private readonly TopicRepository _topicRepository;

    public TopicController(TopicRepository topicRepository)
    {
        _topicRepository = topicRepository;
    }

    [HttpGet("get/{topicId}")]
    public IActionResult GetTopic(string topicId)
    {
        Topic? topic = _topicRepository.GetItemById(topicId);
        return topic == null
            ? NotFound(new { error = "Topic not found." })
            : Ok(topic);
    }

    [HttpGet("list/{subjectId}")]
    public IActionResult ListTopics(string subjectId)
    {
        return Ok(_topicRepository.GetTopicsBySubjectId(subjectId));
    }

    [HttpPost("upload")]
    public IActionResult UploadTopic([FromBody] JsonElement request)
    {
        Topic? topic = _topicRepository.CreateTopic(request);
        return topic == null
            ? BadRequest("Invalid request body")
            : Ok(topic);
    }

    [HttpDelete("{topicId}/delete")]
    public IActionResult RemoveTopic(string topicId)
    {
        return _topicRepository.RemoveItemById(topicId)
            ? Ok("Topic has been successfully deleted")
            : BadRequest("An error occured while deleting the topic");
    }
}
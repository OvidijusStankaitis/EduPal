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
    public async Task<IActionResult> GetTopicByIdAsync(string topicId)
    {
        Topic? topic = await _topicRepository.GetAsync(topicId);
        return topic == null
            ? NotFound(new { error = "Topic not found." })
            : Ok(topic);
    }

    [HttpGet("list/{subjectId}")]
    public async Task<IActionResult> ListTopicsAsync(string subjectId)
    {
        var topics = await _topicRepository.GetTopicsListBySubjectIdAsync(subjectId);
        return Ok(topics);
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadTopicAsync([FromBody] JsonElement request)
    {
        Topic? topic = await _topicRepository.CreateAsync(request);
        return topic == null
            ? BadRequest("Invalid request body")
            : Ok(topic);
    }

    [HttpDelete("{topicId}/delete")]
    public async Task<IActionResult> RemoveTopicAsync(string topicId)
    {
        bool result = await _topicRepository.RemoveAsync(topicId);
        return result
            ? Ok("Topic has been successfully deleted")
            : BadRequest("An error occurred while deleting the topic");
    }
}
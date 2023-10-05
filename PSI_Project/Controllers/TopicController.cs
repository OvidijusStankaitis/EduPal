using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
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
        List<Topic> subjectTopics = _topicRepository.GetTopicsBySubjectName(subjectName);
        return Ok(subjectTopics); 
    }
    
    [HttpPost("upload")]
    public IActionResult UploadTopic([FromBody] JsonElement request)
    {
        if (request.TryGetProperty("topicName", out var topicNameProperty) &&
            request.TryGetProperty("subjectName", out var subjectNameProperty))
        {
            string topicName = topicNameProperty.GetString();
            string subjectName = subjectNameProperty.GetString();

            _topicRepository.InsertItem(new Topic(topicName, subjectName));
            return Ok(_topicRepository.Items);
        }
        return BadRequest("Invalid request body");
    }
    
    [HttpDelete("{topicName}/delete")]
    public void RemoveTopic(string topicName)
    {
        Topic? topicToRemove = _topicRepository.GetItemByName(topicName);
        if (topicToRemove != null)
        {
            _topicRepository.RemoveItem(topicToRemove.Id);
        }
    }
}

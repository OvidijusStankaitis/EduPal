using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using PSI_Project.HelperFunctions;

namespace PSI_Project.Controllers;

[ApiController]
[Route("[controller]")]
public class TopicController : ControllerBase
{
    private readonly TopicHandler _topicHandler = new TopicHandler();

    [HttpGet("list/{subjectName}")]
    public IActionResult ListTopics(string subjectName)
    {
        List<Topic> subjectTopics = new List<Topic>();
        foreach (var topic in _topicHandler.ItemList)
        {
            if (topic.SubjectName.Equals(subjectName))
            {
                subjectTopics.Add(topic);
            }
        }
        return Ok(subjectTopics); 
    }
    
    [HttpPost("upload")]
    public IActionResult UploadTopic([FromBody] JsonElement request)
    {
        if (request.TryGetProperty("topicName", out var topicNameProperty) && 
            request.TryGetProperty("topicDescription", out var topicDescriptionProperty) &&
            request.TryGetProperty("subjectName", out var subjectNameProperty))
        {
            string topicName = topicNameProperty.GetString();
            string topicDescription = topicDescriptionProperty.GetString() ?? " ";
            string subjectName = subjectNameProperty.GetString();

            _topicHandler.InsertItem(new Topic(topicName, subjectName, topicDescription));
            return Ok(_topicHandler.ItemList);
        }
        return BadRequest("Invalid request body");
    }
    
    [HttpDelete("{topicName}/delete")] //should "delete/" be here?
    public void RemoveTopic(string topicName)
    {
        Topic? topicToRemove = _topicHandler.CheckItemInList(topicName);
        if (topicToRemove != null)
        {
            _topicHandler.RemoveItem(topicToRemove.SubjectId);
        }
    }
}
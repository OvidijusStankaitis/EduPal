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
    public IActionResult GetTopicById(string topicId)
    {
        Topic? topic = _topicRepository.Get(topicId);
        return topic == null
            ? NotFound(new { error = "Topic not found." })
            : Ok(topic);
    }

    [HttpGet("list/{subjectId}")]
    public IActionResult ListTopics(string subjectId)
    {
        return Ok(_topicRepository.GetTopicsListBySubjectId(subjectId));
    }

    [HttpPost("upload")]
    public IActionResult UploadTopic([FromBody] JsonElement request)
    {
        Topic? topic = _topicRepository.Create(request);
        return topic == null
            ? BadRequest("Invalid request body")
            : Ok(topic);
    }
    
    [HttpPut("updateKnowledgeLevel")]
    public IActionResult UpdateKnowledgeLevel([FromBody] JsonElement request)
    {
        try
        {
            string topicId = request.GetProperty("topicId").GetString();
            string knowledgeLevel = request.GetProperty("knowledgeLevel").GetString();

            // Check if the knowledge level is valid (Good, Average, Poor).
            if (Enum.TryParse<KnowledgeLevel>(knowledgeLevel, true, out var level))
            {
                // Update the knowledge level of the topic.
                bool updated = _topicRepository.UpdateKnowledgeLevel(topicId, level);

                if (updated)
                {
                    return Ok("Knowledge level updated successfully");
                }
                else
                {
                    return BadRequest("Error updating knowledge level");
                }
            }
            else
            {
                return BadRequest("Invalid knowledge level");
            }
        }
        catch (Exception ex)
        {
            return BadRequest("Invalid request body: " + ex.Message);
        }
    }

    [HttpDelete("{topicId}/delete")]
    public IActionResult RemoveTopic(string topicId)
    {
        return _topicRepository.Remove(topicId)
            ? Ok("Topic has been successfully deleted")
            : BadRequest("An error occured while deleting the topic");
    }
}
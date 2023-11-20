using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using PSI_Project.DTO;
using PSI_Project.Exceptions;
using PSI_Project.Models;
using PSI_Project.Repositories;
using PSI_Project.Responses;

namespace PSI_Project.Controllers;

[ApiController]
[Route("[controller]")]
public class TopicController : ControllerBase
{
    private readonly TopicRepository _topicRepository;
    private readonly ILogger<TopicController> _logger;

    public TopicController(ILogger<TopicController> logger, TopicRepository topicRepository)
    {
        _logger = logger;
        _topicRepository = topicRepository;
    }

    [HttpGet("get/{topicId}")]
    public IActionResult GetTopicById(string topicId)
    {
        try
        {
            Topic topic = _topicRepository.Get(topicId);
            return Ok(topic);
        }
        catch (ObjectNotFoundException)
        {
            _logger.LogWarning("Tried to reach unavailable / non-existing topic {topicId}", topicId);
            return NotFound("There is no topic with such id");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting topic {topicId} information", topicId);
            return BadRequest(new { error = "Topic not found." });
        }
    }

    [HttpGet("list/{subjectId}")]
    public IActionResult ListTopics(string subjectId)
    {
        try
        {
            return Ok(_topicRepository.GetTopicsListBySubjectId(subjectId));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Couldn't list topics");
            return BadRequest("An error occured while getting topic list");
        }
    }

    [HttpPost("upload")]
    public IActionResult UploadTopic([FromBody] TopicUploadRequestDTO request)
    {
        try
        {
            Topic? topic = _topicRepository.Create(request.TopicName, request.SubjectId);
            if (topic != null)
            {
                return Ok(topic);
            }

            return BadRequest("Invalid topic name");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Couldn't add new topic");
            return BadRequest($"An error occurred while uploading topic");
        }
    }

    [HttpPut("update-knowledge-level")]
    public IActionResult UpdateKnowledgeLevel([FromBody] UpdateKnowledgeLevelRequestDTO request)
    {
        try
        {
            // Check if the knowledge level is valid (Good, Average, Poor).
            if (Enum.TryParse<KnowledgeLevel>(request.KnowledgeLevel, true, out var level))
            {
                // Update the knowledge level of the topic.
                bool updated = _topicRepository.UpdateKnowledgeLevel(request.TopicId, level);

                if (updated)
                {
                    return Ok("Knowledge level updated successfully");
                }

                return BadRequest("Error updating knowledge level");
            }

            return BadRequest("Invalid knowledge level");
        }
        catch (Exception ex)
        {
            return BadRequest("Invalid request body: " + ex.Message);
        }
    }

    [HttpDelete("delete/{topicId}")]
    public IActionResult RemoveTopic(string topicId)
    {
        try
        {
            _topicRepository.Remove(topicId);
            return Ok("Topic has been successfully deleted");
        }
        catch (Exception ex)
        { 
            _logger.LogError(ex, "Couldn't delete topic {topicId}", topicId);
            return BadRequest("An error occured while deleting the topic");
        }
    }
}
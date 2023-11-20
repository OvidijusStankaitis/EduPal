using Microsoft.AspNetCore.Mvc;
using PSI_Project.DTO;
using PSI_Project.Exceptions;
using PSI_Project.Models;
using PSI_Project.Repositories;

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
    public async Task<IActionResult> GetTopicByIdAsync(string topicId)
    {
        try
        {
            Topic topic = await _topicRepository.GetAsync(topicId);
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
    public async Task<IActionResult> ListTopicsAsync(string subjectId)
    {
        try
        {
            var topics = await _topicRepository.GetTopicsListBySubjectIdAsync(subjectId);
            return Ok(topics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Couldn't list topics");
            return BadRequest("An error occurred while getting topic list");
        }
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadTopicAsync([FromBody] TopicUploadRequestDTO request)
    {
        try
        {
            Topic? topic = await _topicRepository.CreateAsync(request.TopicName, request.SubjectId);
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
    public async Task<IActionResult> UpdateKnowledgeLevelAsync([FromBody] UpdateKnowledgeLevelRequestDTO request)
    {
        try
        {
            // Check if the knowledge level is valid (Good, Average, Poor).
            if (Enum.TryParse<KnowledgeLevel>(request.KnowledgeLevel, true, out var level))
            {
                // Update the knowledge level of the topic.
                bool updated = await _topicRepository.UpdateKnowledgeLevelAsync(request.TopicId, level);

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
    public async Task<IActionResult> RemoveTopicAsync(string topicId)
    {
        try
        {
            await _topicRepository.RemoveAsync(topicId);
            return Ok("Topic has been successfully deleted");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Couldn't delete topic {topicId}", topicId);
            return BadRequest("An error occurred while deleting the topic");
        }
    }
}
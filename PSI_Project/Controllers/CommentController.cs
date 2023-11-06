using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PSI_Project.Models;
using PSI_Project.Repositories;

namespace PSI_Project.Controllers;

[ApiController]
[Route("[controller]")]
public class CommentController : ControllerBase
{
    private readonly CommentRepository _commentRepository;
    private readonly ILogger<CommentController> _logger; 

    public CommentController(ILogger<CommentController> logger, CommentRepository commentRepository)
    {
        _logger = logger; 
        _commentRepository = commentRepository;
    }
    
    [HttpGet("get/{topicId}")]
    public IActionResult GetAllCommentsFromTopic(string topicId)
    {
        try
        {
            List<Comment> comment = _commentRepository.GetAllCommentsOfTopic(topicId);
            return Ok(comment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Couldn't list topic {topicId} comments", topicId);
        }
        
        return BadRequest("An error occured while getting all topic comments");
    }
}
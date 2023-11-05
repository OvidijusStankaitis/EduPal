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

    public CommentController(CommentRepository commentRepository)
    {
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
            // TODO: log errors
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
        }
        
        return BadRequest("An error occured while getting all topic comments"); // TODO: think of better way of showing error to a user
    }
}
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using PSI_Project.Models;
using PSI_Project.Repositories;
using System.Threading.Tasks;

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
    
    [HttpGet("getOne/{commentId}")]
    public async Task<IActionResult> GetComment(string commentId)
    {
        Comment? comment = await _commentRepository.GetItemByIdAsync(commentId);
        return comment == null
            ? NotFound(new { error = "Comment not found." })
            : Ok(comment);
    }
    
    [HttpGet("get/{topicId}")]
    public async Task<IActionResult> GetAllCommentsFromTopic(string topicId)
    {
        List<Comment> comments = await _commentRepository.GetAllCommentsOfTopicAsync(topicId);
        return Ok(comments);
    }
    
    [HttpPost("upload")]
    public async Task<IActionResult> UploadComment([FromBody] object request)
    {
        if (request is not JsonElement requestJson)
            return BadRequest("Invalid request body");
            
        Comment? comment = await _commentRepository.CreateCommentAsync(requestJson);
        return comment == null
            ? BadRequest("Invalid request body")
            : Ok(comment);
    }
    
    [HttpDelete("delete/{commentId}")]
    public async Task<IActionResult> RemoveTopic(string commentId)
    {
        return (await _commentRepository.RemoveAsync(commentId))
            ? Ok("Comment has been successfully deleted")
            : BadRequest("An error occured while deleting the comment");
    }
}
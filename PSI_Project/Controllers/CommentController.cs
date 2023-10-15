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
    private readonly CommentRepository _commentRepository = new CommentRepository();
    
    [HttpGet("getOne/{commentId}")]
    public IActionResult GetComment(string commentId)
    {
        Comment? comment = _commentRepository.GetItemById(commentId);
        return comment == null
            ? NotFound(new { error = "Comment not found." })
            : Ok(comment);
    }
    
    [HttpGet("get/{conspectusId}")]
    public IActionResult GetAllCommentsFromConspectus(string conspectusId)
    {
        List<Comment> comment = _commentRepository.GetAllCommentsOfConspectus(conspectusId);
        return Ok(comment);
    }
    
    [HttpPost("upload")]
    public IActionResult UploadComment([FromBody] JsonElement request)
    {
        Comment? comment = _commentRepository.CreateComment(request);
        return comment == null
            ? BadRequest("Invalid request body")
            : Ok(comment);
    }
    
    [HttpDelete("delete/{commentId}")]
    public IActionResult RemoveTopic(string commentId)
    {
        return _commentRepository.RemoveItemById(commentId) 
            ? Ok("Comment has been successfully deleted")
            : BadRequest("An error occured while deleting the topic");
    }
}
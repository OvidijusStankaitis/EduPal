using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PSI_Project.DTO;
using PSI_Project.Models;
using PSI_Project.Repositories;
using PSI_Project.Services;

namespace PSI_Project.Controllers;

[ApiController]
[Route("[controller]")]
public class CommentController : ControllerBase
{
    private readonly CommentRepository _commentRepository;
    private readonly ILogger<CommentController> _logger;
    private readonly ChatService _chatService;
    private readonly UserAuthService _userAuthService;

    public CommentController(ILogger<CommentController> logger, CommentRepository commentRepository, ChatService chatService, UserAuthService userAuthService)
    {
        _logger = logger; 
        _commentRepository = commentRepository;
        _chatService = chatService;
        _userAuthService = userAuthService;
    }
    
    [Authorize]
    [HttpGet("get/{topicId}")]
    public IActionResult GetCommentsForUser(string topicId)
    {
        try
        {
            User user = _userAuthService.GetUser(HttpContext)!;
            List<CommentDTO> comment = _chatService.GetMessagesForUser(user, topicId).ToList();
            return Ok(comment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Couldn't list topic {topicId} comments", topicId);
        }
        
        return BadRequest("An error occured while getting all topic comments");
    }
}
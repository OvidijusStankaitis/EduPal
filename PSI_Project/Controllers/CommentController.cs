using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PSI_Project.DTO;
using PSI_Project.Models;
using PSI_Project.Repositories;
using PSI_Project.Services;

namespace PSI_Project.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly ChatService _chatService;
        private readonly UserAuthService _userAuthService;
        
        private readonly ILogger<CommentController> _logger;
        
        public CommentController(ILogger<CommentController> logger, ChatService chatService, UserAuthService userAuthService)
        {
            _logger = logger; 
            _chatService = chatService;
            _userAuthService = userAuthService;
        }

        [Authorize]
        [HttpGet("get/{topicId}")]
        public async Task<IActionResult> GetCommentsForUser(string topicId)
        {
            try
            {
                User user = await _userAuthService.GetUser(HttpContext)!;
                List<CommentDTO> comments = _chatService.GetMessagesForUser(user, topicId).ToList();
                return Ok(comments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Couldn't list topic {topicId} comments", topicId);
            }
        
            return BadRequest("An error occured while getting all topic comments");
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using PSI_Project.Models;
using PSI_Project.Repositories;

namespace PSI_Project.Controllers
{
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
        public async Task<IActionResult> GetAllCommentsFromTopicAsync(string topicId)
        {
            try
            {
                List<Comment> comments = await _commentRepository.GetAllCommentsOfTopicAsync(topicId);
                return Ok(comments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Couldn't list topic {topicId} comments", topicId);
                return BadRequest("An error occurred while getting all topic comments");
            }
        }
    }
}
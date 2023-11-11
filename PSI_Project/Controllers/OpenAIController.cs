using Microsoft.AspNetCore.Mvc;
using PSI_Project.Repositories;
using PSI_Project.Services;

namespace PSI_Project.Controllers;

[ApiController]
[Route("[controller]")]
public class OpenAIController : ControllerBase
{
    private readonly OpenAIService _openAIService;
    private readonly OpenAIRepository _openAIRepository;
    private readonly ILogger<OpenAIController> _logger;

    public OpenAIController(ILogger<OpenAIController> logger, OpenAIService openAIService, OpenAIRepository openAIRepository)
    {
        _logger = logger;
        _openAIService = openAIService;
        _openAIRepository = openAIRepository;
    }

    [HttpPost("send-message")]
    public async Task<IActionResult> SendMessage([FromBody] string userMessage, [FromQuery] string userEmail)
    {
        try
        {
            var response = await _openAIService.SendMessageAsync(userMessage, userEmail);
            if (response != null)
            {
                return Ok(new { response });
            }
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Couldn't send message to OpenAI {userEmail}", userEmail);
        }
        
        return BadRequest(new { error = "Failed to get response from OpenAI." });
    }

    [HttpGet("get-messages")]
    public IActionResult GetMessages([FromQuery] string userEmail)
    {
        try
        {
            return Ok(_openAIRepository.GetItemsByEmail(userEmail));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting openAI chat {userEmail}", userEmail);
            return BadRequest("An error occured while getting the messages");
        }
    }
}
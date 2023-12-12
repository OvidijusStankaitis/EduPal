using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PSI_Project.Models;
using PSI_Project.Repositories;
using PSI_Project.Services;

namespace PSI_Project.Controllers;

[ApiController]
[Route("[controller]")]
public class OpenAIController : ControllerBase
{
    private readonly OpenAIService _openAIService;
    private readonly OpenAIRepository _openAIRepository;
    private readonly IUserAuthService _userAuthService;
    private readonly ILogger<OpenAIController> _logger;

    public OpenAIController(ILogger<OpenAIController> logger, OpenAIService openAIService,
        OpenAIRepository openAIRepository, IUserAuthService userAuthService)
    {
        _logger = logger;
        _openAIService = openAIService;
        _openAIRepository = openAIRepository;
        _userAuthService = userAuthService;
    }

    [Authorize]
    [HttpPost("send-message")]
    public async Task<IActionResult> SendMessage([FromBody] string userMessage)
    {
        User? user = await _userAuthService.GetUser(HttpContext); 
        string userEmail = user.Email;
        
        try
        {
            var response = await _openAIService.SendMessageAsync(userMessage, userEmail);
            if (response != null)
            {
                return Ok(new { response });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Couldn't send message to OpenAI {userEmail}", userEmail);
        }

        return BadRequest(new { error = "Failed to get response from OpenAI." });
    }

    [Authorize]
    [HttpGet("get-messages")]
    public async Task<IActionResult> GetMessages()
    {
        User? user = await _userAuthService.GetUser(HttpContext); 
        string userEmail = user.Email;
        
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
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

    public OpenAIController(OpenAIService openAIService, OpenAIRepository openAIRepository)
    {
        _openAIService = openAIService;
        _openAIRepository = openAIRepository;
    }

    [HttpPost("send-message")]
    public async Task<IActionResult> SendMessage([FromBody] string userMessage, [FromQuery] string userEmail)
    {
        var response = await _openAIService.SendMessageAsync(userMessage, userEmail);
        if (response == null) return BadRequest(new { error = "Failed to get response from OpenAI." });
        return Ok(new { response });
    }

    [HttpGet("get-messages")]
    public IActionResult GetMessages([FromQuery] string userEmail)
    {
        return Ok(_openAIRepository.GetItemsByEmail(userEmail));
    }

}
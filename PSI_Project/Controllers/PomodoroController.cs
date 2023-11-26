using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PSI_Project.Requests;
using PSI_Project.Services;

[ApiController]
[Route("[controller]")]
public class PomodoroController : ControllerBase
{
    private readonly PomodoroService _pomodoroService;
    private readonly ILogger<PomodoroController> _logger;

    public PomodoroController(PomodoroService pomodoroService, ILogger<PomodoroController> logger)
    {
        _pomodoroService = pomodoroService;
        _logger = logger;
    }
    
    [HttpPost("start-timer")]
    public IActionResult StartTimer([FromBody] StartTimerRequest request)
    {
        _logger.LogInformation($"Starting timer for {request.UserEmail} with intensity {request.Intensity}");
        _pomodoroService.StartTimer(request.UserEmail, request.Intensity);
        return Ok();
    }
    
    [HttpPost("stop-timer")]
    public IActionResult StopTimer([FromBody] StopTimerRequest request)
    {
        _logger.LogInformation($"Stopping timer for {request.UserEmail}");
        _pomodoroService.StopTimer(request.UserEmail);
        return Ok();
    }

    [HttpGet("get-timer-state")]
    public ActionResult GetTimerState(string userEmail)
    {
        _logger.LogInformation($"Getting timer state for {userEmail}");
        var state = _pomodoroService.GetTimerState(userEmail);
        _logger.LogInformation($"Timer state for {userEmail}: {state}");

        var response = new 
        {
            RemainingTime = state.RemainingTime,
            Mode = state.Mode,
            IsActive = state.IsActive
        };

        return Ok(response);
    }
}
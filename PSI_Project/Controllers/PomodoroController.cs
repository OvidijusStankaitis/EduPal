using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PSI_Project.Models;
using PSI_Project.Requests;
using PSI_Project.Services;

[ApiController]
[Route("[controller]")]
public class PomodoroController : ControllerBase
{
    private readonly PomodoroService _pomodoroService;
    private readonly UserAuthService _userAuthService;
    
    private readonly ILogger<PomodoroController> _logger;

    public PomodoroController(PomodoroService pomodoroService, UserAuthService userAuthService, ILogger<PomodoroController> logger)
    {
        _pomodoroService = pomodoroService;
        _userAuthService = userAuthService;
        _logger = logger;
    }
    
    [Authorize]
    [HttpPost("start-timer")]
    public IActionResult StartTimer([FromBody] StartTimerRequest request)
    {
        User user = _userAuthService.GetUser(HttpContext)!;
        _logger.LogInformation($"Starting timer for {user.Id} with intensity {request.Intensity}");
        _pomodoroService.StartTimer(user.Id, request.Intensity);
        return Ok();
    }
    
    [Authorize]
    [HttpPost("stop-timer")]
    public IActionResult StopTimer()
    {
        User user = _userAuthService.GetUser(HttpContext)!;
        _pomodoroService.StopTimer(user.Id);
        
        _logger.LogInformation($"Stopping timer for {user!.Id}");
        
        return Ok();
    }

    [Authorize]
    [HttpGet("get-timer-state")]
    public ActionResult GetTimerState()
    {
        User user = _userAuthService.GetUser(HttpContext)!;
        _logger.LogInformation($"Getting timer state for {user.Id}");
        var state = _pomodoroService.GetTimerState(user.Id);
        _logger.LogInformation($"Timer state for {user.Id}: {state}");

        var response = new 
        {
            state.RemainingTime,
            state.Mode,
            state.IsActive
        };

        return Ok(response);
    }
}
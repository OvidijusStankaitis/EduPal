using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PSI_Project.Models;
using PSI_Project.Requests;
using PSI_Project.Services;

namespace PSI_Project.Controllers;

[ApiController]
[Route("[controller]")]
public class PomodoroController : ControllerBase
{
    private readonly PomodoroService _pomodoroService;
    private readonly IUserAuthService _userAuthService;
    
    private readonly ILogger<PomodoroController> _logger;

    public PomodoroController(PomodoroService pomodoroService, IUserAuthService userAuthService, ILogger<PomodoroController> logger)
    {
        _pomodoroService = pomodoroService;
        _userAuthService = userAuthService;
        _logger = logger;
    }
    
    [Authorize]
    [HttpPost("start-timer")]
    public async Task<ActionResult> StartTimer([FromBody] StartTimerRequest request)
    {
        try
        {
            User? user = await _userAuthService.GetUser(HttpContext);
            _pomodoroService.StartTimer(user!.Id, request.Intensity);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Couldn't start timer pomodoro timer");
        }

        return BadRequest("An error occured while starting pomodoro timer");
    }
    
    [Authorize]
    [HttpGet("stop-timer")]
    public async Task<ActionResult> StopTimer()
    {
        try
        {
            User? user = await _userAuthService.GetUser(HttpContext)!;
            _pomodoroService.StopTimer(user!.Id);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Couldn't stop timer pomodoro timer");
        }

        return BadRequest("An error occured while starting pomodoro timer");
    }

    [Authorize]
    [HttpGet("get-timer-state")]
    public async Task<ActionResult> GetTimerState()
    {
        try
        {
            User? user = await _userAuthService.GetUser(HttpContext);
            var state = _pomodoroService.GetTimerState(user!.Id);

            var response = new 
            {
                state.RemainingTime,
                state.Mode,
                state.IsActive
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Couldn't get pomodoro timer state.");
        }
        
        return BadRequest("An error occured while getting pomodoro timer state");
    }
}
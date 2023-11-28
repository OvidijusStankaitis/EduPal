using Microsoft.AspNetCore.Mvc;
using PSI_Project.Requests;
using PSI_Project.Services;

[ApiController]
[Route("[controller]")]
public class PomodoroController : ControllerBase
{
    private readonly PomodoroService _pomodoroService;

    public PomodoroController(PomodoroService pomodoroService)
    {
        _pomodoroService = pomodoroService;
    }
    
    [HttpPost("start-timer")]
    public IActionResult StartTimer([FromBody] StartTimerRequest request)
    {
        _pomodoroService.StartTimer(request.UserEmail, request.Intensity);
        return Ok();
    }
    
    [HttpPost("stop-timer")]
    public IActionResult StopTimer([FromBody] StopTimerRequest request)
    {
        _pomodoroService.StopTimer(request.UserEmail);
        return Ok();
    }

    [HttpGet("get-timer-state")]
    public ActionResult GetTimerState(string userEmail)
    {
        var state = _pomodoroService.GetTimerState(userEmail);
        var response = new 
        {
            RemainingTime = state.RemainingTime,
            Mode = state.Mode,
            IsActive = state.IsActive
        };

        return Ok(response);
    }
}
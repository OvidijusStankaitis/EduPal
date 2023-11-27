using Microsoft.AspNetCore.Mvc;
using PSI_Project.Repositories;
using PSI_Project.DTO;

namespace PSI_Project.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly UserRepository _userRepository;
    private readonly ILogger<UserController> _logger;

    public UserController(ILogger<UserController> logger, UserRepository userRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] UserCreationDTO newUser)
    {
        try
        {
            string? userId = await _userRepository.CheckUserRegisterAsync(newUser);
            if (userId != null)
            {
                return Ok(new { success = true, message = "Registration successful.", userId });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Couldn't register user {NewUserModel}", newUser);
        }

        return BadRequest(new { success = false, message = "Invalid payload." });
    }


    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] UserLoginRequestDTO request)
    {
        try
        {
            string? userId = await _userRepository.CheckUserLoginAsync(request.Email, request.Password);
            if (userId != null)
            {
                return Ok(new { success = true, message = "Login successful.", userId });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Couldn't check user login information");
        }

        return BadRequest(new { success = false, message = "Invalid payload." });
    }

    [HttpGet("get-name")]
    public IActionResult GetName(string email)
    {
        var user = _userRepository.GetUserByEmailAsync(email);
        if (user != null)
        {
            return Ok(new { name = user.Result.Name });
        }

        return NotFound(new { message = "User not found." });
    }
}
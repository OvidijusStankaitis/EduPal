using Microsoft.AspNetCore.Mvc;
using PSI_Project.Repositories;
using System.Text.Json;
using PSI_Project.DTO;
using PSI_Project.Models;

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
    public IActionResult Register([FromBody] UserCreationDTO newUser)
    {
        try
        {
            string? userId = _userRepository.CheckUserRegister(newUser);
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
    public IActionResult Login([FromBody] JsonElement payload)
    {
        try
        {
            string? userId = _userRepository.CheckUserLogin(payload);
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
}
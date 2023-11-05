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

    public UserController(UserRepository userRepository)
    {
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
            // TODO: log errors
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
        }

        return BadRequest(new { success = false, message = "Invalid payload." }); // TODO: think of better way of showing error to a user
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
            // TODO: log errors
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
        }

        return BadRequest(new { success = false, message = "Invalid payload." }); // TODO: think of better way of showing error to a user
    }
}
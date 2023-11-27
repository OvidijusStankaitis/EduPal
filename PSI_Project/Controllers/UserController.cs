using Microsoft.AspNetCore.Mvc;
using PSI_Project.Repositories;
using Microsoft.AspNetCore.Authorization;
using PSI_Project.DTO;
using PSI_Project.Models;
using PSI_Project.Services;

namespace PSI_Project.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    
    private readonly UserAuthService _userAuthService;
    private readonly UserRepository _userRepository;

    public UserController(ILogger<UserController> logger, UserAuthService userAuthService, UserRepository userRepository)
    {
        _logger = logger;
        
        _userAuthService = userAuthService;
        _userRepository = userRepository;
    }
    
    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest loginData)
    {
        try
        {
            User? user = _userAuthService.Authenticate(loginData);
            
            if (user != null)
            {
                string token = _userAuthService.GenerateToken(user);
                Response.Cookies.Append("token", token, new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddMinutes(15),
                    HttpOnly = true,
                    Secure = true,
                    IsEssential = true,
                    SameSite = SameSiteMode.None
                });
                
                return Ok(new {message = "Login was successful"});
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Couldn't check user login information");
        }

        return BadRequest(new {message = "Invalid login information"});
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequest registerData)
    {
        try
        {
            User? user = _userRepository.Create(registerData);
            
            if (user != null)
            {
                string token = _userAuthService.GenerateToken(user);
                Response.Cookies.Append("token", token, new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddMinutes(15),
                    HttpOnly = true,
                    Secure = true,
                    IsEssential = true,
                    SameSite = SameSiteMode.None
                });
                
                return Ok(new { success = true, message = "Registration successful.", user.Id });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Couldn't register user {NewUserModel}", registerData);
        }

        return BadRequest(new { success = false, message = "Invalid payload." });
    }
    
    [Authorize]
    [HttpGet("get-user-name")]
    public IActionResult GetName()
    {
        try
        {
            User? user = _userAuthService.GetUser(HttpContext);
            return Ok(new { message = "User was successfully found", user?.Name });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Couldn't found user");
            return NotFound(new { message = "User not found." });
        }
    }
}
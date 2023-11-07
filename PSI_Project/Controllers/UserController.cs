using Microsoft.AspNetCore.Mvc;
using PSI_Project.Repositories;
using System.Text.Json;
using PSI_Project.DTO;
using PSI_Project.Models;

namespace PSI_Project.Controllers
{
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
            string? userId = _userRepository.CheckUserRegister(newUser);
            return userId == null
                ? BadRequest(new { success = false, message = "Invalid payload."})
                : Ok(new { success = true, message = "Registration successful.", userId});
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] JsonElement payload)
        {
            string? userId = _userRepository.CheckUserLogin(payload);
            return userId == null
                ? BadRequest(new { success = false, message = "Invalid payload." })
                : Ok(new { success = true, message = "Login successful.", userId});
        }

        [HttpGet("get-name")]
        public IActionResult GetName(string email)
        {
            var user = _userRepository.GetUserByEmail(email);
            if (user != null)
            {
                return Ok(new { name = user.Name });
            }

            return NotFound(new { message = "User not found." });
        }
    }
}
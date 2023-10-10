using Microsoft.AspNetCore.Mvc;
using PSI_Project.Repositories;
using System.Text.Json;

namespace PSI_Project.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserRepository _userRepository = new UserRepository();

        [HttpPost("register")]
        public IActionResult Register(User user)
        {
            var existingUser = _userRepository.GetUserByEmail(user.Email);
            if (existingUser != null)
                return BadRequest(new { success = false, message = "Email already exists." });

            _userRepository.InsertItem(user);
            return Ok(new { success = true, message = "User registered successfully." });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] JsonElement payload)
        {
            if (payload.TryGetProperty("email", out JsonElement emailElement) &&
                payload.TryGetProperty("password", out JsonElement passwordElement))
            {
                var email = emailElement.GetString();
                var password = passwordElement.GetString();

                var user = _userRepository.GetUserByEmail(email);
                if (user == null || user.Password != password)
                    return BadRequest(new { success = false, message = "Invalid email or password." });

                return Ok(new { success = true, message = "Login successful." });
            }

            return BadRequest(new { success = false, message = "Invalid payload." });
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
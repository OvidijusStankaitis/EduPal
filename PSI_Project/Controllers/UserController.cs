using Microsoft.AspNetCore.Mvc;
using PSI_Project.Repositories;
using System.Text.Json;

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
        public IActionResult Register(User user)
        {
            return _userRepository.CheckUserRegister(user)
                ? Ok(new { success = true, message = "Registration successful." })
                : BadRequest(new { success = false, message = "Invalid payload." });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] JsonElement payload)
        {
            return _userRepository.CheckUserLogin(payload)
                ? BadRequest(new { success = false, message = "Invalid payload." })
                : Ok(new { success = true, message = "Login successful." });
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
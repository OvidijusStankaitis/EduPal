using PSI_Project.DTO;
using PSI_Project.Models;

namespace PSI_Project.Services;

public interface IUserAuthService
{
    public User? Authenticate(LoginRequest userData);
    public string GenerateToken(User user);
    public Task<User?> GetUser(HttpContext context);
}
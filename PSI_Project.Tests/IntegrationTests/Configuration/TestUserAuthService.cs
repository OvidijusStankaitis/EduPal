using Microsoft.AspNetCore.Http;
using PSI_Project.DTO;
using PSI_Project.Models;
using PSI_Project.Services;

namespace PSI_Project.Tests.IntegrationTests.Configuration;

public class TestUserAuthService : IUserAuthService
{
    private User? _authenticatedUser = null;

    public void SetAuthenticatedUser(User user)
    {
        _authenticatedUser = user;
    }

    public void ClearAuthenticatedUser()
    {
        _authenticatedUser = null;
    }

    public User? Authenticate(LoginRequest userData)
    {
        return _authenticatedUser;
    }

    public string GenerateToken(User user)
    {
        return "user-auth-token";
    }

    public async Task<User?> GetUser(HttpContext context)
    {
        return await Task.FromResult(_authenticatedUser);
    }
}
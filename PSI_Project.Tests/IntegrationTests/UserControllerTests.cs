using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using PSI_Project.DTO;
using PSI_Project.Models;
using PSI_Project.Services;
using PSI_Project.Tests.IntegrationTests.Configuration;

namespace PSI_Project.Tests.IntegrationTests;

public class UserControllerTests
{
    private readonly HttpClient _client;
    private readonly TestingWebAppFactory _factory;
    
    private record RegisterAsyncOkResponse(bool success, string message, string? userId);    
    private record RegisterAsyncBadRequestResponse(bool success, string message);
    private record LoginAsyncBadRequestResponse(string message);
    private record LoginAsyncOkResponse(string message);
    private record GetNameOkResponse(string message, string name);
    
    public UserControllerTests()
    {
        _factory = new TestingWebAppFactory();
        _client = _factory.CreateClient();
    }
    
    [Fact]
    public async Task RegisterAsync_UserWithSuchEmailDoesNotExist_ReturnsOk()
    {
        // Arrange
        var newUser = new User("test2@test.test", "testPassword2", "newUserName", "newUserSurname");
        
        // Act
        var response = await _client.PostAsync("/user/register", JsonContent.Create(newUser));
        var responseString = await response.Content.ReadAsStringAsync();
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var responseData  = JsonSerializer.Deserialize<RegisterAsyncOkResponse>(responseString);
        Assert.NotNull(responseData);
        Assert.True(responseData.success); 
        Assert.Equal("Registration successful.", responseData.message);
        
        var cookies = response.Headers.GetValues("set-cookie");
        Assert.Contains(cookies, item => item.Contains("user-auth-token"));
    }

    [Fact]
    public async Task RegisterAsync_UserWithSuchEmailAlreadyExists_ReturnsBadRequest()
    {
        // Arrange 
        User existingUser = new User("test1@test.test", "testPassword1", "testName", "testSurname");

        // Act
        var response = await _client.PostAsync("/user/register", JsonContent.Create(existingUser));
        var responseString = await response.Content.ReadAsStringAsync();
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        var responseData  = JsonSerializer.Deserialize<RegisterAsyncBadRequestResponse>(responseString);
        Assert.NotNull(responseData);
        Assert.False(responseData.success); 
        Assert.Equal("Invalid payload.", responseData.message);
    }
    
    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsOk()
    {
        // Arrange
        User user = new User("test1@test.test", "testPassword1", "testName", "testSurname")
        {
            Id = "test-user-id-1"
        };
        
        using var scope = _factory.Services.CreateScope();
        TestUserAuthService? testAuthService = scope.ServiceProvider.GetRequiredService<IUserAuthService>() as TestUserAuthService;
        testAuthService?.SetAuthenticatedUser(user);
        
        LoginRequest request = new LoginRequest("test1@test.test", "password");
    
        // Act
        var response = await _client.PostAsync("/user/login", JsonContent.Create(request));
        var responseString = await response.Content.ReadAsStringAsync();
    
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    
        var responseData = JsonSerializer.Deserialize<LoginAsyncOkResponse>(responseString);
        Assert.NotNull(responseData);
        Assert.Equal("Login was successful", responseData.message);
        
        var cookies = response.Headers.GetValues("set-cookie");
        Assert.Contains(cookies, item => item.Contains("user-auth-token"));
    }
    
    [Fact]
    public async Task LoginAsync_InvalidCredentials_ReturnsBadRequest()
    {
        // Arrange 
        LoginRequest request = new LoginRequest("non-existant-user@test.test", "password");
    
        // Act
        var response = await _client.PostAsync("/user/login", JsonContent.Create(request));
        var responseString = await response.Content.ReadAsStringAsync();
    
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    
        var responseData = JsonSerializer.Deserialize<LoginAsyncBadRequestResponse>(responseString);
        Assert.NotNull(responseData);
        Assert.Equal("Invalid login information", responseData.message);
    }
    
    [Fact]
    public async Task GetName_ValidUser_ReturnsOk()
    {
        // Arrange
        User user = new User("test1@test.test", "testPassword1", "testName", "testSurname")
        {
            Id = "test-user-id-1"
        };
        
        using var scope = _factory.Services.CreateScope();
        TestUserAuthService? testAuthService = scope.ServiceProvider.GetRequiredService<IUserAuthService>() as TestUserAuthService;
        testAuthService?.SetAuthenticatedUser(user);

        // Act
        var response = await _client.GetAsync("/user/get-user-name");
        var responseString = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var responseData = JsonSerializer.Deserialize<GetNameOkResponse>(responseString);
        Assert.NotNull(responseData);
        Assert.Equal("User was successfully found", responseData.message);
        Assert.Equal("testName", responseData.name);
    }
}
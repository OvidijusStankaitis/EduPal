using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using PSI_Project.Models;
using Xunit.Abstractions;

namespace PSI_Project.Tests.IntegrationTests;

public class UserControllerIntegrationTests : IDisposable
{
    private readonly HttpClient _client;
    private readonly TestingWebAppFactory _factory;
    
    record UserControllerOkResponse(bool success, string message, string? userId);
    record UserControllerGetNameOkResponse(string name);
    record UserControllerGetNameNotFoundResponse(string message);
    record UserControllerBadRequestResponse(bool success, string message);
    
    public UserControllerIntegrationTests()
    {
        _factory = new TestingWebAppFactory();
        _client = _factory.CreateClient();
    }
    
    [Fact] 
    public async Task RegisterAsync_GetsNewUser_ReturnsOk()
    {
        // Arrange
        var newUser = new User("test2@test.test", "testPassword2", "newUserName", "newUserSurname");
        
        // Act
        var response = await _client.PostAsync("/user/register", JsonContent.Create(newUser));
        var responseString = await response.Content.ReadAsStringAsync();
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var responseData  = JsonSerializer.Deserialize<UserControllerOkResponse>(responseString);
        Assert.NotNull(responseData);
        Assert.True(responseData.success); 
        Assert.Equal("Registration successful.", responseData.message);
        Assert.NotNull(responseData.userId);
    }
    
    [Fact] 
    public async Task RegisterAsync_GetsExistingUser_ReturnsBadRequest()
    {
        // Arrange
        var existingUser = new User("test1@test.test", "testPassword1", "testName", "testSurname");
        
        // Act
        var response = await _client.PostAsync("/user/register", JsonContent.Create(existingUser));
        var responseString = await response.Content.ReadAsStringAsync();
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        var responseData  = JsonSerializer.Deserialize<UserControllerBadRequestResponse>(responseString);
        Assert.NotNull(responseData);
        Assert.False(responseData.success); 
        Assert.Equal("Invalid payload.", responseData.message);
    }
    
    [Fact] 
    public async Task LoginAsync_GetsValidExistingUserLogin_ReturnsOk()
    {
        // Arrange
        var validUserLogin = new
        {
            email = "test1@test.test",
            password = "testPassword1"
        };
        
        // Act
        var response = await _client.PostAsync("/user/login", JsonContent.Create(validUserLogin));
        var responseString = await response.Content.ReadAsStringAsync();
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var responseData  = JsonSerializer.Deserialize<UserControllerOkResponse>(responseString);
        Assert.NotNull(responseData);
        Assert.True(responseData.success); 
        Assert.Equal("Login successful.", responseData.message);
        Assert.NotNull(responseData.userId);
    }
    
    [Fact] 
    public async Task LoginAsync_GetsNonexistentUserLogin_ReturnsBadRequest()
    {
        // Arrange
        var invalidUserLogin = new
        {
            email = "nonexistent@test.test",
            password = "nonexistentTestPassword"
        }; 
        
        // Act
        var response = await _client.PostAsync("/user/login", JsonContent.Create(invalidUserLogin));
        var responseString = await response.Content.ReadAsStringAsync();
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var responseData  = JsonSerializer.Deserialize<UserControllerBadRequestResponse>(responseString);
        
        Assert.NotNull(responseData);
        Assert.False(responseData.success); 
        Assert.Equal("Invalid payload.", responseData.message);
    }

    [Fact] 
    public async Task GetName_GetsValidExistingEmail_ReturnsOkAndUserName()
    {
        // Arrange
        
        // Act
        var response = await _client.GetAsync("/user/get-name/?email=test1@test.test");
        var responseString = await response.Content.ReadAsStringAsync();
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var responseData  = JsonSerializer.Deserialize<UserControllerGetNameOkResponse>(responseString);
        Assert.NotNull(responseData);
        Assert.NotNull(responseData.name);
    }
    
    [Fact] 
    public async Task GetName_GetsNonexistentEmail_ReturnsNotFound()
    {
        // Arrange
        
        // Act
        var response = await _client.GetAsync("/user/get-name/?email=nonexistent@test.test");
        var responseString = await response.Content.ReadAsStringAsync();
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        
        var responseData  = JsonSerializer.Deserialize<UserControllerGetNameNotFoundResponse>(responseString);
        Assert.NotNull(responseData);
        Assert.Equal("User not found.", responseData.message);
    }
    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}
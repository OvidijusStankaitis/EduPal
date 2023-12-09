using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using PSI_Project.Models;
using PSI_Project.Tests.IntegrationTests.Configuration;

namespace PSI_Project.Tests.IntegrationTests;

public class UserControllerTests
{
    private readonly HttpClient _client;
    private readonly TestingWebAppFactory _factory;
    
    private record RegisterAsyncOkResponse(bool success, string message, string? userId);    
    private record RegisterAsyncBadRequestResponse(bool success, string message);
    
    public UserControllerTests()
    {
        _factory = new TestingWebAppFactory();
        _client = _factory.CreateClient();
    }
    
    [Fact]
    public async Task RegisterAsync_UserWithSuchEmailDoesNotExist_ReturnsBadRequest()
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
}
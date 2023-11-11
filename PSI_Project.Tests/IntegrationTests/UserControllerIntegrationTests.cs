using System.Net;
using System.Net.Http.Json;

namespace PSI_Project.Tests.IntegrationTests;

public class UserControllerIntegrationTests : IDisposable
{
    private readonly HttpClient _client;
    private readonly TestingWebAppFactory _factory;
    
    public UserControllerIntegrationTests()
    {
        _factory = new TestingWebAppFactory();
        _client = _factory.CreateClient();
    }
    
    [Fact] 
    public async Task Register_GetsNewUser_ReturnsOk()
    {
        // Arrange
        var newUser = new User("test2@test.test", "testPassword2", "newUserName", "newUserSurname");
        
        // Act
        var response = await _client.PostAsync("user/register", JsonContent.Create(newUser));
        var responseString = await response.Content.ReadAsStringAsync();
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("{\"success\":true,\"message\":\"Registration successful.\"}", responseString);
    }
    
    [Fact] 
    public async Task Register_GetsExistingUser_ReturnsBadRequest()
    {
        // Arrange
        var existingUser = new User("test1@test.test", "testPassword1", "testName", "testSurname");
        
        // Act
        var response = await _client.PostAsync("user/register", JsonContent.Create(existingUser));
        var responseString = await response.Content.ReadAsStringAsync();
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("{\"success\":false,\"message\":\"Invalid payload.\"}", responseString);
    }
    
    [Fact] 
    public async Task Login_GetsValidExistingUserLogin_ReturnsOk()
    {
        // Arrange
        var validUserLogin = new
        {
            email = "test1@test.test",
            password = "testPassword1"
        }; 
        
        // Act
        var response = await _client.PostAsync("user/login", JsonContent.Create(validUserLogin));
        var responseString = await response.Content.ReadAsStringAsync();
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("{\"success\":true,\"message\":\"Login successful.\"}", responseString);
    }
    
    [Fact] 
    public async Task Login_GetsNonexistentUserLogin_ReturnsBadRequest()
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
        Assert.Equal("{\"success\":false,\"message\":\"Invalid payload.\"}", responseString);
    }

    // [Fact] 
    // public async Task GetName_GetsValidExistingEmail_ReturnsOkAndUserName() // WHERE AM I SUPPOSED TO WRITE EMAIL???
    // {
    //     // Arrange
    //     
    //     // Act
    //     var response = await _client.GetAsync("/user/get-name");
    //     var responseString = await response.Content.ReadAsStringAsync();
    //     
    //     // Assert
    //     Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    // }
    //
    // [Fact] 
    // public async Task GetName_GetsNonexistentEmail_ReturnsNotFound() // WHERE AM I SUPPOSED TO WRITE EMAIL???
    // {
    //     // Arrange
    //     
    //     // Act
    //     var response = await _client.GetAsync("/user/get-name");
    //     var responseString = await response.Content.ReadAsStringAsync();
    //     
    //     // Assert
    //     Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    // }
    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}
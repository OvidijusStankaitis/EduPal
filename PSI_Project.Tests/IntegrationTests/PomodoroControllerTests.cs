using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using PSI_Project.Data;
using PSI_Project.Models;
using PSI_Project.Requests;
using PSI_Project.Services;
using PSI_Project.Tests.IntegrationTests.Configuration;
using Xunit.Abstractions;

namespace PSI_Project.Tests.IntegrationTests;

public class PomodoroControllerIntegrationTests : IDisposable
{
    private readonly HttpClient _client;
    private readonly TestingWebAppFactory _factory;
    
    public PomodoroControllerIntegrationTests()
    {
        _factory = new TestingWebAppFactory();
        _client = _factory.CreateClient();
        
        // Setting up logged in user
        User user = new User("test1@test.test", "testPassword1", "testName", "testSurname")
        {
            Id = "test-user-id-1"
        };

        using var scope = _factory.Services.CreateScope();
        TestUserAuthService? testAuthService = scope.ServiceProvider.GetRequiredService<IUserAuthService>() as TestUserAuthService;
        testAuthService?.SetAuthenticatedUser(user);
    }

     [Fact]
     public async Task StartTimer_GivenValidRequest_ReturnsOk()
     { 
         // Arrange
         StartTimerRequest startRequest = new StartTimerRequest { Intensity = "Low" };

         // Act
         var response = await _client.PostAsJsonAsync("/pomodoro/start-timer", startRequest);

         // Assert
         Assert.Equal(HttpStatusCode.OK, response.StatusCode);
     }
     
     [Fact]
     public async Task StartTimer_GivenInvalidRequest_ReturnsBadRequest()
     { 
         // Arrange
         StartTimerRequest startRequest = new StartTimerRequest { Intensity = "invalid-intensity" };

         // Act
         var response = await _client.PostAsJsonAsync("/pomodoro/start-timer", startRequest);

         // Assert
         Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
     }

     [Fact]
     public async Task StopTimer_GivenValidRequest_ReturnsOk()
     {
         // Arrange
         
         // Act
         var response = await _client.GetAsync("/pomodoro/stop-timer");

         // Assert
         Assert.Equal(HttpStatusCode.OK, response.StatusCode);
     }
     
     public void Dispose()
     {
         _client.Dispose();
         _factory.Dispose();
     }
}

using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using PSI_Project.Models;
using PSI_Project.Requests;
using PSI_Project.Services;
using PSI_Project.Tests.IntegrationTests.Configuration;

namespace PSI_Project.Tests.IntegrationTests;

public class GoalsControllerTests : IDisposable
{
    private readonly HttpClient _client;
    private readonly TestingWebAppFactory _factory;

    public GoalsControllerTests()
    {
        _factory = new TestingWebAppFactory();
        _client = _factory.CreateClient();

        // Setting up logged in user
        User user = new User("test1@test.test", "testPassword1", "testName", "testSurname")
        {
            Id = "test-user-id-1"
        };

        using var scope = _factory.Services.CreateScope();
        var testAuthService =
            scope.ServiceProvider.GetRequiredService<IUserAuthService>() as TestUserAuthService;
        testAuthService?.SetAuthenticatedUser(user);
    }

    [Fact]
    public async Task CreateGoalWithSubjects_ValidRequest_ReturnsOk()
    {
        // Arrange
        var createGoalRequest = new CreateGoalRequest
        {
            SubjectIds = new List<string> { "subject-id-1", "subject-id-2" }, // Use a List<string> instead of an array
            GoalTime = 1.5 // Set a valid goal time
        };

        // Act
        var response = await _client.PostAsJsonAsync("/goals/create", createGoalRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public async Task GetAllSubjects_ReturnsListOfSubjects()
    {
        // Act
        var response = await _client.GetAsync("/goals/subjects");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetAllGoalsForUserWithDetails_ReturnsListOfGoals()
    {
        // Act
        var response = await _client.GetAsync("/goals/view-all");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}
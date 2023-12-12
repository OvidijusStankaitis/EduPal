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
    public async Task CreateGoalWithSubjects_NoSubjectsWereSelected_ReturnsBadRequest()
    {
        // Arrange
        var createGoalRequest = new CreateGoalRequest
        {
            SubjectIds = new List<string>(),
            GoalTime = 1.5
        };
    
        // Act
        var response = await _client.PostAsJsonAsync("/goals/create", createGoalRequest);
    
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        var responseData = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        Assert.NotNull(responseData);
    }
    
    [Fact]
    public async Task CreateGoalWithSubjects_ListIsNull_ReturnsBadRequest()
    {
        // Arrange
        var createGoalRequest = new CreateGoalRequest
        {
            SubjectIds = null,
            GoalTime = 1.5
        };
    
        // Act
        var response = await _client.PostAsJsonAsync("/goals/create", createGoalRequest);
    
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        var responseData = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        Assert.NotNull(responseData);
    }

    
    [Fact]
    public async Task UpdateStudyTime_ValidRequest_ReturnsOkResult()
    {
        // Arrange
        var request = new StudyTimeUpdateRequest
        {
            SubjectId = "test-subject-id-1",
            ElapsedHours = 3.5
        };

        // Act
        var response = await _client.PostAsJsonAsync("goals/update-study-time", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var responseData = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        Assert.NotNull(responseData);
    }
    
    [Fact]
    public async Task UpdateStudyTime_InvalidRequest_ReturnsOkResult()
    {
        // Arrange
        var request = new StudyTimeUpdateRequest
        {
            SubjectId = "non-existent-subject-id",
            ElapsedHours = 3.5
        };

        // Act
        var response = await _client.PostAsJsonAsync("goals/update-study-time", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        var responseData = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        Assert.NotNull(responseData);
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
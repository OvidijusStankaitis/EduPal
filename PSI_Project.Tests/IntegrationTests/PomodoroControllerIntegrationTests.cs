using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using PSI_Project.Requests;

namespace PSI_Project.Tests.IntegrationTests;

public class PomodoroControllerIntegrationTests : IDisposable
{
    private readonly HttpClient _client;
    private readonly TestingWebAppFactory _factory;
    
    public PomodoroControllerIntegrationTests()
    {
        _factory = new TestingWebAppFactory();
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task StartTimer_GivenValidRequest_ReturnsOk()
    {
        // Arrange
        var startRequest = new StartTimerRequest
        {
            UserEmail = "user@example.com",
            Intensity = "Low"
        };

        // Act
        var response = await _client.PostAsync("/pomodoro/start-timer", JsonContent.Create(startRequest));

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task StopTimer_GivenValidRequest_ReturnsOk()
    {
        // Arrange
        var stopRequest = new StopTimerRequest
        {
            UserEmail = "user@example.com"
        };

        // Act
        var response = await _client.PostAsync("/pomodoro/stop-timer", JsonContent.Create(stopRequest));

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetTimerState_GivenUserEmail_ReturnsTimerState()
    {
        // Arrange
        string userEmail = "user@example.com";

        // Act
        var response = await _client.GetAsync($"/pomodoro/get-timer-state?userEmail={userEmail}");
        var responseContent = await response.Content.ReadAsStringAsync();
        var timerState = JsonSerializer.Deserialize<dynamic>(responseContent);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(timerState);
        // Additional assertions can be made based on the expected structure of the timer state
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}

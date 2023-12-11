using Moq;
using PSI_Project.Models;
using PSI_Project.Services;

namespace PSI_Project.Tests.UnitTests;

public class PomodoroServiceTests
{
    [Fact]
    public void StartTimer_ArgumentsAreValid_StateIsActive()
    {
        // Arrange
        string userId = "user-id";
        PomodoroService pomodoroService = new PomodoroService();
        
        // Act
        pomodoroService.StartTimer(userId, "Low");
        var result = pomodoroService.GetTimerState(userId);

        // Assert  
        Assert.NotEqual("Inactive", result.Mode);
        Assert.True(result.IsActive);
    }
    
    [Fact]
    public void StartTimer_IntensityIsInvalid_StateIsActive()
    {
        // Arrange
        string userId = "user-id";
        PomodoroService pomodoroService = new PomodoroService();
        
        // Act
        var result = Assert.Throws<ArgumentException>(() => pomodoroService.StartTimer(userId, "non-existent-intensity"));

        // Assert  
        Assert.Equal("Invalid intensity level.", result.Message);
    }
    
    [Fact]
    public void StopTimer_ArgumentsAreValid_StateIsInactive()
    {
        // Arrange
        string userId = "user-id";
        PomodoroService pomodoroService = new PomodoroService();
        
        // Act
        pomodoroService.StartTimer(userId, "Low");
        pomodoroService.StopTimer(userId);
        var result = pomodoroService.GetTimerState(userId);

        // Assert  
        Assert.Equal("Inactive", result.Mode);
        Assert.False(result.IsActive);  
    }
}
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using PSI_Project.Controllers;
using PSI_Project.DTO;
using PSI_Project.Models;
using PSI_Project.Services;
using PSI_Project.Tests.IntegrationTests.Configuration;

namespace PSI_Project.Tests.IntegrationTests;

public class CommentControllerTests
{
    private readonly HttpClient _client;
    private readonly TestingWebAppFactory _factory;

    private User _user;
    
    public CommentControllerTests()
    {
        _factory = new TestingWebAppFactory();
        _client = _factory.CreateClient();
        
        // Setting up logged in user
        _user = new User("test1@test.test", "testPassword1", "testName", "testSurname")
        {
            Id = "test-user-id-1"
        };

        using var scope = _factory.Services.CreateScope();
        TestUserAuthService? testAuthService = scope.ServiceProvider.GetRequiredService<IUserAuthService>() as TestUserAuthService;
        testAuthService?.SetAuthenticatedUser(_user);
    }

    [Fact]
    public async Task GetCommentsForUser_GetExistingTopicId_ReturnsOkAndListOfComment()
    {
        string topicId = "existingTopicId";
    
        var loggerMock = new Mock<ILogger<CommentController>>();

        var userAuthServiceMock = new Mock<IUserAuthService>();
        userAuthServiceMock.Setup(service => service.GetUser(It.IsAny<HttpContext>()))
            .ReturnsAsync(_user);
        
        var chatServiceMock = new Mock<ChatService>();
        chatServiceMock.Setup(service => service.GetMessagesForUser(It.IsAny<User>(), topicId))
            .Returns(new List<CommentDTO> { new CommentDTO("id1", "messageTest1", DateTime.Now, true) });

        var commentController = new CommentController(loggerMock.Object, chatServiceMock.Object, userAuthServiceMock.Object);

        // Act
        var response = await commentController.GetCommentsForUser(topicId) as OkObjectResult;
        var comments = response?.Value as List<CommentDTO>;

        // Assert
        Assert.NotNull(response);
        Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
        Assert.Single(comments);
        Assert.Equal("id1",comments[0].Id);
        Assert.Equal("messageTest1", comments[0].Content);
        Assert.True(comments[0].IsFromCurrentUser);
    }
    
    [Fact]
    public async Task GetCommentsForUser_GetNonexistentTopicId_ReturnsOkAnEmptyList()
    {
        // Arrange
        string nonexistentTopicId = "nonexistentTopicId";

        // Act
        var response = await _client.GetAsync($"/comment/get/{nonexistentTopicId}");
        var responseString = await response.Content.ReadAsStringAsync();
        var comments = JsonConvert.DeserializeObject<IEnumerable<Comment>>(responseString);
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(comments);
    }
    
    [Fact]
    public async Task GetCommentsForUser_ExceptionOccurs_ReturnsBadRequest()
    {
        // Arrange
        string topicId = "existingTopicId";

        var loggerMock = new Mock<ILogger<CommentController>>();

        var userAuthServiceMock = new Mock<IUserAuthService>();
        userAuthServiceMock.Setup(service => service.GetUser(It.IsAny<HttpContext>()))
            .ThrowsAsync(new Exception("Simulating GetUser exception")); // Simulate an exception in GetUser

        var chatServiceMock = new Mock<ChatService>();

        var commentController = new CommentController(loggerMock.Object, chatServiceMock.Object, userAuthServiceMock.Object);

        // Act
        var response = await commentController.GetCommentsForUser(topicId) as ObjectResult;

        // Assert
        Assert.NotNull(response);
        Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
        Assert.Equal("An error occurred while getting all topic comments", response.Value);
    }


    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}
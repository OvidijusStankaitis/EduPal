using Microsoft.Extensions.Logging;
using Moq;
using PSI_Project.Data;
using PSI_Project.Exceptions;
using PSI_Project.Models;
using PSI_Project.Repositories;
using PSI_Project.Services;

namespace PSI_Project.Tests.UnitTests;

public class ChatServiceTests
{
    [Fact]
    public async Task GetMessagesForUser_NoMessages_ReturnsEmptyList()
    {
        // Arrange
        Mock mockDbContext = new Mock<EduPalDatabaseContext>();
        Mock<CommentRepository> commentRepositoryMock = new Mock<CommentRepository>(mockDbContext.Object);
        Mock<ILogger<ChatService>> loggerMock = new Mock<ILogger<ChatService>>();

        ChatService chatService = new ChatService(commentRepositoryMock.Object, loggerMock.Object);

        User user = new User("user-email", "user-password", "user-name", "user-surname")
        {
            Id = "test-user-id"
        };
        string topicId = "test-topic-id";
        
        commentRepositoryMock.Setup(repo => repo.GetAll()).Returns(new List<Comment>());

        // Act
        var result = chatService.GetMessagesForUser(user, topicId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
    
    [Fact]
    public async Task GetMessagesForUser_MessagesExist_ReturnsMessageList()
    {
        // Arrange
        Mock mockDbContext = new Mock<EduPalDatabaseContext>();
        Mock<CommentRepository> commentRepositoryMock = new Mock<CommentRepository>(mockDbContext.Object);
        Mock<ILogger<ChatService>> loggerMock = new Mock<ILogger<ChatService>>();

        ChatService chatService = new ChatService(commentRepositoryMock.Object, loggerMock.Object);

        User user = new User("user-email", "user-password", "user-name", "user-surname")
        {
            Id = "test-user-id"
        };
        string topicId = "test-topic-id";

        var comments = new List<Comment>
        {
            new Comment(user.Id, topicId, "Message 1") { Timestamp = DateTime.UtcNow.AddSeconds(1) },
            new Comment(user.Id, topicId, "Message 2") { Timestamp = DateTime.UtcNow.AddSeconds(2) },
            new Comment(user.Id, topicId, "Message 3") { Timestamp = DateTime.UtcNow.AddSeconds(3) }
        };

        commentRepositoryMock.Setup(repo => repo.GetAll()).Returns(comments);

        // Act
        var result = chatService.GetMessagesForUser(user, topicId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
    }

    [Fact]
    public async Task DeleteMessage_MessageDoesNotExist_ThrowsObjectNotFoundException()
    {
        // Arrange
        Mock mockDbContext = new Mock<EduPalDatabaseContext>();
        Mock<CommentRepository> commentRepositoryMock = new Mock<CommentRepository>(mockDbContext.Object);
        Mock<ILogger<ChatService>> loggerMock = new Mock<ILogger<ChatService>>();

        ChatService chatService = new ChatService(commentRepositoryMock.Object, loggerMock.Object);

        string userId = "test-user-id";
        string topicId = "test-topic-id";
        Comment message = new Comment(userId, topicId, "test-message-content")
        {
            Id = "test-message-id-0"
        };

        commentRepositoryMock.Setup(repo => repo.GetAsync(message.Id))
            .Throws(new ObjectNotFoundException("Couldn't get object with specified id"));
        
        // Act
        var exception = await Assert.ThrowsAsync<ObjectNotFoundException>(async () => await chatService.DeleteMessage(message.Id));

        // Assert
        Assert.Contains("Couldn't get object with specified id", exception.Message);
    }

    [Fact]
    public async Task DeleteMessage_MessageExist_ReturnsMessage()
    {
        // Arrange
        Mock mockDbContext = new Mock<EduPalDatabaseContext>();
        Mock<CommentRepository> commentRepositoryMock = new Mock<CommentRepository>(mockDbContext.Object);
        Mock<ILogger<ChatService>> loggerMock = new Mock<ILogger<ChatService>>();

        ChatService chatService = new ChatService(commentRepositoryMock.Object, loggerMock.Object);

        string userId = "test-user-id";
        string topicId = "test-topic-id";
        Comment toBeDeletedComment = new Comment(userId, topicId, "test-message-content")
        {
            Id = "test-message-id-0"
        };

        commentRepositoryMock.Setup(repo => repo.GetAsync(toBeDeletedComment.Id))
            .ReturnsAsync(toBeDeletedComment);
        
        // Act
        Comment deletedComment = await chatService.DeleteMessage(toBeDeletedComment.Id);

        // Assert
        Assert.Equal(toBeDeletedComment.Id, deletedComment.Id);
        Assert.Equal(toBeDeletedComment.TopicId, deletedComment.TopicId);
        Assert.Equal(toBeDeletedComment.UserId, deletedComment.UserId);
        Assert.Equal(toBeDeletedComment.Content, deletedComment.Content);
    }
}
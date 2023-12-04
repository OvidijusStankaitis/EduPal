using System.Linq.Expressions;
using Moq;
using PSI_Project.Data;
using PSI_Project.Exceptions;
using PSI_Project.Models;
using PSI_Project.Repositories;

namespace PSI_Project.Tests.UnitTests;

public class CommentRepositoryTests
{
    [Fact]
    public async Task GetAllCommentsOfTopicAsync_GetsEmptyList_ReturnsEmptyList()
    {
        // Arrange
        var topic = new Topic("testTopic", new Subject("testSubject"), KnowledgeLevel.Average);
        
        var mockDbContext = new Mock<EduPalDatabaseContext>();
        var mockRepository = new Mock<CommentRepository>(mockDbContext.Object);
        
        mockRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<Comment, bool>>>())).ReturnsAsync(new List<Comment>());
        
        var commentRepository = mockRepository.Object;
        
        // Act
        var result = await commentRepository.GetAllCommentsOfTopicAsync(topic.Id);
        
        // Assert
        Assert.Empty(result);
    }
    
    [Fact]
    public async Task GetAllCommentsOfTopicAsync_GetsListOfOneTopicsOfOneSubject_ReturnsListWithOneElement()
    {
        // Arrange
        var topic = new Topic("testTopic", new Subject("testSubject"), KnowledgeLevel.Average);
        var user = new User("TestUserName", "TestUserSurname", "testmail@test.test", "userPassword");
        var comment = new Comment(user.Id, topic.Id, "testComment");
        
        var mockDbContext = new Mock<EduPalDatabaseContext>();
        var mockRepository = new Mock<Repositories.CommentRepository>(mockDbContext.Object);
        
        mockRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<Comment, bool>>>())).ReturnsAsync(new List<Comment>{comment});
        
        var commentRepository = mockRepository.Object;
        
        // Act
        var result = await commentRepository.GetAllCommentsOfTopicAsync(topic.Id);
        
        // Assert
        Assert.Single(result);
        Assert.Collection(result, commentOfList1 => Assert.Equal(comment.Id, commentOfList1.Id));
    }
    
    [Fact]
    public async Task GetAllCommentsOfTopicAsync_GetsListOfTwoTopicsOfOneSubject_ReturnsListWithTwoElements()
    {
        // Arrange
        var topic = new Topic("testTopic", new Subject("testSubject"), KnowledgeLevel.Average);
        var user = new User("TestUserName", "TestUserSurname", "testmail@test.test", "userPassword");
        var comment1 = new Comment(user.Id, topic.Id, "testComment1");
        var comment2 = new Comment(user.Id, topic.Id, "testComment2");
        
        var mockDbContext = new Mock<EduPalDatabaseContext>();
        var mockRepository = new Mock<Repositories.CommentRepository>(mockDbContext.Object);
        
        mockRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<Comment, bool>>>())).ReturnsAsync(new List<Comment>{comment1, comment2});
        
        var commentRepository = mockRepository.Object;
        
        // Act
        var result = await commentRepository.GetAllCommentsOfTopicAsync(topic.Id);
        
        // Assert
        Assert.Collection(result,
            commentOfList1 => Assert.Equal(comment1.Id, commentOfList1.Id),
            commentOfList2 => Assert.Equal(comment2.Id, commentOfList2.Id));
    }
    
    [Fact]
    public async Task RemoveAsync_CommentExists_ReturnsTrue()
    {
        // Arrange
        var user = new User("TestUserName", "TestUserSurname", "testmail@test.test", "userPassword");
        var topic = new Topic("testTopic", new Subject("testSubject"), KnowledgeLevel.Average);
        var comment = new Comment(user.Id, topic.Id, "testComment");

        var mockDbContext = new Mock<EduPalDatabaseContext>();
        var mockRepository = new Mock<Repositories.CommentRepository>(mockDbContext.Object);
        
        mockRepository.Setup(repo => repo.GetAsync(comment.Id)).ReturnsAsync(comment);
        mockRepository.Setup(repo => repo.Remove(comment)).Returns(1);

        var commentRepository = mockRepository.Object;
    
        // Act
        var result = await commentRepository.RemoveAsync(comment.Id);
        
        // Assert
        Assert.True(result);
    }
    
    [Fact] 
    public async Task RemoveAsync_CommentDoesNotExist_ThrowsObjectNotFoundException()
    {
        // Arrange
        var user = new User("TestUserName", "TestUserSurname", "testmail@test.test", "userPassword");
        var topic = new Topic("nonexistentTestTopic", new Subject("nonexistentTestSubject"), KnowledgeLevel.Average);
        var comment = new Comment(user.Id, topic.Id, "nonexistentTestComment");
        
        var mockDbContext = new Mock<EduPalDatabaseContext>();
        var mockRepository = new Mock<Repositories.CommentRepository>(mockDbContext.Object);
        
        mockRepository.Setup(repo => repo.GetAsync(comment.Id)).ThrowsAsync(new ObjectNotFoundException());

        var commentRepository = mockRepository.Object;
        
        // Act
        var exception = await Assert.ThrowsAsync<ObjectNotFoundException>(async () => await commentRepository.RemoveAsync(comment.Id));

        // Assert
        Assert.Contains("Exception of type 'PSI_Project.Exceptions", exception.Message);
    }
    
    [Fact]
    public async Task GetItemByIdAsync_GetsIdOfNonexistentComment_ReturnsNull()
    {
        // Arrange
        var mockDbContext = new Mock<EduPalDatabaseContext>();
        var mockRepository = new Mock<Repositories.CommentRepository>(mockDbContext.Object);
        
        mockRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<Comment, bool>>>()))
            .ReturnsAsync(new List<Comment>());        
        var commentRepository = mockRepository.Object;
        
        // Act
        var result = await commentRepository.GetItemByIdAsync("nonexistentId");
        
        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public async Task GetItemByIdAsync_GetsIdOfComment_ReturnsComment()
    {
        // Arrange
        var user = new User("TestUserName", "TestUserSurname", "testmail@test.test", "userPassword");
        var topic = new Topic("testTopic", new Subject("testSubject"), KnowledgeLevel.Average);
        var comment = new Comment(user.Id, topic.Id, "testComment");
        
        var mockDbContext = new Mock<EduPalDatabaseContext>();
        var mockRepository = new Mock<Repositories.CommentRepository>(mockDbContext.Object);
        
        mockRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<Comment, bool>>>()))
            .ReturnsAsync(new List<Comment>{comment});        
        var commentRepository = mockRepository.Object;
        
        // Act
        var result = await commentRepository.GetItemByIdAsync(comment.Id);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(comment.Id, result.Id);
        Assert.Equal(comment.UserId, result.UserId);
        Assert.Equal(comment.TopicId, result.TopicId);
        Assert.Equal(comment.CommentText, result.CommentText);
    }
    
}
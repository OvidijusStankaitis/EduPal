using System.Linq.Expressions;
using Moq;
using PSI_Project.Data;
using PSI_Project.Exceptions;
using PSI_Project.Models;
using PSI_Project.Repositories;

namespace PSI_Project.Tests.UnitTests;

public class TopicRepositoryUnitTestsMoq
{
    [Fact]
    public async Task GetTopicsListBySubjectIdAsync_GetsListOfOneTopicsOfOneSubject_ReturnsListWithOneElements()
    {
        // Arrange
        var subject = new Subject("testSubject");
        var topics = new List<Topic>
        {
            new Topic("topic1", subject)
        };
        
        var mockDbContext = new Mock<EduPalDatabaseContext>();
        var mockRepository = new Mock<TopicRepository>(mockDbContext.Object);
        
        mockRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<Topic, bool>>>())).ReturnsAsync(topics);
        
        var topicRepository = mockRepository.Object;

        // Act
        var result = await topicRepository.GetTopicsListBySubjectIdAsync(subject.Id);
        
        // Assert
        Assert.Single(result);
        Assert.Collection(result, topicList1 => Assert.Equal(topics[0].Name, topicList1.Name));
    }

    [Fact]
    public async Task GetTopicsListBySubjectIdAsync_GetsListOfTwoTopicsOfOneSubject_ReturnsListWithTwoElements()
    {
        // Arrange
        var subject = new Subject("testSubject");
        var topics = new List<Topic>
        {
            new Topic("topic1", subject),
            new Topic("topic2", subject)
        };

        var mockDbContext = new Mock<EduPalDatabaseContext>();
        var mockRepository = new Mock<TopicRepository>(mockDbContext.Object);

        mockRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<Topic, bool>>>())).ReturnsAsync(topics);

        var topicRepository = mockRepository.Object;

        // Act
        var result = await topicRepository.GetTopicsListBySubjectIdAsync(subject.Id);

        // Assert
        Assert.Collection(result, 
            topicList1 => Assert.Equal(topics[0].Id, topicList1.Id), 
            topicList2 => Assert.Equal(topics[1].Name, topicList2.Name));
    }

    [Fact]
    public async Task RemoveAsync_TopicExists_ReturnsTrue()
    {
        // Arrange
        var topic = new Topic("removeTestName", new Subject("testSubject"), KnowledgeLevel.Average);

        var mockDbContext = new Mock<EduPalDatabaseContext>();
        var mockRepository = new Mock<TopicRepository>(mockDbContext.Object);
        
        mockRepository.Setup(repo => repo.GetAsync(topic.Id)).ReturnsAsync(topic);
        mockRepository.Setup(repo => repo.Remove(topic)).Returns(1);
        
        var topicRepository = mockRepository.Object;

        // Act
        var result = await topicRepository.RemoveAsync(topic.Id);

        // Assert
        Assert.True(result);
    }
    
    [Fact]
    public async Task RemoveAsync_TopicDoesNotExist_ThrowsObjectNotFoundException()
    {
        // Arrange
        var topic = new Topic("nonexistentTestRemove", new Subject("nonexistentTestRemoveSubject"), KnowledgeLevel.Average);
        
        var mockDbContext = new Mock<EduPalDatabaseContext>();
        var mockRepository = new Mock<TopicRepository>(mockDbContext.Object);

        mockRepository.Setup(repo => repo.GetAsync(topic.Id)).ThrowsAsync(new ObjectNotFoundException());
        mockRepository.Setup(repo => repo.Remove(topic)).Returns(1);
       
        var topicRepository = mockRepository.Object;

        // Act
        var exception = await Assert.ThrowsAsync<ObjectNotFoundException>(async () => await topicRepository.RemoveAsync(topic.Id));

        // Assert
        Assert.Contains("Exception of type 'PSI_Project.Exceptions", exception.Message);
    }
}
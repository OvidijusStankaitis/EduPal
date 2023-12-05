using System.Linq.Expressions;
using Microsoft.Extensions.FileProviders;
using Moq;
using PSI_Project.Data;
using PSI_Project.Exceptions;
using PSI_Project.Models;
using PSI_Project.Repositories;
using PSI_Project.Repositories.For_tests;

namespace PSI_Project.Tests.UnitTests;

public class ConspectusRepositoryUnitTests
{
    [Fact]
    public async Task GetConspectusListByTopicIdAsync_GetsExistingTopicWithTwoConspectuses_ReturnsListOfConspectuses()
    {
        // Arrange
        var topic = new Topic("testTopic", new Subject("testSubject"), KnowledgeLevel.Average);
        var conspectus1 = new Conspectus{Id = "id1", Name = "test1.pdf", Path = "somePath1", Topic = topic, Rating = 2555};
        var conspectus2 = new Conspectus{Id = "id2", Name = "test2.pdf", Path = "somePath2", Topic = topic, Rating = -258484};
        
        var mockDbContext = new Mock<EduPalDatabaseContext>();
        var mockFileOperations = new Mock<IFileOperations>();
        var mockRepository = new Mock<ConspectusRepository>(mockDbContext.Object, mockFileOperations.Object);

        mockRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<Conspectus, bool>>>())).ReturnsAsync(new List<Conspectus>{conspectus1,conspectus2});
        
        var conspectusRepository = mockRepository.Object;
        
        // Act
        var result = await conspectusRepository.GetConspectusListByTopicIdAsync(topic.Id);
        
        // Assert
        Assert.Equal(2, result.Count());
        Assert.Equal("id1", result.ToList()[0].Id);
        Assert.Equal(-258484, result.ToList()[1].Rating);
    }
    
    [Fact]
    public async Task GetConspectusListByTopicIdAsync_GetsExistingTopicWithNoConspectuses_ReturnsEmptyListOfConspectuses()
    {
        // Arrange
        var topic = new Topic("testTopic", new Subject("testSubject"), KnowledgeLevel.Average);
        
        var mockDbContext = new Mock<EduPalDatabaseContext>();
        var mockFileOperations = new Mock<IFileOperations>();
        var mockRepository = new Mock<ConspectusRepository>(mockDbContext.Object, mockFileOperations.Object);

        mockRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<Conspectus, bool>>>())).ReturnsAsync(new List<Conspectus>());
        
        var conspectusRepository = mockRepository.Object;
        
        // Act
        var result = await conspectusRepository.GetConspectusListByTopicIdAsync(topic.Id);
        
        // Assert
        Assert.Empty(result);
    }
    
    [Fact]
     public async Task GetConspectusListByTopicIdAsync_GetsNonexistentTopicWithNoConspectuses_ReturnsEmptyList()
     {
         // Arrange
         var mockDbContext = new Mock<EduPalDatabaseContext>();
         var mockFileOperations = new Mock<IFileOperations>();
         var mockRepository = new Mock<ConspectusRepository>(mockDbContext.Object, mockFileOperations.Object);

         mockRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<Conspectus, bool>>>())).ReturnsAsync(new List<Conspectus>());
        
         var conspectusRepository = mockRepository.Object;
        
         // Act
         var result = await conspectusRepository.GetConspectusListByTopicIdAsync("nonexistentTopicId");
        
         // Assert
         Assert.Empty(result);
     }

     [Fact]
     public async Task ChangeRatingAsync_GetsExistingConspectusId_ReturnsConspectusWithIncreasedRating()
     {
         // Arrange
         var topic = new Topic("testTopic", new Subject("testSubject"), KnowledgeLevel.Average);
         var conspectus = new Conspectus{Id = "id1", Name = "test.pdf", Path = "somePath1", Topic = topic, Rating = 10};

         var mockDbContext = new Mock<EduPalDatabaseContext>();
         var mockFileOperations = new Mock<IFileOperations>();
         var mockRepository = new Mock<ConspectusRepository>(mockDbContext.Object, mockFileOperations.Object);

         mockRepository.Setup(repo => repo.GetAsync(conspectus.Id)).ReturnsAsync(conspectus);
        
         var conspectusRepository = mockRepository.Object;
        
         // Act
         var result = await conspectusRepository.ChangeRatingAsync(conspectus.Id, true);
        
         // Assert
         Assert.Equal("id1", result.Id);
         Assert.Equal("test.pdf", result.Name);
         Assert.Equal("somePath1", result.Path);
         Assert.Equal(topic.Id, result.Topic.Id);
         Assert.Equal(11, result.Rating);
     }
     
     [Fact]
     public async Task ChangeRatingAsync_GetsExistingConspectusId_ReturnsConspectusWithDecreasedRating()
     {
         // Arrange
         var topic = new Topic("testTopic", new Subject("testSubject"), KnowledgeLevel.Average);
         var conspectus = new Conspectus{Id = "id1", Name = "test.pdf", Path = "somePath1", Topic = topic, Rating = 10};

         var mockDbContext = new Mock<EduPalDatabaseContext>();
         var mockFileOperations = new Mock<IFileOperations>();
         var mockRepository = new Mock<ConspectusRepository>(mockDbContext.Object, mockFileOperations.Object);

         mockRepository.Setup(repo => repo.GetAsync(conspectus.Id)).ReturnsAsync(conspectus);
        
         var conspectusRepository = mockRepository.Object;
        
         // Act
         var result = await conspectusRepository.ChangeRatingAsync(conspectus.Id, false);
        
         // Assert
         Assert.Equal("id1", result.Id);
         Assert.Equal("test.pdf", result.Name);
         Assert.Equal("somePath1", result.Path);
         Assert.Equal(topic.Id, result.Topic.Id);
         Assert.Equal(9, result.Rating);
     }
     
     [Fact]
     public async Task RemoveAsync_GetsExistingConspectusWithExistingPath_ReturnsString()
     {
         // Arrange
         var topic = new Topic("testTopic", new Subject("testSubject"), KnowledgeLevel.Average);
         var conspectus = new Conspectus{Id = "id1", Name = "test.pdf", Path = "existing file path", Topic = topic, Rating = 10};
         
         var mockDbContext = new Mock<EduPalDatabaseContext>();
         var mockFileOperations = new Mock<IFileOperations>();
         var mockRepository = new Mock<ConspectusRepository>(mockDbContext.Object, mockFileOperations.Object);
         
         mockRepository.Setup(repo => repo.GetAsync(conspectus.Id)).ReturnsAsync(conspectus);
         mockRepository.Setup(repo => repo.Remove(conspectus)).Returns(1);
         mockFileOperations.Setup(f => f.Exists(It.IsAny<string>())).Returns(true);
         
         var conspectusRepository = mockRepository.Object;
        
         // Act
         var result = await conspectusRepository.RemoveAsync(conspectus.Id);
        
         // Assert
         Assert.Equal($"File deleted: {conspectus.Path}",result);
         mockFileOperations.Verify(f => f.Delete(It.IsAny<string>()), Times.Once);
     }
     
     [Fact]
     public async Task RemoveAsync_GetsConspectusWithNonexistentConspectusPath_ReturnsString()
     {
         // Arrange
         var topic = new Topic("testTopic", new Subject("testSubject"), KnowledgeLevel.Average);
         var conspectus = new Conspectus{Id = "id1", Name = "test.pdf", Path = "nonexistent file path", Topic = topic, Rating = 10};
         
         var mockDbContext = new Mock<EduPalDatabaseContext>();
         var mockFileOperations = new Mock<IFileOperations>();
         var mockRepository = new Mock<ConspectusRepository>(mockDbContext.Object, mockFileOperations.Object);
         
         mockRepository.Setup(repo => repo.GetAsync(conspectus.Id)).ReturnsAsync(conspectus);
         mockRepository.Setup(repo => repo.Remove(conspectus)).Returns(1);
         mockFileOperations.Setup(f => f.Exists(It.IsAny<string>())).Returns(false);
         
         var conspectusRepository = mockRepository.Object;
        
         // Act
         var result = await conspectusRepository.RemoveAsync(conspectus.Id);
        
         // Assert
         Assert.Equal($"File not found: {conspectus.Path}",result);
         mockFileOperations.Verify(f => f.Delete(It.IsAny<string>()), Times.Never);
     }
     
   
     
}
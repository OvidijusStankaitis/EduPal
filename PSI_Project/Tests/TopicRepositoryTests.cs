using Microsoft.EntityFrameworkCore;
using PSI_Project.Data;
using PSI_Project.Models;
using PSI_Project.Repositories;
using Xunit;

namespace PSI_Project.Tests;

public class TopicRepositoryTests
{
    private readonly TopicRepository _topicRepository;
    private readonly SubjectRepository _subjectRepository;
    private readonly DbContextOptions<EduPalDatabaseContext> _options;
    private readonly EduPalDatabaseContext _context;
    
    public TopicRepositoryTests()
    {
        _options = GetInMemoryDatabaseOptions("TestDB"); 
        _context = new EduPalDatabaseContext(_options); //should it be in using? 
        _topicRepository = new TopicRepository(_context);
        _subjectRepository = new SubjectRepository(_context);
    }
    
    private DbContextOptions<EduPalDatabaseContext> GetInMemoryDatabaseOptions(string dbName)
    {
        return new DbContextOptionsBuilder<EduPalDatabaseContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
    }

    [Fact]
    public void GetTopicsListBySubjectId_GetsListOfOneTopicsOfOneSubject_ReturnsListWithOneElements()
    {
        // Arrange
        var subject = new Subject("testSubject");
        _subjectRepository.Add(subject);

        var topic1 = new Topic("topic1", subject);
        _topicRepository.Add(topic1);
        _context.SaveChanges();
        
        // Act
        var result = _topicRepository.GetTopicsListBySubjectId(subject.Id);
        _subjectRepository.RemoveSubject(subject.Id); // removing subject so DB would be empty for the GetSubjectsList_ListIsEmpty_ReturnsNull() method
        _topicRepository.Remove(topic1.Id); // removing topic so DB would be empty for the GetSubjectsList_ListIsEmpty_ReturnsNull() method
        
        // Assert
        Assert.Single(result);
        Assert.Collection(result, topicOfList1 => Assert.Contains(topic1.Name, topicOfList1.Name));
    }
    
    [Fact]
    public void GetTopicsListBySubjectId_GetsListOfTwoTopicsOfOneSubject_ReturnsListWithTwoElements()
    {
        // Arrange
        var subject = new Subject("testSubject");
        _subjectRepository.Add(subject);

        var topic1 = new Topic("topic1", subject);
        var topic2 = new Topic("topic2", subject);
        _topicRepository.Add(topic1);
        _topicRepository.Add(topic2);
        _context.SaveChanges();
        
        // Act
        var result = _topicRepository.GetTopicsListBySubjectId(subject.Id);
        _subjectRepository.RemoveSubject(subject.Id); // removing subject so DB would be empty for the GetSubjectsList_ListIsEmpty_ReturnsNull() method
        _topicRepository.Remove(topic1.Id); // removing topic so DB would be empty for the GetSubjectsList_ListIsEmpty_ReturnsNull() method
        _topicRepository.Remove(topic2.Id); // removing topic so DB would be empty for the GetSubjectsList_ListIsEmpty_ReturnsNull() method
       
        // Assert
        Assert.NotEmpty(result);
        Assert.Collection(result, 
            topicOfList1 => Assert.Contains(topic1.Name, topicOfList1.Name), 
            topicOfList2 => Assert.Contains(topic2.Name, topicOfList2.Name));
    }
    
    [Fact]
    public void Remove_UserExists_ReturnsTrue()
    {
        // Arrange
        var topic = new Topic("removeTestName", new Subject("testSubject"), KnowledgeLevel.Average);
        _topicRepository.Add(topic);
        _context.SaveChanges();
        
        // Act
        var result = _topicRepository.Remove(topic.Id);

        // Assert
        Assert.True(result);
    }
    
    [Fact]
    public void Remove_UserDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var topic = new Topic("nonexistentTestRemove", new Subject("nonexistentTestRemoveSubject"), KnowledgeLevel.Average);
        
        // Act
        var result = _topicRepository.Remove(topic.Id);

        // Assert
        Assert.False(result);
    }
    
}
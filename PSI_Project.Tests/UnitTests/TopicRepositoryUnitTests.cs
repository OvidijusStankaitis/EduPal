using Microsoft.EntityFrameworkCore;
using PSI_Project.Data;
using PSI_Project.Exceptions;
using PSI_Project.Models;
using PSI_Project.Repositories;

namespace PSI_Project.Tests.UnitTests;

public class TopicRepositoryUnitTests
{
    private readonly TopicRepository _topicRepository;
    private readonly SubjectRepository _subjectRepository;
    private readonly DbContextOptions<EduPalDatabaseContext> _options;
    private readonly EduPalDatabaseContext _context;
    
    public TopicRepositoryUnitTests()
    {
        _options = GetInMemoryDatabaseOptions("TestTopicDB"); 
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
    public async Task GetTopicsListBySubjectId_GetsListOfOneTopicsOfOneSubject_ReturnsListWithOneElements()
    {
        // Arrange
        var subject = new Subject("testSubject");
        _subjectRepository.Add(subject);

        var topic1 = new Topic("topic1", subject);
        _topicRepository.Add(topic1);
        _context.SaveChanges();
        
        // Act
        var result = await _topicRepository.GetTopicsListBySubjectIdAsync(subject.Id);
        //_topicRepository.Remove(topic1.Id); // removing topic so DB would be empty for the GetSubjectsList_ListIsEmpty_ReturnsNull() method
        //_subjectRepository.RemoveSubject(subject.Id); // removing subject so DB would be empty for the GetSubjectsList_ListIsEmpty_ReturnsNull() method
        
        // Assert
        Assert.Single(result);
        Assert.Collection(result, topicOfList1 => Assert.Equal(topic1.Name, topicOfList1.Name));
    }
    
    [Fact]
    public async Task GetTopicsListBySubjectId_GetsListOfTwoTopicsOfOneSubject_ReturnsListWithTwoElements()
    {
        // Arrange
        var subject = new Subject("testSubject");
        _subjectRepository.Add(subject);
        _context.SaveChanges();
        
        var topic1 = new Topic("topic1", subject);
        var topic2 = new Topic("topic2", subject);
        _topicRepository.Add(topic1);
        _topicRepository.Add(topic2);
        _context.SaveChanges();
        
        // Act
        var result = await _topicRepository.GetTopicsListBySubjectIdAsync(subject.Id);
       // _topicRepository.Remove(topic1.Id); // removing topic so DB would be empty for the GetSubjectsList_ListIsEmpty_ReturnsNull() method
       // _topicRepository.Remove(topic2.Id); // removing topic so DB would be empty for the GetSubjectsList_ListIsEmpty_ReturnsNull() method
       // _subjectRepository.RemoveSubject(subject.Id); // removing subject so DB would be empty for the GetSubjectsList_ListIsEmpty_ReturnsNull() method
       
        // Assert
        Assert.Collection(result, 
            topicOfList1 => Assert.Equal(topic1.Id, topicOfList1.Id), 
            topicOfList2 => Assert.Equal(topic2.Name, topicOfList2.Name));
    }
    
    [Fact]
    public async Task Remove_TopicExists_ReturnsTrue()
    {
        // Arrange
        var subject = new Subject("testSubject");
        
        var topic = new Topic("removeTestName", subject, KnowledgeLevel.Average);
        _topicRepository.Add(topic);
        _subjectRepository.Add(subject);
        _context.SaveChanges();
        
        // Act
        var result = await _topicRepository.RemoveAsync(topic.Id);
        await _subjectRepository.RemoveSubjectAsync(subject.Id);

        // Assert
        Assert.True(result);
    }
    
    [Fact]
    public async Task Remove_TopicDoesNotExist_ThrowsException()
    {
        // Arrange
        var topic = new Topic("nonexistentTestRemove", new Subject("nonexistentTestRemoveSubject"), KnowledgeLevel.Average);
    
        // Act

        // Assert
        var exception = await Assert.ThrowsAsync<ObjectNotFoundException>(() => _topicRepository.RemoveAsync(topic.Id));
        Assert.Equal("Couldn't get object with specified id", exception.Message);
    }
}
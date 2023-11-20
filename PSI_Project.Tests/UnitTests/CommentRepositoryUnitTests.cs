using Microsoft.EntityFrameworkCore;
using PSI_Project.Data;
using PSI_Project.Exceptions;
using PSI_Project.Models;
using PSI_Project.Repositories;

namespace PSI_Project.Tests.UnitTests;

public class CommentRepositoryUnitTests
{
    private readonly CommentRepository _commentRepository;
    private readonly UserRepository _userRepository;
    private readonly TopicRepository _topicRepository;
    private readonly SubjectRepository _subjectRepository;
    private readonly DbContextOptions<EduPalDatabaseContext> _options;
    private readonly EduPalDatabaseContext _context;
    
    public CommentRepositoryUnitTests()
    {
        _options = GetInMemoryDatabaseOptions("TestCommentDB"); 
        _context = new EduPalDatabaseContext(_options); //should it be in using? 
        _commentRepository = new CommentRepository(_context);
        _topicRepository = new TopicRepository(_context);
        _subjectRepository = new SubjectRepository(_context);
        _userRepository = new UserRepository(_context);
    }
    
    private DbContextOptions<EduPalDatabaseContext> GetInMemoryDatabaseOptions(string dbName)
    {
        return new DbContextOptionsBuilder<EduPalDatabaseContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
    }
    
    [Fact]
    public void GetAllCommentsOfTopic_GetsEmptyList_ReturnsEmptyList()
    {
        // Arrange
        var subject = new Subject("testSubject");
        var topic = new Topic("testTopic", subject, KnowledgeLevel.Average);
        
        // Act
        var result = _commentRepository.GetAllCommentsOfTopicA(topic.Id);
        
        // Assert
        Assert.Empty(result);
    }
    
    [Fact]
    public void GetAllCommentsOfTopic_GetsListOfOneTopicsOfOneSubject_ReturnsListWithOneElements()
    {
        // Arrange
        var subject = new Subject("testSubject");
        var topic = new Topic("testTopic", subject, KnowledgeLevel.Average);
        var user = new User("TestUserName", "TestUserSurname", "testmail@test.test", "userPassword");
        
        _userRepository.Add(user);
        _subjectRepository.Add(subject);
        _topicRepository.Add(topic);

        var comment = new Comment(user.Id, topic.Id, "testComment");
        _commentRepository.Add(comment);
        _context.SaveChanges();
        
        // Act
        var result = _commentRepository.GetAllCommentsOfTopic(topic.Id);
        _commentRepository.Remove(comment.Id);
        _topicRepository.Remove(topic.Id); // removing topic so DB would be empty for the GetSubjectsList_ListIsEmpty_ReturnsNull() method
        _subjectRepository.RemoveSubject(subject.Id); // removing subject so DB would be empty for the GetSubjectsList_ListIsEmpty_ReturnsNull() method
        _userRepository.Remove(user);
        
        // Assert
        Assert.Single(result);
        Assert.Collection(result, commentOfList1 => Assert.Equal(comment.Id, commentOfList1.Id));
    }
    
    [Fact]
    public void GetAllCommentsOfTopic_GetsListOfTwoTopicsOfOneSubject_ReturnsListWithTwoElements()
    {
        // Arrange
        var user = new User("TestUserName", "TestUserSurname", "testmail@test.test", "userPassword");
        var subject = new Subject("testSubject");
        var topic = new Topic("testTopic", subject, KnowledgeLevel.Average);
        _userRepository.Add(user);
        _subjectRepository.Add(subject);
        _topicRepository.Add(topic);
    
        var comment1 = new Comment(user.Id, topic.Id, "testComment1");
        var comment2 = new Comment(user.Id, topic.Id, "testComment2");
        _commentRepository.Add(comment1);
        _commentRepository.Add(comment2);
        _context.SaveChanges();
        
        // Act
        var result = _commentRepository.GetAllCommentsOfTopic(topic.Id);
        _commentRepository.Remove(comment1.Id);
        _commentRepository.Remove(comment2.Id);
        _topicRepository.Remove(topic.Id); // removing topic so DB would be empty for the GetSubjectsList_ListIsEmpty_ReturnsNull() method
        _subjectRepository.RemoveSubject(subject.Id); // removing subject so DB would be empty for the GetSubjectsList_ListIsEmpty_ReturnsNull() method
        _userRepository.Remove(user);
    
        // Assert
        Assert.Collection(result,
            commentOfList1 => Assert.Equal(comment1.Id, commentOfList1.Id),
            commentOfList2 => Assert.Equal(comment2.Id, commentOfList2.Id));
    }
    
    [Fact]
    public void Remove_CommentExists_ReturnsTrue()
    {
        // Arrange
        var user = new User("TestUserName", "TestUserSurname", "testmail@test.test", "userPassword");
        var subject = new Subject("testSubject");
        var topic = new Topic("testTopic", subject, KnowledgeLevel.Average);
        _userRepository.Add(user);
        _subjectRepository.Add(subject);
        _topicRepository.Add(topic);
    
        var comment = new Comment(user.Id, topic.Id, "testComment");
        _commentRepository.Add(comment);
        _context.SaveChanges();
    
        // Act
        var result = _commentRepository.Remove(comment.Id);
        _topicRepository.Remove(topic.Id); // removing topic so DB would be empty for the GetSubjectsList_ListIsEmpty_ReturnsNull() method
        _subjectRepository.RemoveSubject(subject.Id); // removing subject so DB would be empty for the GetSubjectsList_ListIsEmpty_ReturnsNull() method
        _userRepository.Remove(user);
        
        // Assert
        Assert.True(result);
    }
    
    [Fact] 
    public void Remove_CommentDoesNotExist_ThrowsObjectNotFoundException()
    {
        // Arrange
        var user = new User("TestUserName", "TestUserSurname", "testmail@test.test", "userPassword");
        var subject = new Subject("nonexistentTestSubject");
        var topic = new Topic("nonexistentTestTopic", subject, KnowledgeLevel.Average);
        var comment = new Comment(user.Id, topic.Id, "nonexistentTestComment");
        
        // Act
        
        // Assert
        var exception = Assert.Throws<ObjectNotFoundException>(() => _commentRepository.Remove(comment.Id));
        Assert.Equal("Couldn't get object with specified id", exception.Message);
    }
}
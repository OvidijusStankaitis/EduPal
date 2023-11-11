using Microsoft.EntityFrameworkCore;
using PSI_Project.Data;
using PSI_Project.Models;
using PSI_Project.Repositories;

namespace PSI_Project.Tests.UnitTests;

public class SubjectRepositoryUnitTests
{
    private readonly SubjectRepository _subjectRepository;
    private readonly DbContextOptions<EduPalDatabaseContext> _options;
    private readonly EduPalDatabaseContext _context;
    
    public SubjectRepositoryUnitTests()
    {
        _options = GetInMemoryDatabaseOptions("TestSubjectDB"); 
        _context = new EduPalDatabaseContext(_options); //should it be in using? 
        _subjectRepository = new SubjectRepository(_context);
        
    }
    
    private DbContextOptions<EduPalDatabaseContext> GetInMemoryDatabaseOptions(string dbName)
    {
        return new DbContextOptionsBuilder<EduPalDatabaseContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
    }

    [Fact]
    public void GetSubjectsList_DBIsEmpty_ReturnsEmpty()
    {
        // Arrange
        
        // Act
        var result = _subjectRepository.GetSubjectsList();

        // Assert
        Assert.Empty(result);
    }
    
    [Fact]
    public void GetSubjectsList_DBContainsOneElement_ReturnsAListOfOneElement()
    {
        // Arrange
        var subject = new Subject("testSubject");
        _subjectRepository.Add(subject);
        _context.SaveChanges();
        
        // Act
        var result = _subjectRepository.GetSubjectsList();
        _subjectRepository.RemoveSubject(subject.Id); // removing subject so DB would be empty for the GetSubjectsList_ListIsEmpty_ReturnsNull() method
        
        // Assert
        Assert.Single(result);
        Assert.Collection(result, subject1 => Assert.Equal(subject.Name, subject1.Name));
    }
    
    [Fact]
    public void GetSubjectsList_DBContainsTwoElement_ReturnsAListOfTwoElements()
    {
        // Arrange
        var subject1 = new Subject("testSubject1");
        _subjectRepository.Add(subject1);
        _context.SaveChanges();
       
        var subject2 = new Subject("testSubject2");
        _subjectRepository.Add(subject2);
        _context.SaveChanges();
        
        // Act
        var result = _subjectRepository.GetSubjectsList();
        _subjectRepository.RemoveSubject(subject1.Id); // removing subject so DB would be empty for the GetSubjectsList_ListIsEmpty_ReturnsNull() method
        _subjectRepository.RemoveSubject(subject2.Id); // removing subject so DB would be empty for the GetSubjectsList_ListIsEmpty_ReturnsNull() method
        
        // Assert
        Assert.NotEmpty(result);
        Assert.Collection(result, 
            subjectOfList1 => Assert.Equal(subject1.Name, subjectOfList1.Name),
            subjectOfList2 => Assert.Equal(subject2.Name, subjectOfList2.Name));
    }
    
    [Fact]
    public void RemoveSubject_UserExists_ReturnsTrue()
    {
        // Arrange
        var subject = new Subject("removeSubjectTestName");
        _subjectRepository.Add(subject);
        _context.SaveChanges();
        
        // Act
        var result = _subjectRepository.RemoveSubject(subject.Id);

        // Assert
        Assert.True(result);
    }
    
    [Fact]
    public void RemoveSubject_UserDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var subject = new Subject("nonexistentTestRemoveSubject");
        
        // Act
        var result = _subjectRepository.RemoveSubject(subject.Id);

        // Assert
        Assert.False(result);
    }

    
}
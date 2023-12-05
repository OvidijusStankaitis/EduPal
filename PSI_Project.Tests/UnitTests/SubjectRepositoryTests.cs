using Microsoft.EntityFrameworkCore;
using Moq;
using PSI_Project.Controllers;
using PSI_Project.Data;
using PSI_Project.Exceptions;
using PSI_Project.Models;
using PSI_Project.Repositories;

namespace PSI_Project.Tests.UnitTests;

public class SubjectRepositoryTests
{
    [Fact]
    public void GetSubjectsList_ReturnsListOfSubjects()
    {
        // Arrange
        var subjects = new List<Subject>
        {
            new Subject ("subject1")
        };

        var mockDbContext = new Mock<EduPalDatabaseContext>();
        var mockRepository = new Mock<SubjectRepository>(mockDbContext.Object);
        
        mockRepository.Setup(repo => repo.GetAll()).Returns(subjects);

        var subjectRepository = mockRepository.Object;

        // Act
        var result =  subjectRepository.GetSubjectsList();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(subjects.Count, result.Count());
        Assert.Equal(subjects[0].Name,result[0].Name);
    }
    
    [Fact]
    public void GetSubjectsList_DBContainsTwoElement_ReturnsAListOfTwoElements()
    {
        // Arrange
        var subjects = new List<Subject>
        {
            new Subject ("subject1"),
            new Subject ("subject2"),
        };

        var mockDbContext = new Mock<EduPalDatabaseContext>();
        var mockRepository = new Mock<SubjectRepository>(mockDbContext.Object);
        
        mockRepository.Setup(repo => repo.GetAll()).Returns(subjects);

        var subjectRepository = mockRepository.Object;

        // Act
        var result = subjectRepository.GetSubjectsList();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(subjects.Count, result.Count());
        Assert.Equal(subjects[0].Name,result[0].Name);
        Assert.Equal(subjects[1].Id,result[1].Id);
    }
    
    [Fact]
    public void GetSubjectsList_DBIsEmpty_ReturnsEmpty()
    {
        // Arrange
        var mockDbContext = new Mock<EduPalDatabaseContext>();
        var mockRepository = new Mock<SubjectRepository>(mockDbContext.Object);
        
        mockRepository.Setup(repo => repo.GetAll()).Returns(new List<Subject>());
        
        var subjectRepository = mockRepository.Object;
        
        // Act
        var result = subjectRepository.GetSubjectsList();

        // Assert
        Assert.Empty(result);
    }
    
    [Fact]
    public async Task RemoveSubjectAsync_SubjectExists_ReturnsTrue()
    {
        // Arrange
        var subject = new Subject("removeSubjectTestName");
        
        var mockDbContext = new Mock<EduPalDatabaseContext>();
        var mockRepository = new Mock<SubjectRepository>(mockDbContext.Object);

        mockRepository.Setup(repo => repo.GetAsync(subject.Id)).ReturnsAsync(subject);
        mockRepository.Setup(repo => repo.Remove(subject)).Returns(2);
        
        var subjectRepository = mockRepository.Object;
        
        // Act
        var result = await subjectRepository.RemoveSubjectAsync(subject.Id);

        // Assert
        Assert.True(result);
    }
    
    [Fact]
    public async Task RemoveSubjectAsync_SubjectDoesNotExist_ThrowsObjectNotFoundException()
    {
        // Arrange
        var subject = new Subject("removeSubjectTestName");
        
        var mockDbContext = new Mock<EduPalDatabaseContext>();
        var mockRepository = new Mock<SubjectRepository>(mockDbContext.Object);

        mockRepository.Setup(repo => repo.GetAsync(subject.Id)).ThrowsAsync(new ObjectNotFoundException());
        mockRepository.Setup(repo => repo.Remove(subject)).Returns(0);
        
        var subjectRepository = mockRepository.Object;
        
        // Act
        var exception = await Assert.ThrowsAsync<ObjectNotFoundException>(async () => await subjectRepository.RemoveSubjectAsync(subject.Id));
        
        // Assert
        Assert.Contains("Exception of type 'PSI_Project.Exceptions", exception.Message);
    }
}
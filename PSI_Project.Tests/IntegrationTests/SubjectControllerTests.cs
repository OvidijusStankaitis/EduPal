using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using PSI_Project.Controllers;
using PSI_Project.Data;
using PSI_Project.Models;
using PSI_Project.Repositories;
using PSI_Project.Services;
using PSI_Project.Tests.IntegrationTests.Configuration;

namespace PSI_Project.Tests.IntegrationTests;

public class SubjectControllerTests : IDisposable
{
    private readonly HttpClient _client;
    private readonly TestingWebAppFactory _factory;
    
    public SubjectControllerTests()
    {
        _factory = new TestingWebAppFactory();
        _client = _factory.CreateClient();
        
        // Setting up logged in user
        User user = new User("test1@test.test", "testPassword1", "testName", "testSurname")
        {
            Id = "test-user-id-1"
        };

        using var scope = _factory.Services.CreateScope();
        TestUserAuthService? testAuthService = scope.ServiceProvider.GetRequiredService<IUserAuthService>() as TestUserAuthService;
        testAuthService?.SetAuthenticatedUser(user);
    }

    [Fact]
    public async Task ListSubjects_Always_ReturnsListOfOneSubject()
    {
        // Arrange
        var expectedNames = new List<string> { "testSubject1", "testSubject2", "testSubject3" };

        var loggerMock = new Mock<ILogger<SubjectController>>();

        var mockDbContext = new Mock<EduPalDatabaseContext>();
        var subjectRepositoryMock = new Mock<SubjectRepository>(mockDbContext.Object);
        subjectRepositoryMock.Setup(repo => repo.GetSubjectsList())
            .Returns(new List<Subject>
            {
                new Subject(expectedNames[0]),
                new Subject(expectedNames[1]),
                new Subject(expectedNames[2])
            });
        
        var subjectController = new SubjectController(loggerMock.Object, subjectRepositoryMock.Object);

        // Act
        var response = subjectController.ListSubjects() as ObjectResult;
        var data = response?.Value as IEnumerable<Subject>;

        // Assert
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, (HttpStatusCode)response.StatusCode);
        Assert.NotNull(data);
        Assert.True(data.Count() == 3);

        Assert.All(expectedNames, expectedName =>
        {
            Assert.Contains(data, subject => subject.Name == expectedName);
        });

        subjectRepositoryMock.Verify(repo => repo.GetSubjectsList(), Times.Once);
    }

    [Fact] 
    public async Task GetSubject_GetsExistingId_ReturnsOkAndOneSubjectWithThisID()
    {
        // Arrange
        var testSubject = new { subjectName = "subjectTest"}; 
        
        // Act
        //uploading a subject for the test
        var responseToGetSubject = await _client.PostAsync("/subject/upload", JsonContent.Create(testSubject));
        var responseStringForSubject1 = await responseToGetSubject.Content.ReadAsStringAsync();
        var resultSubject1 = JsonConvert.DeserializeObject<Subject>(responseStringForSubject1);
        
        var response = await _client.GetAsync($"/subject/get/{resultSubject1?.Id}");
        var responseString = await response.Content.ReadAsStringAsync();
        var resultSubject2 = JsonConvert.DeserializeObject<Subject>(responseString);
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(resultSubject1?.Name, resultSubject2?.Name);
        Assert.Equal(resultSubject1?.Id, resultSubject2?.Id);
    }
    
    [Fact] 
    public async Task GetSubject_GetsNonExistingId_ReturnsNotFound()
    {
        // Arrange
        
        // Act
        var response = await _client.GetAsync("/subject/get/nonexistent-id");
        var responseString = await response.Content.ReadAsStringAsync();
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("There is no subject with such id", responseString);
    }

    [Fact]
    public async Task UploadSubject_GetsValidSubject_ReturnsOk()
    {
        // Arrange
        var validSubject = new { subjectName = "validSubjectTest"}; 
        
        // Act
        var response = await _client.PostAsync("/subject/upload", JsonContent.Create(validSubject));
        var responseString = await response.Content.ReadAsStringAsync();
        var resultSubject = JsonConvert.DeserializeObject<Subject>(responseString);
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("validSubjectTest", resultSubject?.Name);
    }
    
    [Fact]
    public async Task UploadSubject_GetsInvalidSubject_ReturnsBadRequest()
    {
        // Arrange
        var validSubject = new { invalidSubjectName = "invalidSubjectTest"}; 
        
        // Act
        var response = await _client.PostAsync("/subject/upload", JsonContent.Create(validSubject));
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task RemoveSubject_GetsValidId_ReturnsOk()
    {
        // Arrange
        var testSubject = new { subjectName = "subjectTest"}; 
        
        // Act
        //uploading a subject for the test
        var responseToGetSubject = await _client.PostAsync("/subject/upload", JsonContent.Create(testSubject));
        var responseStringForSubject1 = await responseToGetSubject.Content.ReadAsStringAsync();
        var resultSubject1 = JsonConvert.DeserializeObject<Subject>(responseStringForSubject1);
        
        var response = await _client.DeleteAsync($"/subject/delete/{resultSubject1?.Id}");
        var responseString = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Subject has been successfully deleted", responseString);
    }
    
    [Fact]
    public async Task RemoveSubject_GetsInvalidId_ReturnsBadRequest()
    {
        // Arrange
        
        // Act
        var response = await _client.DeleteAsync("/subject/delete/nonexistent-id");
        var responseString = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("An error occurred while deleting the subject", responseString);
    }
        
    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}
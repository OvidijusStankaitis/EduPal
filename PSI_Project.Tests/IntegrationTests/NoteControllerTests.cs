using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using PSI_Project.DTO;
using PSI_Project.Models;
using PSI_Project.Services;
using PSI_Project.Tests.IntegrationTests.Configuration;

namespace PSI_Project.Tests.IntegrationTests;

public class NoteControllerTests : IDisposable
{
    private readonly HttpClient _client;
    private readonly TestingWebAppFactory _factory;

    public NoteControllerTests()
    {
        _factory = new TestingWebAppFactory();
        _client = _factory.CreateClient();

        // Setting up logged in user
        User user = new User("test1@test.test", "testPassword1", "testName", "testSurname")
        {
            Id = "test-user-id-1"
        };

        using var scope = _factory.Services.CreateScope();
        TestUserAuthService? testAuthService =
            scope.ServiceProvider.GetRequiredService<IUserAuthService>() as TestUserAuthService;
        testAuthService?.SetAuthenticatedUser(user);
    }

    [Fact]
    public async Task GetNoteById_GetsNonexistentId_ReturnsBadRequest()
    {
        // Arrange
        var invalidNote = new Note("testName", "testContent");

        // Act
        var response = await _client.GetAsync($"/note/{invalidNote.Id}");
        var responseString = await response.Content.ReadAsStringAsync();
        var errorResponse = JsonConvert.DeserializeObject<ErrorResponseDTO>(responseString);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal($"The value '{invalidNote.Id}' is not valid.", errorResponse.Errors["id"].First());
    }
    
    [Fact]
    public async Task GetAllNotes_AlwaysIfNoException_ReturnsOkAndListOfNotes()
    {
        // Arrange
        var expectedNames = new List<string> { "testName1", "testName2" };
        var expectedContents = new List<string> { "testContent1", "testContent2" };

        // Act
        var response = await _client.GetAsync($"/note");
        var responseString = await response.Content.ReadAsStringAsync();
        var data= JsonConvert.DeserializeObject<IEnumerable<Note>>(responseString);
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(data.Count() >= 2);
        Assert.All(expectedNames, expectedName =>
        {
            Assert.Contains(data, note => note.Name == expectedName);
        });
        Assert.All(expectedContents, expectedContents =>
        {
            Assert.Contains(data, note => note.Content == expectedContents);
        });
    }
    
    [Fact]
    public async Task AddNote_GetsValidNote_ReturnsOk()
    {
        // Arrange
        var validNote = new NoteCreationDTO("testName", "testContent");

        // Act
        var response = await _client.PostAsync("/note", JsonContent.Create(validNote));
        var responseString = await response.Content.ReadAsStringAsync();
        var addedNote = JsonConvert.DeserializeObject<Note>(responseString);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(addedNote.Id);
        Assert.Equal("testName", addedNote.Name);
        Assert.Equal("testContent", addedNote.Content);
    }
    
    [Fact]
    public async Task AddNote_GetsInvalidNote_ReturnsBadRequest()
    {
        // Arrange
        var invalidNote = new { invalidNoteName = "_____"}; 
        
        // Act
        var response = await _client.PostAsync("/note", JsonContent.Create(invalidNote));
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}
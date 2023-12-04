using System.Net;
using System.Net.Http.Json;
using Newtonsoft.Json;
using PSI_Project.DTO;
using PSI_Project.Models;

namespace PSI_Project.Tests.IntegrationTests;

public class NoteControllerTests : IDisposable
{
    private readonly HttpClient _client;
    private readonly TestingWebAppFactory _factory;
    
    public NoteControllerTests()
    {
        _factory = new TestingWebAppFactory();
        _client = _factory.CreateClient();
    }
    
    /*[Fact]
    public async Task GetNoteById_GetsExistingId_ReturnsOkAndNote()
    {
        //Arrange
        var responseForNotes = await _client.GetAsync($"/note");
        var data= JsonConvert.DeserializeObject<IEnumerable<Note>>(await responseForNotes.Content.ReadAsStringAsync());
        var oneNote = data?.FirstOrDefault();
        
        // Act
        var response = await _client.GetAsync($"/note/{oneNote?.Id}");
        var responseString = await response.Content.ReadAsStringAsync();
        var note = JsonConvert.DeserializeObject<Note>(responseString);
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(note);
        Assert.Equal(oneNote.Id, note.Id);
        Assert.Equal(oneNote.Content, note.Content);
        Assert.Equal(oneNote.Name, note.Name);
    }*/
    
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
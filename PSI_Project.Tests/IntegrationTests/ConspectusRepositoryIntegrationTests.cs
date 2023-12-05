using System.Net;
using System.Net.Http.Json;
using Newtonsoft.Json;
using PSI_Project.Models;
using PSI_Project.Tests.IntegrationTests.Configuration;

namespace PSI_Project.Tests.IntegrationTests;

public class ConspectusRepositoryIntegrationTests : IDisposable
{
    private readonly HttpClient _client;
    private readonly TestingWebAppFactory _factory;
    
    public ConspectusRepositoryIntegrationTests()
    {
        _factory = new TestingWebAppFactory();
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetTopicFilesAsync_GetsValidTopicId_ReturnsOkAndListOfConspectuses()
    {
        // Arrange
        var responseForSubjects = await _client.GetAsync("/subject/list");
        var listOfSubjects = JsonConvert.DeserializeObject<IEnumerable<Subject>>(await responseForSubjects.Content.ReadAsStringAsync()); 
        var responseForTopics = await _client.GetAsync($"/topic/list/{listOfSubjects?.ToList()[2].Id}");
        var listOfTopics = JsonConvert.DeserializeObject<IEnumerable<Topic>>(await responseForTopics.Content.ReadAsStringAsync());
        
        
        // Act
        var response = await _client.GetAsync($"/conspectus/list/{listOfTopics?.ToList()[0].Id}");
        var responseString = await response.Content.ReadAsStringAsync();
        var conspectuses = JsonConvert.DeserializeObject<IEnumerable<Conspectus>>(responseString);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(conspectuses?.Count() >= 1);
        Assert.Equal("conspectus1.pdf",conspectuses.ToList()[0].Name);
    }
    
    [Fact]
    public async Task GetTopicFilesAsync_GetsNonexistentTopicId_ReturnsOkAndEmptyListOfConspectuses()
    {
        // Arrange
        
        // Act
        var response = await _client.GetAsync($"/conspectus/list/some-topic-id");
        var responseString = await response.Content.ReadAsStringAsync();
        var conspectuses = JsonConvert.DeserializeObject<IEnumerable<Conspectus>>(responseString);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Empty(conspectuses);

    }
    
    /*[Fact]
    public async Task GetConspectusAsync_GetsValidConspectusId_ReturnsOk()
    {
        // Arrange
        var responseForSubjects = await _client.GetAsync("/subject/list");
        var listOfSubjects = JsonConvert.DeserializeObject<IEnumerable<Subject>>(await responseForSubjects.Content.ReadAsStringAsync()); 
        var responseForTopics = await _client.GetAsync($"/topic/list/{listOfSubjects?.ToList()[2].Id}");
        var listOfTopics = JsonConvert.DeserializeObject<IEnumerable<Topic>>(await responseForTopics.Content.ReadAsStringAsync());
        var responseForConspectuses = await _client.GetAsync($"/conspectus/list/{listOfTopics?.ToList()[0].Id}");
        var listOfConspectuses = JsonConvert.DeserializeObject<IEnumerable<Conspectus>>(await responseForConspectuses.Content.ReadAsStringAsync());
        
        // Act
        var response = await _client.GetAsync($"/conspectus/get/{listOfConspectuses?.ToList()[0].Id}");
        var responseString = await response.Content.ReadAsStringAsync();
    
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        // more asserts should be added 
    }*/
    
    [Fact]
    public async Task RateConspectusUpAsync_GetsValidConspectusId_ReturnsOkAndUpdatedConspectus()
    {
        // Arrange
        var responseForSubjects = await _client.GetAsync("/subject/list");
        var listOfSubjects = JsonConvert.DeserializeObject<IEnumerable<Subject>>(await responseForSubjects.Content.ReadAsStringAsync()); 
        var responseForTopics = await _client.GetAsync($"/topic/list/{listOfSubjects?.ToList()[2].Id}");
        var listOfTopics = JsonConvert.DeserializeObject<IEnumerable<Topic>>(await responseForTopics.Content.ReadAsStringAsync());
        var responseForConspectuses = await _client.GetAsync($"/conspectus/list/{listOfTopics?.ToList()[0].Id}");
        var listOfConspectuses = JsonConvert.DeserializeObject<IEnumerable<Conspectus>>(await responseForConspectuses.Content.ReadAsStringAsync());

        var conspectusTested = listOfConspectuses?.ToList()[0];
        
        // Act
        var response = await _client.PostAsync($"/conspectus/rate-up/{conspectusTested?.Id}",JsonContent.Create(conspectusTested));
        var responseString = await response.Content.ReadAsStringAsync();
        var conspectusUpdated = JsonConvert.DeserializeObject<Conspectus>(responseString);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(conspectusTested?.Id,conspectusUpdated?.Id);
        Assert.Equal(conspectusTested?.Rating + 1, conspectusUpdated?.Rating);
    }
        
    [Fact]
    public async Task RateConspectusDownAsync_GetsValidConspectusId_ReturnsOkAndUpdatedConspectus()
    {
        // Arrange
        var responseForSubjects = await _client.GetAsync("/subject/list");
        var listOfSubjects = JsonConvert.DeserializeObject<IEnumerable<Subject>>(await responseForSubjects.Content.ReadAsStringAsync()); 
        var responseForTopics = await _client.GetAsync($"/topic/list/{listOfSubjects?.ToList()[2].Id}");
        var listOfTopics = JsonConvert.DeserializeObject<IEnumerable<Topic>>(await responseForTopics.Content.ReadAsStringAsync());
        var responseForConspectuses = await _client.GetAsync($"/conspectus/list/{listOfTopics?.ToList()[0].Id}");
        var listOfConspectuses = JsonConvert.DeserializeObject<IEnumerable<Conspectus>>(await responseForConspectuses.Content.ReadAsStringAsync());

        var conspectusTested = listOfConspectuses?.ToList()[0];
        
        // Act
        var response = await _client.PostAsync($"/conspectus/rate-down/{conspectusTested?.Id}",JsonContent.Create(conspectusTested));
        var responseString = await response.Content.ReadAsStringAsync();
        var conspectusUpdated = JsonConvert.DeserializeObject<Conspectus>(responseString);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(conspectusTested?.Id,conspectusUpdated?.Id);
        Assert.Equal(conspectusTested?.Rating - 1, conspectusUpdated?.Rating);
    }
    
    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}
using System.Net;
using System.Net.Http.Json;
using Newtonsoft.Json;
using PSI_Project.Models;

namespace PSI_Project.Tests.IntegrationTests;

public class TopicControllerIntegrationTests : IDisposable
{
    private readonly HttpClient _client;
    private readonly TestingWebAppFactory _factory;
    
    public TopicControllerIntegrationTests()
    {
        _factory = new TestingWebAppFactory();
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task UploadTopic_GetsValidTopic_ReturnsOkAndNewTopic()
    {
        // Arrange
        var responseForSubject = await _client.GetAsync("/subject/list");
        var data = JsonConvert.DeserializeObject<IEnumerable<Subject>>(await responseForSubject.Content.ReadAsStringAsync());    
        
        var validTopic = new { topicName = "validTopicNameTest", subjectId = $"{data.First().Id}"}; 
        
        // Act
        var response = await _client.PostAsync("/topic/upload", JsonContent.Create(validTopic));
        var responseString = await response.Content.ReadAsStringAsync();
        var resultTopic = JsonConvert.DeserializeObject<Topic>(responseString);
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("validTopicNameTest", resultTopic?.Name);
        Assert.Equal(data.First().Id, resultTopic?.Subject.Id);
        Assert.Equal(KnowledgeLevel.Poor, resultTopic?.KnowledgeRating);
    }
    
    [Fact]
    public async Task UploadTopic_GetsValidTopicButNotValidSubjectId_ReturnsBadRequest()
    {
        // Arrange
        var invalidTopic = new { topicName = "validTopicNameTest", subjectId = "nonexistentId"}; 
        
        // Act
        var response = await _client.PostAsync("/topic/upload", JsonContent.Create(invalidTopic));
        var responseString = await response.Content.ReadAsStringAsync();
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("An error occured while uploading topic", responseString);
    }
    
    [Fact]
    public async Task UploadTopic_GetsTopicWithInvalidName_ReturnsBadRequest()
    {
        // Arrange
        var responseForSubject = await _client.GetAsync("/subject/list");
        var data = JsonConvert.DeserializeObject<IEnumerable<Subject>>(await responseForSubject.Content.ReadAsStringAsync());    
        
        var invalidTopic = new { topicName = default(string), subjectId = $"{data?.First().Id}"}; 
        
        // Act
        var response = await _client.PostAsync("/topic/upload", JsonContent.Create(invalidTopic));
        var responseString = await response.Content.ReadAsStringAsync();
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("Invalid topic name", responseString);
    }
    
    [Fact]
    public async Task RemoveTopic_GetsExistingTopic_ReturnsOk()
    {
        // Arrange
        var responseForSubject = await _client.GetAsync("/subject/list");
        var data = JsonConvert.DeserializeObject<IEnumerable<Subject>>(await responseForSubject.Content.ReadAsStringAsync());    
        
        var validTopic = new { topicName = "validTopicNameTest", subjectId = $"{data.First().Id}"};
        var responseForTopic = await _client.PostAsync("/topic/upload", JsonContent.Create(validTopic));
        var resultTopic = JsonConvert.DeserializeObject<Topic>(await responseForTopic.Content.ReadAsStringAsync());
        
        // Act
        var response = await _client.DeleteAsync($"/topic/delete/{resultTopic?.Id}");
        var responseString = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Topic has been successfully deleted", responseString);
    }
    
    [Fact]
    public async Task RemoveTopic_GetsNonexistentTopic_ReturnsOk()
    {
        // Arrange
        
        // Act
        var response = await _client.DeleteAsync($"/topic/delete/nonexistent-id");
        var responseString = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("An error occured while deleting the topic", responseString);
    }


    [Fact]
    public async Task ListTopics_GetsValidSubjectId_ReturnsOkAndListOfTopics()
    {
        // Arrange
        var responseForSubject = await _client.GetAsync("/subject/list");
        var listOfSubjects = JsonConvert.DeserializeObject<IEnumerable<Subject>>(await responseForSubject.Content.ReadAsStringAsync()); 
        
        // Act
        var response = await _client.GetAsync($"/topic/list/{listOfSubjects.ToList()[1].Id}");
        var data = JsonConvert.DeserializeObject<IEnumerable<Topic>>(await response.Content.ReadAsStringAsync());
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Collection((data as IEnumerable<Topic>)!,
            t1 => Assert.Equal("testTopic1", t1.Name),
            t2 => Assert.Equal("testTopic2", t2.Name));
    }
    
    [Fact]
    public async Task ListTopics_GetsInvalidSubjectId_ReturnsOkAndEmptyList()
    {
        // Arrange
        
        // Act
        var response = await _client.GetAsync($"/topic/list/nonexistent-subject-id");
        var responseString = await response.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject<IEnumerable<Topic>>(await response.Content.ReadAsStringAsync());

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Empty(data);
    }
    
    [Fact]
    public async Task GetTopicById_GetsValidTopicId_ReturnsOkAndTopic()
    {
        // Arrange
        var responseForSubjects = await _client.GetAsync("/subject/list");
        var listOfSubjects = JsonConvert.DeserializeObject<IEnumerable<Subject>>(await responseForSubjects.Content.ReadAsStringAsync()); 
        var responseForTopics = await _client.GetAsync($"/topic/list/{listOfSubjects?.ToList()[1].Id}");
        var listOfTopics = JsonConvert.DeserializeObject<IEnumerable<Topic>>(await responseForTopics.Content.ReadAsStringAsync());
        
        // Act
        var response = await _client.GetAsync($"/topic/get/{listOfTopics?.ToList()[0].Id}");
        var responseString = await response.Content.ReadAsStringAsync();
        var topic = JsonConvert.DeserializeObject<Topic>(responseString);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(listOfTopics?.ToList()[0].Id, topic?.Id);
        Assert.Equal(listOfTopics?.ToList()[0].Name, topic?.Name);
        Assert.Equal(listOfTopics?.ToList()[0].Subject, topic?.Subject);
        Assert.Equal(listOfTopics?.ToList()[0].KnowledgeRating, topic?.KnowledgeRating);
    }
    
    [Fact]
    public async Task GetTopicById_GetsInvalidTopicId_NotFound()
    {
        // Arrange
        
        // Act
        var response = await _client.GetAsync($"/topic/get/nonexistent-id");
        var responseString = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("There is no topic with such id", responseString);
    }

    [Fact]
    public async Task UpdateKnowledgeLevel_GetsValidTopicIdAndKnowledgeLevel_ReturnsOk()
    {
        // Arrange
        // creating a new topic for the test (then deleting it)
        var responseForSubject = await _client.GetAsync("/subject/list");
        var data = JsonConvert.DeserializeObject<IEnumerable<Subject>>(await responseForSubject.Content.ReadAsStringAsync());    
        var validTopic = new { topicName = "validTopicNameTest", subjectId = $"{data.First().Id}"}; 
        var responseForTopic = await _client.PostAsync("/topic/upload", JsonContent.Create(validTopic));
        var resultTopic = JsonConvert.DeserializeObject<Topic>(await responseForTopic.Content.ReadAsStringAsync());

        var validData = new { topicId = resultTopic.Id, knowledgeLevel = KnowledgeLevel.Good.ToString()};
        
        // Act
        var response = await _client.PutAsync($"/topic/update-knowledge-level", JsonContent.Create(validData));
        var responseString = await response.Content.ReadAsStringAsync();
        
        // deleting the new topic
        await _client.DeleteAsync($"/topic/delete/{resultTopic.Id}");
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Knowledge level updated successfully", responseString);
    }
    
    [Fact]
    public async Task UpdateKnowledgeLevel_GetsValidTopicIdAndInvalidKnowledgeLevel_ReturnsBadRequest()
    {
        // Arrange
        // creating a new topic for the test (then deleting it)
        var responseForSubject = await _client.GetAsync("/subject/list");
        var data = JsonConvert.DeserializeObject<IEnumerable<Subject>>(await responseForSubject.Content.ReadAsStringAsync());    
        var validTopic = new { topicName = "validTopicNameTest", subjectId = $"{data.First().Id}"}; 
        var responseForTopic = await _client.PostAsync("/topic/upload", JsonContent.Create(validTopic));
        var resultTopic = JsonConvert.DeserializeObject<Topic>(await responseForTopic.Content.ReadAsStringAsync());

        var invalidData = new { topicId = resultTopic.Id, knowledgeLevel = KnowledgeLevel.Good};
        
        // Act
        var response = await _client.PutAsync($"/topic/update-knowledge-level", JsonContent.Create(invalidData));
        var responseString = await response.Content.ReadAsStringAsync();
        
        // deleting the new topic
        await _client.DeleteAsync($"/topic/delete/{resultTopic.Id}");
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("Invalid request body:", responseString);
    }
    
    [Fact]
    public async Task UpdateKnowledgeLevel_GetsInvalidTopicId_ReturnsBadRequest()
    {
        // Arrange
        var invalidData = new { topicId = "nonexistentId", knowledgeLevel = KnowledgeLevel.Good.ToString()};
        
        // Act
        var response = await _client.PutAsync($"/topic/update-knowledge-level", JsonContent.Create(invalidData));
        var responseString = await response.Content.ReadAsStringAsync();
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("Invalid request body:", responseString);
    }
    
    [Fact]
    public async Task UpdateKnowledgeLevel_GetsNonexistentKnowledgeLevel_ReturnsBadRequest()
    {
        // Arrange
        // creating a new topic for the test (then deleting it)
        var responseForSubject = await _client.GetAsync("/subject/list");
        var data = JsonConvert.DeserializeObject<IEnumerable<Subject>>(await responseForSubject.Content.ReadAsStringAsync());    
        var validTopic = new { topicName = "validTopicNameTest", subjectId = $"{data.First().Id}"}; 
        var responseForTopic = await _client.PostAsync("/topic/upload", JsonContent.Create(validTopic));
        var resultTopic = JsonConvert.DeserializeObject<Topic>(await responseForTopic.Content.ReadAsStringAsync());

        var invalidData = new { topicId = resultTopic.Id, knowledgeLevel = "nonexistentKnowledgeLevel"};
        
        // Act
        var response = await _client.PutAsync($"/topic/update-knowledge-level", JsonContent.Create(invalidData));
        var responseString = await response.Content.ReadAsStringAsync();
        
        // deleting the new topic
        await _client.DeleteAsync($"/topic/delete/{resultTopic.Id}");
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("Invalid knowledge level", responseString);
    }
    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}
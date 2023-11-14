using System.Text;
using System.Text.Json;
using PSI_Project.Models;
using PSI_Project.Repositories;

namespace PSI_Project.Services;

public class OpenAIService
{
    private readonly OpenAIRepository _openAIRepository;
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public OpenAIService(OpenAIRepository openAIRepository, IHttpClientFactory httpClientFactory)
    {
        _openAIRepository = openAIRepository;
        _httpClient = httpClientFactory.CreateClient();
        _apiKey = File.ReadAllText("..//PSI_Project//DB//api_key.txt").Trim();
    }

    public async Task<string?> SendMessageAsync(string userMessage, string userEmail) // 7. Usage of async/await
    {
        //_openAIRepository.InsertItem(new Message(userMessage, userEmail, true));
        _openAIRepository.Add(new Message(userMessage, userEmail, true));
        _openAIRepository.EduPalContext.SaveChanges();

        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");

        // Use the _apiKey variable to set the Authorization header
        request.Headers.Add("Authorization", $"Bearer {_apiKey}");

        // Construct the messages array to send as part of the request payload
        var messages = new[]
        {
            new { role = "system", content = "You are a helpful assistant." },
            new { role = "user", content = userMessage }
        };

        // Convert the messages array to a JSON string
        var payload = JsonSerializer.Serialize(new { model = "gpt-3.5-turbo", messages });  // Include the model parameter

        request.Content = new StringContent(payload, Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode) return null;

        var openAIResponse = await response.Content.ReadAsStringAsync();
        var responseObject = JsonDocument.Parse(openAIResponse);

        // Corrected path to extract the generated message
        var generatedMessage = responseObject.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

       // _openAIRepository.InsertItem(new Message(generatedMessage, userEmail, false));  // corrected line
       _openAIRepository.Add(new Message(generatedMessage, userEmail, false));  // corrected line
       _openAIRepository.EduPalContext.SaveChanges();

        return generatedMessage;
    }
}
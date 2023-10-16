using System.Text;
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
        
        // Read the API key from the file using the literal path you provided
        _apiKey = File.ReadAllText("..//PSI_Project//DB//api_key.txt").Trim();
    }

    public async Task<string?> SendMessageAsync(string userMessage)
    {
        _openAIRepository.InsertItem(new Message(userMessage, true));

        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/engines/davinci/completions");
        
        // Use the _apiKey variable to set the Authorization header
        request.Headers.Add("Authorization", $"Bearer {_apiKey}");
        
        request.Content = new StringContent($"{{\"prompt\": \"{userMessage}\", \"max_tokens\": 150}}", Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode) return null;

        var openAIResponse = await response.Content.ReadAsStringAsync();
        _openAIRepository.InsertItem(new Message(openAIResponse, false));

        return openAIResponse;
    }
}
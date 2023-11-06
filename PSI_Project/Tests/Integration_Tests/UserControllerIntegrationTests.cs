using Xunit;

namespace PSI_Project.Tests.Integration_Tests;

public class UserControllerIntegrationTests : IDisposable
{
    private readonly HttpClient _client;
    private readonly TestingWebAppFactory _factory;
    
    public UserControllerIntegrationTests()
    {
        _factory = new TestingWebAppFactory();
        _client = _factory.CreateClient();
    }
    
    

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}
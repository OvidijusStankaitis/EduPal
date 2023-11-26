namespace PSI_Project.Tests.IntegrationTests;

public class ErrorResponse
{
    public int Status { get; set; }
    public Dictionary<string, List<string>> Errors { get; set; }
}
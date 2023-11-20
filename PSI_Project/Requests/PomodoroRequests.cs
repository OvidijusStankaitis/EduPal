namespace PSI_Project.Requests;

public class TimerRequest
{
    public string UserId { get; set; }
    public int Duration { get; set; }
}

public class TimerStopRequest
{
    public string UserId { get; set; }
}
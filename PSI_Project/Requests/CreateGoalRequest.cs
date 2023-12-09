namespace PSI_Project.Requests;

public class CreateGoalRequest
{
    public string UserId { get; set; }
    public List<string> SubjectIds { get; set; }
    public double GoalTime { get; set; }
}
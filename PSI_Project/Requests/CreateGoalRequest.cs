namespace PSI_Project.Requests;

public class CreateGoalRequest
{
    public List<string> SubjectIds { get; set; }
    public double GoalTime { get; set; }
}
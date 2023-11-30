namespace PSI_Project.DTO;

public class GoalDetailDto
{
    public string Id { get; set; }
    public DateTime GoalDate { get; set; }
    public double TargetHours { get; set; }
    public double ActualHoursStudied { get; set; }
}
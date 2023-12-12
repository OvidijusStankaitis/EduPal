namespace PSI_Project.Models
{
    public class SubjectGoal : BaseEntity
    {
        public string SubjectId { get; set; }
        public Subject Subject { get; set; }
        public string GoalId { get; set; }
        public Goal Goal { get; set; }
        public double TargetHours { get; set; }
        public double ActualHoursStudied { get; set; } = 0;
        
        public SubjectGoal()
        {
            
        }

        // Constructor that takes subject and goal
        public SubjectGoal(Subject subject, Goal goal, double targetHours)
        {
            Subject = subject;
            Goal = goal;
            TargetHours = targetHours;
        }
    }
}
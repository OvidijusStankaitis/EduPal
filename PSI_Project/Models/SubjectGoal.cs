namespace PSI_Project.Models
{
    public class SubjectGoal
    {
        public string SubjectId { get; set; }
        public double TargetHours { get; set; }
        public double ActualHoursStudied { get; set; }
        
        public SubjectGoal(string subjectId, double targetHours)
        {
            SubjectId = subjectId;
            TargetHours = targetHours;
            ActualHoursStudied = 0; // 0 hours studied on initialization
        }
    }
}
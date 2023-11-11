namespace PSI_Project.Models
{
    public class SubjectGoal : BaseEntity
    {
        public Subject Subject { get; set; }
        public double TargetHours { get; set; }
        public double ActualHoursStudied { get; set; }

        private SubjectGoal()   // used by EF    
        {
        }

        public SubjectGoal(Subject subject, double targetHours)
        {
            Subject = subject;
            TargetHours = targetHours;
            ActualHoursStudied = 0; // 0 hours studied on initialization
        }
    }
}
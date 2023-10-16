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
        
        public struct StudyDuration // 1. Struct used, not implemented yet TODO: view goals implementation which uses this struct
        {
            public int Hours { get; set; }
            public int Minutes { get; set; }

            public static StudyDuration ToReadableTime(double totalHours)
            {
                int totalMinutes = (int)(totalHours * 60);
                return new StudyDuration
                {
                    Hours = totalMinutes / 60,
                    Minutes = totalMinutes % 60
                };
            }

            public override string ToString()
            {
                return $"{Hours} hours and {Minutes} minutes";
            }
        }
    }
}
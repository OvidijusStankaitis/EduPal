namespace PSI_Project.Models
{
    public class Goal : BaseEntity
    {
        public string UserId { get; set; }
        public User User { get; set; }
        public DateTime GoalDate { get; set; } = DateTime.UtcNow;
        public List<SubjectGoal> SubjectGoals { get; set; } = new List<SubjectGoal>();
        
        public Goal() // Ef core usage
        {
        }
    }
}
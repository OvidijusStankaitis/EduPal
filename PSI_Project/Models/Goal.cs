using System.Collections.Generic;
using System;

namespace PSI_Project.Models
{
    public class Goal : BaseEntity
    {
        public DateTime GoalDate { get; set; } = DateTime.Now.Date; 
        public string UserId { get; set; }
        public List<SubjectGoal> SubjectGoals { get; set; }

        public Goal(string userId)
        {
            Id = Guid.NewGuid().ToString();
            UserId = userId;
            SubjectGoals = new List<SubjectGoal>();
        }
    }
}
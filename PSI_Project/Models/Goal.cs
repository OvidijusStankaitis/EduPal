using System.Collections.Generic;
using System;

namespace PSI_Project.Models
{
    public class Goal : BaseEntity
    {
        public DateTime GoalDate { get; set; } = DateTime.Now.Date; 
        //public string UserId { get; set; }
        public User User { get; set; }
        public List<SubjectGoal> SubjectGoals { get; set; }

        public Goal(User user)
        {
            Id = Guid.NewGuid().ToString(); // do we need it here if there is such code in BaseEntity?
            User = user;
            SubjectGoals = new List<SubjectGoal>();
        }
    }
}
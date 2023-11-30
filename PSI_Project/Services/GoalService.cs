using PSI_Project.Repositories;
using PSI_Project.Models;
using System.Linq;

namespace PSI_Project.Services
{
    public class GoalService
    {
        private GoalsRepository _goalsRepository;
        public GoalService(GoalsRepository goalsRepository)
        {
            _goalsRepository = goalsRepository;
        }
        
        public bool CanCreateNewGoal(string userId)
        {
            var goals = _goalsRepository.GetAllGoalsForUser(userId);

            // If there are no goals, then we can create a new one
            if (!goals.Any())
            {
                return true;
            }

            // If there are goals, check if all subject goals are completed
            foreach (var goal in goals)
            {
                foreach (var subjectGoal in goal.SubjectGoals)
                {
                    if (subjectGoal.ActualHoursStudied < subjectGoal.TargetHours)
                    {
                        // Found a subject goal that is not complete
                        return false;
                    }
                }
            }

            // All goals and their subject goals are complete
            return true;
        }
        
        public bool AddGoal(Goal goal, List<string> subjectIds)
        {
            // Check if the user is allowed to create a new goal
            if (!CanCreateNewGoal(goal.UserId))
            {
                // User cannot create a new goal
                return false;
            }
            
            // Check if at least one subject is selected
            if (subjectIds == null || !subjectIds.Any())
            {
                // No subjects selected
                return false;
            }
            
            // Add the goal to the database
            return _goalsRepository.AddGoal(goal);
        }
        
        public bool AddSubjectGoal(SubjectGoal subjectGoal)
        {
            // Add the subject goal to the database
            return _goalsRepository.AddSubjectGoal(subjectGoal);
        }
        
        public Goal? GetTodaysGoalForUser(string userId)
        {
            return _goalsRepository.GetTodaysGoalForUser(userId);
        }
        
        public List<Goal> GetAllGoalsForUser(string userId)
        {
            return _goalsRepository.GetAllGoalsForUser(userId);
        }
        

        public bool UpdateHoursStudied(string userId, string subjectId, double elapsedHours)
        {
            var todaysGoal = _goalsRepository.GetTodaysGoalForUser(userId);

            if (todaysGoal == null)
            {
                return false;
            }
            var subjectGoal = todaysGoal.SubjectGoals.FirstOrDefault(sg => sg.Subject.Id == subjectId);

            if (subjectGoal == null)
            {
                return false;
            }
            subjectGoal.ActualHoursStudied += elapsedHours;
            
            return _goalsRepository.UpdateItem(todaysGoal);
        }
    }
}
using PSI_Project.Repositories;
using PSI_Project.Models;
using System.Linq;
using PSI_Project.DTO;

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
            return _goalsRepository.AddSubjectGoal(subjectGoal);
        }
        
        public List<GoalDetailDto> GetAllGoalsForUserWithDetails(string userId)
        {
            return _goalsRepository.GetAllGoalsWithDetailsForUser(userId);
        }
        
        public bool UpdateHoursStudied(string userId, string subjectId, double elapsedHours)
        {
            var activeGoal = _goalsRepository.GetCurrentGoalForUser(userId);
            if (activeGoal == null)
            {
                return false;
            }

            var subjectGoalToUpdate = activeGoal.SubjectGoals.FirstOrDefault(sg => sg.Subject.Id == subjectId);
            if (subjectGoalToUpdate == null)
            {
                return false;
            }

            // Update actual hours and ensure it does not exceed target hours
            subjectGoalToUpdate.ActualHoursStudied = Math.Min(subjectGoalToUpdate.ActualHoursStudied + elapsedHours, subjectGoalToUpdate.TargetHours);
            return _goalsRepository.UpdateItem(activeGoal);
        }
        
        public SubjectGoal GetCurrentSubjectForUser(string userId)
        {
            return _goalsRepository.GetCurrentSubjectForUser(userId);
        }
    }
}
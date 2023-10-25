using PSI_Project.Repositories;
using PSI_Project.Models;

namespace PSI_Project.Services
{
    public class GoalService
    {
        private GoalsRepository _goalsRepository;

        public GoalService(GoalsRepository goalsRepository)
        {
            _goalsRepository = goalsRepository;
        }

        public bool AddGoal(Goal goal)
        {
            var todaysGoal = _goalsRepository.GetTodaysGoalForUser(goal.User.Id);
            if (todaysGoal != null)
            {
                // A goal for today already exists for this user (only 1 daily goal allowed)
                return false;
            }

            return _goalsRepository.InsertItem(goal);
        }

        public Goal? GetTodaysGoalForUser(string userId)
        {
            return _goalsRepository.GetTodaysGoalForUser(userId);
        }

        public List<Goal> GetAllGoalsForUser(string userId)
        {
            return _goalsRepository.GetAllGoalsForUser(userId);
        }
        
        // TODO: All the time time tracking logic for subjects will go here, thus we need to service layer for the business logic
    }
}
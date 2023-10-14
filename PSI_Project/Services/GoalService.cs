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

        public bool AddSubjectGoalToGoal(string goalId, SubjectGoal subjectGoal)
        {
            var goal = _goalsRepository.GetItemById(goalId);
            if (goal != null)
            {
                goal.SubjectGoals.Add(subjectGoal);
                _goalsRepository.UpdateItem(goal);
                return true; // successfully added
            }

            return false; // goal not found
        }
    }
}

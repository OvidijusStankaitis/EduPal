using Microsoft.EntityFrameworkCore;
using PSI_Project.Models;
using PSI_Project.Data;
using PSI_Project.DTO;

namespace PSI_Project.Repositories
{
    public class GoalsRepository : Repository<Goal>
    {
        public EduPalDatabaseContext EduPalContext => Context as EduPalDatabaseContext;
    
        public GoalsRepository(EduPalDatabaseContext context) : base(context)
        {
        }

        public bool AddGoal(Goal goal)
        {
            try
            {
                int changes = Add(goal);
                return changes > 0;
            }
            catch
            {
                return false;
            }
        }
        
        public bool AddSubjectGoal(SubjectGoal subjectGoal)
        {
            try
            {
                EduPalContext.SubjectGoal.Add(subjectGoal);
                int changes = EduPalContext.SaveChanges();
                return changes > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        
        public List<Goal> GetAllGoalsForUser(string userId)
        {
            return EduPalContext.Goals
                .Where(g => g.User.Id == userId)
                .Include(g => g.SubjectGoals)
                .ToList();
        }
        
        public List<GoalDetailDto> GetAllGoalsWithDetailsForUser(string userId)
        {
            var goalsWithDetails = EduPalContext.Goals
                .Where(g => g.UserId == userId)
                .Select(g => new GoalDetailDto
                {
                    Id = g.Id,
                    GoalDate = g.GoalDate,
                    TargetHours = g.SubjectGoals.FirstOrDefault().TargetHours,
                    ActualHoursStudied = g.SubjectGoals.FirstOrDefault().ActualHoursStudied
                })
                .ToList();

            return goalsWithDetails;
        }
        
        public bool UpdateItem(Goal goalToUpdate)
        {
            try
            {
                var goalByIndex = EduPalContext.Goals.Find(goalToUpdate.Id);
                
                if (goalByIndex == null) return false; // Goal not found
                
                EduPalContext.Goals.Update(goalToUpdate);
                int changes = EduPalContext.SaveChanges();

                return changes > 0;
            }
            catch
            {
                return false;
            }
        }
        
        public Goal GetCurrentGoalForUser(string userId)
        {
            return EduPalContext.Goals
                .Where(g => g.UserId == userId)
                .Include(g => g.SubjectGoals)
                .ThenInclude(sg => sg.Subject)
                .OrderByDescending(g => g.GoalDate)
                .FirstOrDefault(g => g.SubjectGoals.Any(sg => sg.TargetHours > sg.ActualHoursStudied));
        }
        
        public SubjectGoal GetCurrentSubjectForUser(string userId)
        {
            var goals = EduPalContext.Goals
                .Include(g => g.SubjectGoals)
                .ThenInclude(sg => sg.Subject)
                .Where(g => g.UserId == userId)
                .ToList();

            foreach (var goal in goals)
            {
                foreach (var subjectGoal in goal.SubjectGoals)
                {
                    if (subjectGoal.TargetHours > subjectGoal.ActualHoursStudied)
                    {
                        return subjectGoal; // Returns the first subject goal meeting the criteria
                    }
                }
            }

            return null; // No current subject found
        }
    }
}
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
        
        public Goal? GetTodaysGoalForUser(string userId)
        {
            DateTime utcNow = DateTime.UtcNow;
            DateTime today = new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, 0, 0, 0, DateTimeKind.Utc);
            return EduPalContext.Goals
                .FirstOrDefault(g => g.UserId == userId && g.GoalDate >= today && g.GoalDate < today.AddDays(1));
        }
    }
}
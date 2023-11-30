using Microsoft.EntityFrameworkCore;
using PSI_Project.Models;
using PSI_Project.Data;

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
                // var goalString = ItemToDbString(goal);
                // File.AppendAllText(DbFilePath, goalString + Environment.NewLine);
                int changes = Add(goal);
                //int changes = EduPalContext.SaveChanges();

                return changes > 0;
            }
            catch
            {
                // Log error here
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
                // Log error here
                return false;
            }
        }

        public bool UpdateItem(Goal goalToUpdate)
        {
            try
            {
                //var allGoals = File.ReadAllLines(DbFilePath).ToList();
                //var goalIndex = allGoals.FindIndex(line => line.StartsWith(goalToUpdate.Id + ";"));
                var goalByIndex = EduPalContext.Goals.Find(goalToUpdate.Id);
                
                //if (goalIndex == -1) return false; // Goal not found
                if (goalByIndex == null) return false; // Goal not found
                
                //allGoals[goalIndex] = ItemToDbString(goalToUpdate);
                //File.WriteAllLines(DbFilePath, allGoals);
                EduPalContext.Goals.Update(goalToUpdate);
                int changes = EduPalContext.SaveChanges();

                return changes > 0;
                //return true;
            }
            catch
            {
                // Log error here
                return false;
            }
        }

        // Given a user ID, retrieve the goal for today.
        public Goal? GetTodaysGoalForUser(string userId)
        {
            DateTime utcNow = DateTime.UtcNow;
            DateTime today = new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, 0, 0, 0, DateTimeKind.Utc);
            return EduPalContext.Goals
                .FirstOrDefault(g => g.UserId == userId && g.GoalDate >= today && g.GoalDate < today.AddDays(1));
        }

        // Given a user ID, retrieve all goals for that user.
        public List<Goal> GetAllGoalsForUser(string userId)
        {
            // Ensure that when you load goals, you also load the related subject goals
            return EduPalContext.Goals
                .Where(g => g.User.Id == userId)
                .Include(g => g.SubjectGoals) // Include the SubjectGoals in the query
                .ToList();
        }
        
    }
}
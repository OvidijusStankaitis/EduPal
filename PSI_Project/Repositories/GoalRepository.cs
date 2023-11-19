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

        public bool InsertItem(Goal goal)
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
            DateTime today = DateTime.Now.Date;
            return EduPalContext.Goals.FirstOrDefault(g => g.User.Id == userId && g.GoalDate == today);
        }

        // Given a user ID, retrieve all goals for that user.
        public List<Goal> GetAllGoalsForUser(string userId)
        {
            return EduPalContext.Goals.Where(g => g.User.Id == userId).ToList();
        }
        
    }
}
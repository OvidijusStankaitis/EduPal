using PSI_Project.Models;


namespace PSI_Project.Repositories
{
    public class GoalsRepository : BaseRepository<Goal>
    {
        protected override string DbFilePath => "..//PSI_Project//DB//goals.txt";

        public bool InsertItem(Goal goal)
        {
            try
            {
                var goalString = ItemToDbString(goal);
                File.AppendAllText(DbFilePath, goalString + Environment.NewLine);
                return true;
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
                var allGoals = File.ReadAllLines(DbFilePath).ToList();
                var goalIndex = allGoals.FindIndex(line => line.StartsWith(goalToUpdate.Id + ";"));

                if (goalIndex == -1) return false; // Goal not found

                allGoals[goalIndex] = ItemToDbString(goalToUpdate);
                File.WriteAllLines(DbFilePath, allGoals);

                return true;
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
            return Items.FirstOrDefault(g => g.UserId == userId && g.GoalDate == today);
        }

        // Given a user ID, retrieve all goals for that user.
        public List<Goal> GetAllGoalsForUser(string userId)
        {
            return Items.Where(g => g.UserId == userId).ToList();
        }

        protected override string ItemToDbString(Goal goal)
        {
            var subjectGoalsString = string.Join("|",
                goal.SubjectGoals.Select(sg => $"{sg.SubjectId},{sg.TargetHours},{sg.ActualHoursStudied}"));
            return $"{goal.Id};{goal.UserId};{goal.GoalDate};{subjectGoalsString}";
        }

        protected override Goal StringToItem(string dbString)
        {
            var parts = dbString.Split(";");
            var goal = new Goal(parts[1])
            {
                Id = parts[0],
                GoalDate = DateTime.Parse(parts[2])
            };

            if (parts.Length > 3 && !string.IsNullOrEmpty(parts[3]))
            {
                var subjectGoalsStrings = parts[3].Split("|");
                foreach (var sgs in subjectGoalsStrings)
                {
                    var sgParts = sgs.Split(",");
                    var subjectGoal = new SubjectGoal(sgParts[0], double.Parse(sgParts[1]))
                    {
                        ActualHoursStudied = double.Parse(sgParts[2])
                    };
                    goal.SubjectGoals.Add(subjectGoal);
                }
            }

            return goal;
        }
    }
}
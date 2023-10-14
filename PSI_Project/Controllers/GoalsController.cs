using Microsoft.AspNetCore.Mvc;
using PSI_Project.Services;
using PSI_Project.Models;

namespace PSI_Project.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GoalsController : ControllerBase
    {
        private readonly GoalService _goalService;

        public GoalsController(GoalService goalService)
        {
            _goalService = goalService;
        }

        [HttpPost("add")]
        public IActionResult AddGoal([FromBody] Goal goal)
        {
            // Implementation: Use the GoalService to save the Goal object.
            if (_goalService.AddGoal(goal))
            {
                return Ok(new { success = true, message = "Goal added successfully." });
            }
            return BadRequest(new { success = false, message = "Failed to add goal." });
        }

        [HttpPost("add-subject-goal")]
        public IActionResult AddSubjectGoalToGoal(string goalId, [FromBody] SubjectGoal subjectGoal)
        {
            // Implementation: Use the GoalService to add a SubjectGoal to the specified Goal.
            if (_goalService.AddSubjectGoalToGoal(goalId, subjectGoal))
            {
                return Ok(new { success = true, message = "SubjectGoal added to Goal." });
            }
            return BadRequest(new { success = false, message = "Failed to add SubjectGoal to Goal." });
        }

        [HttpGet("today/{userId}")]
        public IActionResult GetTodaysGoalForUser(string userId)
        {
            // Implementation: Use the GoalService to retrieve today's goal for the given user.
            var goal = _goalService.GetTodaysGoalForUser(userId);
            if (goal != null)
            {
                return Ok(goal);
            }
            return NotFound(new { message = "Today's goal not found for the user." });
        }

        [HttpGet("all/{userId}")]
        public IActionResult GetAllGoalsForUser(string userId)
        {
            // Implementation: Use the GoalService to retrieve all goals for the given user.
            var goals = _goalService.GetAllGoalsForUser(userId);
            return Ok(goals);
        }
    }
}
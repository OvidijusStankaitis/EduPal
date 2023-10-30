﻿﻿using Microsoft.AspNetCore.Mvc;
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
        // DTO object specific to Goals class only (specifically POST update-study-time endpoint)
        // so that's why it is here rather than Models folder.
        public class StudyTimeUpdateRequest
        {
            public string UserId { get; set; }
            public string SubjectId { get; set; }
            public double ElapsedHours { get; set; }
        }

        [HttpPost("create")]
        public IActionResult CreateDailyGoal([FromBody] Goal goalRequest)
        {
            // Check if the user already has a goal for today
            var existingGoal = _goalService.GetTodaysGoalForUser(goalRequest.User.Id);
            if (existingGoal != null)
            {
                return BadRequest(new { success = false, message = "Goal for today already exists." });
            }
            // If no existing goal for today, create the new goal
            if (_goalService.AddGoal(goalRequest))
            {
                return Ok(new { success = true, message = "Goal created successfully." });
            }
            return BadRequest(new { success = false, message = "Failed to create goal." });
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

        [HttpPost("update-study-time")]
        public IActionResult UpdateStudyTime([FromBody] StudyTimeUpdateRequest request)
        {
            if (_goalService.UpdateHoursStudied(request.UserId, request.SubjectId, request.ElapsedHours))
            {
                return Ok(new { success = true, message = "Hours updated successfully." });
            }

            return BadRequest(new { success = false, message = "Failed to update hours." });
        }
    }
}
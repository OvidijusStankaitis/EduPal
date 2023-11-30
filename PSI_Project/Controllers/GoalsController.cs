using System.Transactions;
using Microsoft.AspNetCore.Mvc;
using PSI_Project.Services;
using PSI_Project.Models;
using PSI_Project.Repositories;
using PSI_Project.Requests;

namespace PSI_Project.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GoalsController : ControllerBase
    {
        private readonly GoalService _goalService;
        private readonly SubjectRepository _subjectRepository;
        private readonly ILogger<GoalsController> _logger;

        public GoalsController(ILogger<GoalsController> logger, GoalService goalService, SubjectRepository subjectRepository)
        {
            _logger = logger;
            _goalService = goalService;
            _subjectRepository = subjectRepository;
        }

        [HttpPost("create")]
        public IActionResult CreateGoalWithSubjects([FromBody] CreateGoalRequest request)
        {
            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    // Create a new Goal object
                    var goal = new Goal
                    {
                        UserId = request.UserId,
                        GoalDate = DateTime.UtcNow
                    };
            
                    // Try to add the Goal
                    bool isGoalAdded = _goalService.AddGoal(goal, request.SubjectIds);
                    if (!isGoalAdded)
                    {
                        // If the goal is not added, return a BadRequest response
                        return BadRequest(new { success = false, message = "Unable to create a new goal. Make sure you have completed all existing goals before proceeding." });
                    }
            
                    // If the goal is added, proceed to add subject goals
                    foreach (var subjectId in request.SubjectIds)
                    {
                        var subjectGoal = new SubjectGoal
                        {
                            GoalId = goal.Id,
                            SubjectId = subjectId,
                            TargetHours = request.GoalTime
                        };
                        _goalService.AddSubjectGoal(subjectGoal);
                    }

                    // Complete the transaction
                    transaction.Complete();
                    return Ok(new { success = true, message = "Goal and subject goals created successfully." });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating goal with subjects.");
                    return StatusCode(500, "An error occurred while creating the goal with subjects.");
                }
            }
        }
        
        [HttpGet("subjects")]
        public IActionResult GetAllSubjects()
        {
            try
            {
                var subjects = _subjectRepository.GetSubjectsList();
                return Ok(subjects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subjects");
                return StatusCode(500, "An error occurred while fetching subjects");
            }
        }

        [HttpGet("today/{userId}")]
        public IActionResult GetTodaysGoalForUser(string userId)
        {
            // Implementation: Use the GoalService to retrieve today's goal for the given user.
            try
            {
                var goal = _goalService.GetTodaysGoalForUser(userId);
                if (goal != null)
                {
                    return Ok(goal);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Couldn't get user's {userId} today's goal", userId);
                return StatusCode(500, "An error occured while getting user's today's goal");
            }

            return NotFound(new { message = "Today's goal not found for the user." });
        }

        [HttpGet("all/{userId}")]
        public IActionResult GetAllGoalsForUser(string userId)
        {
            // Implementation: Use the GoalService to retrieve all goals for the given user.
            try
            {
                var goals = _goalService.GetAllGoalsForUser(userId);
                return Ok(goals);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Couldn't get user {userId} goals", userId);
                return StatusCode(500, "An error occured while getting user goals");
            }
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
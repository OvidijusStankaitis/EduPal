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
        private readonly IUserAuthService _userAuthService;
        private readonly ILogger<GoalsController> _logger;

        public GoalsController(ILogger<GoalsController> logger, GoalService goalService, SubjectRepository subjectRepository, IUserAuthService userAuthService)
        {
            _logger = logger;
            _goalService = goalService;
            _subjectRepository = subjectRepository;
            _userAuthService = userAuthService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateGoalWithSubjects([FromBody] CreateGoalRequest request)
        {
            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {   
                    User? user = await _userAuthService.GetUser(HttpContext); 
                    // Create a new Goal object
                    var goal = new Goal
                    {
                        UserId = user.Id,
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
        
        [HttpGet("view-all")]
        public async Task <IActionResult> GetAllGoalsForUserWithDetails()
        {
            User? user = await _userAuthService.GetUser(HttpContext);
            string userId = user.Id;
            try
            {
                var goalsWithDetails = _goalService.GetAllGoalsForUserWithDetails(userId);
                return Ok(goalsWithDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred when getting all goals with details for user {UserId}", userId);
                return StatusCode(500, "An error occurred while getting goals with details");
            }
        }

        [HttpPost("update-study-time")]
        public async Task <IActionResult> UpdateStudyTime([FromBody] StudyTimeUpdateRequest request)
        {
            User? user = await _userAuthService.GetUser(HttpContext);
            string userId = user.Id;
            if (_goalService.UpdateHoursStudied(userId, request.SubjectId, request.ElapsedHours))
            {
                return Ok(new { success = true, message = "Hours updated successfully." });
            }
            Console.WriteLine("Elapsed hours: " + request.ElapsedHours);
            Console.WriteLine("Subject ID: " + request.SubjectId);
            Console.WriteLine("User ID: " + userId);

            return BadRequest(new { success = false, message = "Failed to update hours." });
        }
        
        [HttpGet("current-subject")]
        public async Task <IActionResult> GetCurrentSubject()
        {
            User? user = await _userAuthService.GetUser(HttpContext);
            string userId = user.Id;
            var currentSubject = _goalService.GetCurrentSubjectForUser(userId);
            if (currentSubject == null)
            {
                return NotFound(new { success = false, message = "No current subject found." });
            }

            return Ok(new { success = true, currentSubjectId = currentSubject.SubjectId });
        }
    }
}
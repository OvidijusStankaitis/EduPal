using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace PSI_Project.Controllers;

[ApiController]
[Route("[controller]")]
public class SubjectController : ControllerBase
{
    [HttpGet]
    public IActionResult ListSubjects()
    {
        //TESTS
        // SubjectHandler.CreateSubject("a", "b");
        // SubjectHandler.CreateSubject("a1", "b1");
        // SubjectHandler.CreateSubject("a12", "b12");
        
        return Ok(SubjectHandler.SubjectList);
    }
    
    [HttpGet("{subjectName}")]
    public IActionResult ListTopics(string subjectName)
    {
        List<Topic> subjectTopics = new List<Topic>();
        foreach (var topic in TopicHandler.TopicList)
        {
            if (topic.SubjectName == subjectName)
            {
                subjectTopics.Add(topic);
            }
        }
        return Ok(subjectTopics); 
    }

}
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
        //SubjectHandler.CreateSubject("a", "b");
        //SubjectHandler.CreateSubject("a1", "b1");
        //SubjectHandler.CreateSubject("a12", "b12");

        return Ok(SubjectHandler.SubjectList);
    }
}
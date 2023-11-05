using Microsoft.AspNetCore.Mvc;
using PSI_Project.DTO;
using PSI_Project.Exceptions;
using PSI_Project.Models;
using PSI_Project.Repositories;

namespace PSI_Project.Controllers;

[ApiController]
[Route("[controller]")]
public class ConspectusController : ControllerBase
{
    private readonly ConspectusRepository _conspectusRepository;

    public ConspectusController(ConspectusRepository conspectusRepository)
    {
        _conspectusRepository = conspectusRepository;
    }

    [HttpGet("get/{conspectusId}")]
    public IActionResult GetConspectus(string conspectusId)
    {
        try
        {
            Stream pdfStream = _conspectusRepository.GetPdfStream(conspectusId);
            return File(pdfStream, "application/pdf");
        }
        catch (Exception ex)
        {
            // TODO: log errors
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            
            return NotFound(new { error = "File not found." });
        }
    }

    [HttpGet("list/{topicId}")]
    public IActionResult GetTopicFiles(string topicId)
    {
        try
        {
            return Ok(_conspectusRepository.GetConspectusListByTopicId(topicId));
        }
        catch (Exception ex)
        {
            // TODO: log errors
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            
            return BadRequest("An error occured while uploading file");
        }
    }

    [HttpPost("upload/{topicId}")]
    public IActionResult UploadFiles(string topicId, List<IFormFile> files)
    {
        try
        {
            return Ok(_conspectusRepository.Upload(topicId, files).ToList());
        }
        catch (Exception ex)
        {
            // TODO: log errors
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            
            return BadRequest("An error occured while uploading file");
        }
    }
    
    [HttpGet("download/{conspectusId}")]
    public IActionResult DownloadFile(string conspectusId)
    {
        try
        {
            ConspectusFileContentDTO response = _conspectusRepository.Download(conspectusId);
            Response.Headers.Add("Content-Disposition", "attachment; filename=" + response.Name);
            return response.FileContent;
        }
        catch (Exception ex)
        {
            // TODO: log errors
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            
            return BadRequest("An error occured while downloading file");
        }
    }

    [HttpDelete("{conspectusId}/delete")]
    public IActionResult DeleteFile(string conspectusId)
    {
        try
        {
            _conspectusRepository.Remove(conspectusId);
            return Ok("File has been successfully deleted");
        }
        catch (Exception ex)
        {
            // TODO: log errors
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            
            return BadRequest("An error occured while deleting file");
        }
    }
    
    [HttpPost("rateUp/{conspectusId}")]
    public IActionResult RateConspectusUp(string conspectusId)
    {
        try
        {
            Conspectus ratedConspectus = _conspectusRepository.ChangeRating(conspectusId, true);
            return Ok(ratedConspectus);
        }
        catch (Exception ex)
        {
            // TODO: log errors
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
        }
        
        return NotFound(new { error = "File not found in database." });
    }
    
    [HttpPost("rateDown/{conspectusId}")]
    public IActionResult RateConspectusDown(string conspectusId)
    {
        try
        {
            Conspectus ratedConspectus = _conspectusRepository.ChangeRating(conspectusId, false);
            return Ok(ratedConspectus);
        }
        catch (Exception ex)
        {
            // TODO: log errors
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            
            return NotFound(new { error = "File not found in database." }); 
        }
    }
}

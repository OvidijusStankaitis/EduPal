using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PSI_Project.DTO;
using PSI_Project.Exceptions;
using PSI_Project.Models;
using PSI_Project.Repositories;
using PSI_Project.Services;

namespace PSI_Project.Controllers;


[Authorize]
[ApiController]
[Route("[controller]")]
public class ConspectusController : ControllerBase
{
    private readonly ConspectusService _conspectusService;
    private readonly ConspectusRepository _conspectusRepository;
    private readonly ILogger<ConspectusController> _logger;
    
    public ConspectusController(ILogger<ConspectusController> logger, ConspectusRepository conspectusRepository, ConspectusService conspectusService)
    {
        _logger = logger;
        _conspectusService = conspectusService;
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
            _logger.LogError(ex, "Couldn't get conspectus {conspectusId} information", conspectusId);
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
            _logger.LogError(ex, "Couldn't list topic {topicId} conspectuses", topicId);            
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
            _logger.LogError(ex, "Couldn't add new conspectus(es)");
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
            _logger.LogError("Couldn't download conspectus {conspectusId}", conspectusId);
            return BadRequest("An error occured while downloading file");
        }
    }

    [HttpDelete("delete/{conspectusId}")]
    public IActionResult DeleteFile(string conspectusId)
    {
        try
        {
            _conspectusRepository.Remove(conspectusId);
            return Ok("File has been successfully deleted");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Couldn't delete conspectus {conspectusId}", conspectusId);
            return BadRequest("An error occured while deleting file");
        }
    }
    
    [HttpPost("rate-up/{conspectusId}")]
    public IActionResult RateConspectusUp(string conspectusId)
    {
        try
        {
            Conspectus ratedConspectus = _conspectusRepository.ChangeRating(conspectusId, true);
            return Ok(ratedConspectus);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Couldn't rate up conspectus {conspectusId}", conspectusId);
        }
        
        return NotFound(new { error = "File not found in database." });
    }
    
    [HttpPost("rate-down/{conspectusId}")]
    public IActionResult RateConspectusDown(string conspectusId)
    {
        try
        {
            Conspectus ratedConspectus = _conspectusRepository.ChangeRating(conspectusId, false);
            return Ok(ratedConspectus);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Couldn't rate down conspectus {conspectusId}", conspectusId);
            return NotFound(new { error = "File not found in database." }); 
        }
    }

}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PSI_Project.DTO;
using PSI_Project.Models;
using PSI_Project.Repositories;
using PSI_Project.Services;

namespace PSI_Project.Controllers;

[ApiController]
[Route("[controller]")]
public class ConspectusController : ControllerBase
{
    private readonly ConspectusService _conspectusService;
    private readonly ConspectusRepository _conspectusRepository;
    private readonly ILogger<ConspectusController> _logger;

    public ConspectusController(ILogger<ConspectusController> logger, ConspectusRepository conspectusRepository,
        ConspectusService conspectusService)
    {
        _logger = logger;
        _conspectusService = conspectusService;
        _conspectusRepository = conspectusRepository;
    }

    [Authorize]
    [HttpGet("get/{conspectusId}")]
    public async Task<IActionResult> GetConspectusAsync(string conspectusId)
    {
        try
        {
            Stream pdfStream = await _conspectusRepository.GetPdfStreamAsync(conspectusId);
            return File(pdfStream, "application/pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Couldn't get conspectus {conspectusId} information", conspectusId);
            return NotFound(new { error = "File not found." });
        }
    }

    [Authorize]
    [HttpGet("list/{topicId}")]
    public async Task<IActionResult> GetTopicFilesAsync(string topicId)
    {
        try
        {
            return Ok(await _conspectusService.GetConspectusesAsync(topicId));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Couldn't list topic {topicId} conspectuses", topicId);
            return BadRequest("An error occurred while listing conspectuses");
        }
    }

    [Authorize]
    [HttpPost("upload/{topicId}")]
    public async Task<IActionResult> UploadFilesAsync(string topicId, List<IFormFile> files)
    {
        try
        {
            var uploadedConspectuses = await _conspectusRepository.UploadAsync(topicId, files);
            return Ok(uploadedConspectuses.ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Couldn't add new conspectus(es)");
            return BadRequest("An error occurred while uploading conspectuses");
        }
    }
    
    [Authorize]
    [HttpDelete("delete/{conspectusId}")]
    public async Task<IActionResult> DeleteFileAsync(string conspectusId)
    {
        try
        {
            await _conspectusRepository.RemoveAsync(conspectusId);
            return Ok("File has been successfully deleted");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Couldn't delete conspectus {conspectusId}", conspectusId);
            return BadRequest("An error occurred while deleting file");
        }
    }

    [Authorize]
    [HttpGet("download/{conspectusId}")]
    public async Task<IActionResult> DownloadFileAsync(string conspectusId)
    {
        try
        {
            ConspectusFileContentDTO response = await _conspectusRepository.DownloadAsync(conspectusId);
            Response.Headers.Add("Content-Disposition", "attachment; filename=" + response.Name);
            return response.FileContent;
        }
        catch (Exception ex)
        {
            _logger.LogError("Couldn't download conspectus {conspectusId}", conspectusId);
            return BadRequest("An error occurred while downloading file");
        }
    }

    [Authorize]
    [HttpPost("rate-up/{conspectusId}")]
    public async Task<IActionResult> RateConspectusUpAsync(string conspectusId)
    {
        try
        {
            Conspectus ratedConspectus = await _conspectusRepository.ChangeRatingAsync(conspectusId, true);
            return Ok(ratedConspectus);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Couldn't rate up conspectus {conspectusId}", conspectusId);
            return NotFound(new { error = "File not found in database." });
        }

    }

    [Authorize]
    [HttpPost("rate-down/{conspectusId}")]
    public async Task<IActionResult> RateConspectusDownAsync(string conspectusId)
    {
        try
        {
            Conspectus ratedConspectus = await _conspectusRepository.ChangeRatingAsync(conspectusId, false);
            return Ok(ratedConspectus);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Couldn't rate down conspectus {conspectusId}", conspectusId);
            return NotFound(new { error = "File not found in database." });
        }
    }
}
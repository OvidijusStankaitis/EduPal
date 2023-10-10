using Microsoft.AspNetCore.Mvc;
using PSI_Project.Repositories;

namespace PSI_Project.Controllers;

[ApiController]
[Route("[controller]")]
public class ConspectusController : ControllerBase
{
    private readonly ConspectusRepository _conspectusRepository = new ConspectusRepository();

    [HttpGet("get/{conspectusId}")]
    public IActionResult GetConspectus(string conspectusId)
    {
        Stream? pdfStream = _conspectusRepository.GetConspectusPdfStream(conspectusId);
        return pdfStream != null
            ? File(pdfStream, "application/pdf")
            : NotFound(new { error = "File not found." });
    }

    [HttpGet("list/{topicId}")]
    public IActionResult GetTopicFiles(string topicId)
    {   
        return Ok(_conspectusRepository.GetConspectusByTopicId(topicId));
    }

    [HttpPost("upload/{topicId}")]
    public IActionResult UploadFiles(string topicId, List<IFormFile> files)
    {
        return Ok(_conspectusRepository.UploadConspectus(topicId, files));
    }

    [HttpGet("download/{conspectusId}")]
    public IActionResult DownloadFile(string conspectusId)
    {
        FileContentResult? fileContent = _conspectusRepository.DownloadConspectus(conspectusId);
        if (fileContent == null)
            return NotFound();
        
        string? conspectusPath = _conspectusRepository.GetConspectusPath(conspectusId);
        Response.Headers.Add("Content-Disposition", "attachment; filename=" + Path.GetFileName(conspectusPath));
        
        return fileContent;
    }

    [HttpDelete("{conspectusId}/delete")]
    public IActionResult DeleteFile(string conspectusId)
    {
        return _conspectusRepository.RemoveItemById(conspectusId) 
            ? Ok("File has been successfully deleted") 
            : BadRequest("An error occured while deleting file");
    }
}

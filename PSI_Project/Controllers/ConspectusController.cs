using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using PSI_Project.Repositories;

namespace PSI_Project.Controllers;

[ApiController]
[Route("[controller]")]
public class ConspectusController : ControllerBase
{
    private ConspectusRepository _conspectusRepository = new ConspectusRepository();

    [HttpGet("get/{conspectusId}")]
    public IActionResult GetConspectus(string conspectusId)
    {
        try
        {
            Conspectus? conspectus = _conspectusRepository.GetItemById(conspectusId);
        
            if (conspectus == null)
                return NotFound(new { error = "File not found in database." });
        
            string dirPath = Path.GetDirectoryName(conspectus.Path);
            string filename = Path.GetFileName(conspectus.Path);
        
            IFileProvider provider = new PhysicalFileProvider(dirPath);
            IFileInfo fileInfo = provider.GetFileInfo(filename);
            if (!fileInfo.Exists)
                return NotFound(new { error = "File not found on server." });

            var readStream = fileInfo.CreateReadStream();

            return File(readStream, "application/pdf", filename);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpGet("list/{topicName}")]
    public IActionResult GetTopicFiles(string topicName)
    {   
        return Ok(_conspectusRepository.GetConspectusListByTopicName(topicName));
    }

    [HttpPost("upload/{topicName}")]
    public IActionResult UploadFiles(string topicName, List<IFormFile> files)
    {
        foreach (var formFile in files)
        {
            string fileName = formFile.FileName;
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Files", fileName);

            // copying file to files folder
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                formFile.CopyTo(fileStream);
            }

            _conspectusRepository.InsertItem(new Conspectus(topicName, filePath));
        }
    
        return Ok(_conspectusRepository.GetConspectusListByTopicName(topicName));
    }

    [HttpGet("download/{conspectusId}")]
    public IActionResult DownloadFile(string conspectusId)
    {
        Conspectus? conspectus = _conspectusRepository.GetItemById(conspectusId);
        if (conspectus == null)
            return NotFound();
    
        string filePath = conspectus.Path;
        if (System.IO.File.Exists(filePath))
        {
            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            var response = new FileContentResult(fileBytes, "application/pdf")
            {
                FileDownloadName = Path.GetFileName(filePath) // Set the desired filename
            };
            Response.Headers.Add("Content-Disposition", "attachment; filename=" + Path.GetFileName(filePath));
            return response;
        }
    
        return NotFound();
    }

    [HttpDelete("delete/{conspectusId}")]
    public void DeleteFile(string conspectusId)
    {
        Conspectus? conspectus = _conspectusRepository.GetItemById(conspectusId);
        if (conspectus == null)
            return;
        
        string filePath = conspectus.Path;
        try
        {
            _conspectusRepository.RemoveItem(conspectusId);

            // Check if the file is used in other topics before deleting
            if (!_conspectusRepository.IsFileUsedInOtherTopics(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}

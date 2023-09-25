using Microsoft.AspNetCore.Mvc;

namespace PSI_Project.Controllers;

[ApiController]
[Route("[controller]")]
public class ConspectusController : ControllerBase
{
    private ConspectusHandler _conspectusHandler = new ConspectusHandler();
    
    [HttpGet("list")]
    public IActionResult ListUploadedFiles()
    {   
        return Ok(_conspectusHandler.ConspectusList);
    }
    
    [HttpPost("upload")]
    public IActionResult UploadFiles(List<IFormFile> files)
    {
        foreach (var formFile in files)
        {
            string fileName = formFile.FileName;
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Files", fileName);

            // copying file to files folder
            FileStream fileStream = new FileStream(filePath, FileMode.Create);
            formFile.CopyTo(fileStream);
            
            // saving file to conspectus.txt (temp db)
            _conspectusHandler.UploadConspectus(new Conspectus(filePath));
        }
        
        return Ok(_conspectusHandler.ConspectusList);
    }

    [HttpGet("download/{conspectusId}")]
    public IActionResult DownloadFile(string conspectusId)
    {
        // checking if there is conspectus with such id
        Conspectus? conspectus = _conspectusHandler.GetConspectusById(conspectusId);
        if (conspectus == null)
            return NotFound();
        
        // checking if the file with such path exists
        string filePath = conspectus.Path;
        if (System.IO.File.Exists(filePath))
            return PhysicalFile(filePath, "application/octet-stream");
        
        // if there is no file, return not found error
        return NotFound();
    }

    [HttpDelete("delete/{conspectusId}")]
    public void DeleteFile(string conspectusId)
    {
        Conspectus? conspectus = _conspectusHandler.GetConspectusById(conspectusId);
        if (conspectus == null)
            return;
                
        string filePath = conspectus.Path;
        try
        {
            if (System.IO.File.Exists(filePath))
            {
                // deleting conspectus from filesystem
                System.IO.File.Delete(filePath);
                
                // deleting conspectus from conspectus.txt (temp db)
                _conspectusHandler.RemoveConspectus(conspectusId);
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
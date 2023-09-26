using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

namespace PSI_Project.Controllers;

[ApiController]
[Route("[controller]")]
public class ConspectusController : ControllerBase
{
    private ConspectusHandler _conspectusHandler = new ConspectusHandler();
    
    [HttpGet("get/{conspectusId}")]
    public IActionResult GetConspectus(string conspectusId)
    {
        try
        {
            Conspectus? conspectus = _conspectusHandler.GetConspectusById(conspectusId);
        
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
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                formFile.CopyTo(fileStream);
            }
        
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
            return PhysicalFile(filePath, "application/pdf");
        
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
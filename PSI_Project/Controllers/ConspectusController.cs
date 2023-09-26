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
            
            // saving file to db (conspectus.txt file)
            Conspectus conspectus = new Conspectus(filePath);
            _conspectusHandler.UploadConspectus(conspectus);
        }
        
        return Ok(_conspectusHandler.ConspectusList);
    }

    [HttpGet("download/{filename}")]
    public IActionResult DownloadFile(string filename)
    {
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Files", filename);

        if (System.IO.File.Exists(filePath))
        {
            return PhysicalFile(filePath, "application/octet-stream");
        }

        return NotFound();
    }

    [HttpDelete("delete/{filename}")]
    public void DeleteFile(string filename)
    {
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Files", filename);

        try
        {
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
                Conspectus conspectus = new Conspectus(filePath);
                _conspectusHandler.RemoveConspectus(conspectus);
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
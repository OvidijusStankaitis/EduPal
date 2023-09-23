using Microsoft.AspNetCore.Mvc;

namespace PSI_Project.Controllers;

[ApiController]
[Route("[controller]")]
public class FileHandlingController : ControllerBase
{
    [HttpPost("upload")]
    public IActionResult UploadFiles(List<IFormFile> files)
    {
        foreach (var formFile in files)
        {
            string fileName = formFile.FileName;
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Files", fileName);

            FileStream fileStream = new FileStream(filePath, FileMode.Create);
            formFile.CopyTo(fileStream);
        }
        
        return Ok("Files has been successfully uploaded");
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
    
    [HttpGet("list")]
    public IActionResult ListUploadedFiles()
    {
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Files");
        return Ok(Directory.GetFiles(filePath));
    }

    [HttpDelete("delete/{filename}")]
    public void DeleteFile(string filename)
    {
        
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Files", filename);
        Console.WriteLine(filePath);
        try
        {
            if (System.IO.File.Exists(filePath))
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
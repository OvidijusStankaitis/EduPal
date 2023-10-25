using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using PSI_Project.Data;
using PSI_Project.DTO;
using PSI_Project.Models;

namespace PSI_Project.Repositories;
public class ConspectusRepository : Repository<Conspectus>
{
    public EduPalDatabaseContext EduPalContext => Context as EduPalDatabaseContext;

    public ConspectusRepository(EduPalDatabaseContext context) : base(context)
    {
    }

    public IEnumerable<Conspectus> GetConspectusListByTopicId(string topicId)
    {
        return 
            EduPalContext.Conspectuses.Where(conspectus => conspectus.Topic.Id == topicId);
    }

    public Stream? GetPdfStream(string conspectusId)
    {
        try
        {
            Conspectus? conspectus = Get(conspectusId);
            if (conspectus == null)
                return null;
        
            string? dirPath = Path.GetDirectoryName(conspectus.Path);
            string filename = Path.GetFileName(conspectus.Path);
        
            IFileProvider provider = new PhysicalFileProvider(dirPath);
            IFileInfo fileInfo = provider.GetFileInfo(filename);
            if (!fileInfo.Exists)
                return null;

            return fileInfo.CreateReadStream();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }

    public ConspectusFileContentDTO? Download(string conspectusId)
    {
        Conspectus? conspectus = Get(conspectusId);
        if (conspectus == null)
            return null;
        
        if (File.Exists(conspectus.Path))
        {
            var fileBytes = File.ReadAllBytes(conspectus.Path);
            var fileContent = new FileContentResult(fileBytes, "application/pdf")
            {
                FileDownloadName = Path.GetFileName(conspectus.Name) 
            };
            
            return new ConspectusFileContentDTO(conspectus.Name, fileContent);  
        }

        return null;
    }

    public IEnumerable<Conspectus> Upload(string topicId, List<IFormFile> files)
    {
        foreach (var formFile in files)
        {
            string fileName = formFile.FileName;
            if (!fileName.IsValidFileName())
            {
                Console.WriteLine($"The file {fileName} is not a valid PDF format.");
                continue;
            }

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Files", fileName);
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                formFile.CopyTo(fileStream);
            }
            
            Conspectus conspectus = new()
            {
                Name = fileName,
                Path = filePath,
                Topic = EduPalContext.Topics.Find(topicId)
            };
            
            Add(conspectus);
            EduPalContext.SaveChanges();
        }

        return GetConspectusListByTopicId(topicId);
    }
    
    public bool ChangeRating(string conspectusId, bool toIncrease)
    {
        Conspectus? conspectus = Get(conspectusId);
        if (conspectus is null)
            return false;

        conspectus.Rating += toIncrease ? +1 : -1;
        int changes = EduPalContext.SaveChanges();
        return changes > 0;
    }

    public bool Remove(string conspectusId)
    {
        Conspectus? conspectus = Get(conspectusId);
        if (conspectus is null)
            return false;
        
        Remove(conspectus);
        int changes = EduPalContext.SaveChanges();

        string filePath = conspectus.Path;
        DeleteFile(filePath);

        return changes > 0;
    }

    public void DeleteFile(string filePath)
    {
        try
        {
            if (!IsFileUsed(filePath))
                File.Delete(filePath);
        }
        catch (IOException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    
    public bool IsFileUsed(string filePath)
    {
        return EduPalContext.Conspectuses.Any(item => item.Path == filePath);
    }
}
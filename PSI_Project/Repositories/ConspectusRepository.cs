using System.Runtime.InteropServices.JavaScript;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using PSI_Project.Data;
using PSI_Project.DTO;
using PSI_Project.Exceptions;
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
        return EduPalContext.Conspectuses.Where(conspectus => conspectus.Topic.Id == topicId);
    }

    public Stream GetPdfStream(string conspectusId)
    {
        Conspectus conspectus = Get(conspectusId);
        string dirPath = Path.GetDirectoryName(conspectus.Path)!;
        string filename = Path.GetFileName(conspectus.Path);
        
        IFileProvider provider = new PhysicalFileProvider(dirPath);
        IFileInfo fileInfo = provider.GetFileInfo(filename);
        
        return fileInfo.CreateReadStream();
    }

    public ConspectusFileContentDTO Download(string conspectusId)
    {
        Conspectus conspectus = Get(conspectusId);
        byte[] fileBytes = File.ReadAllBytes(conspectus.Path);
        FileContentResult fileContent = new FileContentResult(fileBytes, "application/pdf")
        {
            FileDownloadName = Path.GetFileName(conspectus.Name) 
        };
    
        return new ConspectusFileContentDTO(conspectus.Name, fileContent);
    }

    public IEnumerable<Conspectus> Upload(string topicId, List<IFormFile> files)
    {
        foreach (var formFile in files)
        {
            try
            {
                string fileName = formFile.FileName;
                if (!fileName.IsValidFileName()) // TODO: make useful validation
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
            catch (Exception ex)
            {
                // TODO: log errors
                // TODO: mb do something with un-uploaded file
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw new EntityCreationException("Error occured while uploading one of the files", ex);
            }
        }

        return GetConspectusListByTopicId(topicId); // TODO: mb other return type is better?
    }
    
    public Conspectus ChangeRating(string conspectusId, bool toIncrease)
    {
        Conspectus conspectus = Get(conspectusId);
        conspectus.Rating += toIncrease ? +1 : -1;
        
        EduPalContext.SaveChanges();
        return conspectus;
    }
    
    public void Remove(string conspectusId)
    {
        Conspectus conspectus = Get(conspectusId);
        Remove(conspectus);
        EduPalContext.SaveChanges();

        try
        {
            string filePath = conspectus.Path;
            if (!IsFileUsed(filePath))
                File.Delete(filePath);
        }
        catch (Exception ex)
        {
            // TODO: do something with deleted files from db
            throw new EntityDeletionException("Couldn't delete conspectus", ex);
        }
    }
    
    public bool IsFileUsed(string filePath)
    {
        return EduPalContext.Conspectuses.Any(item => item.Path == filePath);
    }
}
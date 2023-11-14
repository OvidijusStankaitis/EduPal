using System.Runtime.InteropServices.JavaScript;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.FileProviders;
using PSI_Project.Data;
using PSI_Project.DTO;
using PSI_Project.Exceptions;
using PSI_Project.Models;

namespace PSI_Project.Repositories;
public class ConspectusRepository : Repository<Conspectus>
{
    public EduPalDatabaseContext EduPalContext => Context as EduPalDatabaseContext;
    private static readonly object _deleteLock = new object();

    public ConspectusRepository(EduPalDatabaseContext context) : base(context)
    {
    }

    public IEnumerable<Conspectus> GetConspectusListByTopicId(string topicId)
    {
        return Find(conspectus => conspectus.Topic.Id == topicId);
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
            string fileName = formFile.FileName;
            if (!fileName.IsValidFileName())
            {
                Console.WriteLine($"The file {fileName} is not a valid PDF format.");
                continue;
            }

            string filePath;
            try
            {
                filePath = Path.Combine(Directory.GetCurrentDirectory(), "Files", fileName);
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    formFile.CopyTo(fileStream);
                }
            }
            catch (Exception ex)
            {
                // Create at least 1 exception type and throw it; meaningfully deal with it; 
                throw new EntityCreationException("Error occured while uploading one of the files", ex);
            }

            try
            {
                Topic? topic = EduPalContext.Topics.Find(topicId);
                if (topic == null)
                {
                    // Create at least 1 exception type and throw it; meaningfully deal with it; 
                    throw new ObjectNotFoundException("Couldn't find topic with specified id");
                }

                Conspectus conspectus = new()
                {
                    Name = fileName,
                    Path = filePath,
                    Topic = topic
                };

                Add(conspectus);
                EduPalContext.SaveChanges();
            }
            catch (Exception ex)
            {
                File.Delete(filePath);
                // Create at least 1 exception type and throw it; meaningfully deal with it; 
                throw new EntityCreationException("Error occured while uploading one of the files", ex);
            }
        }

        return GetConspectusListByTopicId(topicId);
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

        // Use Monitor for thread safety
        Monitor.Enter(_deleteLock); // 8. Use at least 1 concurrent collection or Monitor;
        try
        {
            string filePath = conspectus.Path;
        
            // Remove the conspectus from the context
            Remove(conspectus);
            EduPalContext.SaveChanges();

            // Try to delete the file
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Console.WriteLine($"File deleted: {filePath}");
            }
            else
            {
                Console.WriteLine($"File not found: {filePath}");
            }
        }
        catch (Exception ex)
        {
            EntityEntry<Conspectus> entry = EduPalContext.Entry(conspectus);
            entry.State = EntityState.Unchanged;
            // Create at least 1 exception type and throw it; meaningfully deal with it; 
            throw new EntityDeletionException("Couldn't delete conspectus", ex);
        }
        finally
        {
            Monitor.Exit(_deleteLock);
        }
    }
    
    public bool IsFileUsed(string filePath)
    {
        return EduPalContext.Conspectuses.Any(item => item.Path == filePath);
    }
}
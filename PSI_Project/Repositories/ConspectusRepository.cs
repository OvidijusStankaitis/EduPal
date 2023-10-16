using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using PSI_Project.DTO;
using PSI_Project.Models;

namespace PSI_Project.Repositories;
public class ConspectusRepository : BaseRepository<Conspectus>
{
    protected override string DbFilePath => "..//PSI_Project//DB//conspectus.txt";
    
    public List<Conspectus> GetConspectusByTopicId(string topicId)
    {
        return Items.Where(conspectus => conspectus.TopicId == topicId).ToList();
    }
    
    public string? GetConspectusPath(string conspectusId)
    {
        return GetItemById(conspectusId)?.Path;
    }

    public Stream? GetConspectusPdfStream(string conspectusId)
    {
        try
        {
            Conspectus? conspectus = GetItemById(conspectusId);
            if (conspectus == null)
                return null;
        
            string dirPath = Path.GetDirectoryName(conspectus.Path);
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

    public ConspectusFileContentDTO? DownloadConspectus(string itemId)
    {
        Conspectus? conspectus = GetItemById(itemId);
        if (conspectus == null)
            return null;
        
        if (File.Exists(conspectus.Path))
        {
            var fileBytes = File.ReadAllBytes(conspectus.Path);
            var fileContent = new FileContentResult(fileBytes, "application/pdf")
            {
                FileDownloadName = Path.GetFileName(conspectus.Name) 
            };
            
            return new ConspectusFileContentDTO(conspectus.Name, fileContent);  // 1: using record
        }

        return null;
    }
    
    public List<Conspectus> UploadConspectus(string topicId, List<IFormFile> files)
    {
        foreach (var formFile in files) // 5: iterating through collection the right way
        {
            string fileName = formFile.FileName;
            
            if (!fileName.IsValidFileName()) // 4. Extension method usage
            {
                Console.WriteLine($"The file {fileName} is not a valid PDF format.");
                continue;
            }

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Files", fileName);
            
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                formFile.CopyTo(fileStream);
            }

            InsertItem(new Conspectus(topicId, filePath, fileName));
        }

        return GetConspectusByTopicId(topicId);
    }

    public override bool RemoveItemById(string itemId)
    {
        Conspectus? conspectus = GetItemById(itemId);
        if (conspectus == null)
            return false;
        
        string filePath = conspectus.Path;
        try
        {
            RemoveItemFromDB(conspectus.Id);
            
            if (!IsFileUsedInOtherTopics(filePath))
            {
                File.Delete(filePath);
            }
            
            return true;
        }
        catch (IOException ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }
    
    private bool IsFileUsedInOtherTopics(string filePath)
    {
        return Items.Any(conspectus => conspectus.Path == filePath);
    }
    
    protected override string ItemToDbString(Conspectus item)
    {
        return $"{item.Id};{item.Name};{item.TopicId};{item.Path};";
    }
    
    protected override Conspectus StringToItem(string dbString)
    {
        String[] fields = dbString.Split(";");
        Conspectus newConspectus = new Conspectus(name: fields[1], topicId: fields[2], path: fields[3]) // 3: named argument usage
        {
            Id = fields[0]
        };
        return newConspectus;
    }
}
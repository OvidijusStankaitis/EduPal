using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using PSI_Project.Models;

namespace PSI_Project.Repositories;
public class ConspectusRepository : BaseRepository<Conspectus>
{
    protected override string DbFilePath => "..//PSI_Project//DB//conspectus.txt";
    
    public List<Conspectus> GetConspectusListByTopicName(string topicName)
    {
        return Items.Where(conspectus => conspectus.TopicName == topicName).ToList();
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

    public string GetConspectusPath(string conspectusId)
    {
        Conspectus conspectus = GetItemById(conspectusId);
        return conspectus.Path;
    }

    public FileContentResult? DownloadConspectus(string itemId)
    {
        Conspectus? conspectus = GetItemById(itemId);
        if (conspectus == null)
            return null;
        
        string filePath = conspectus.Path;
        if (File.Exists(filePath))
        {
            var fileBytes = File.ReadAllBytes(filePath);
            var response = new FileContentResult(fileBytes, "application/pdf")
            {
                FileDownloadName = Path.GetFileName(filePath) // Set the desired filename
            };
            
            return response;
        }

        return null;
    }

    public List<Conspectus> UploadConspectus(string topicName, List<IFormFile> files)
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

            InsertItem(new Conspectus(topicName, filePath));
        }

        return GetConspectusListByTopicName(topicName);
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

            // Check if the file is used in other topics before deleting
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
    
    public bool IsFileUsedInOtherTopics(string filePath)
    {
        return Items.Any(conspectus => conspectus.Path == filePath);
    }
    
    protected override string ItemToDbString(Conspectus item)
    {
        return $"{item.Id};{item.TopicName};{item.Path};";
    }
    
    protected override Conspectus StringToItem(string dbString)
    {
        String[] fields = dbString.Split(";");
        Conspectus newConspectus = new Conspectus(fields[1], fields[2]);
        newConspectus.Id = fields[0];
        return newConspectus;
    }
}
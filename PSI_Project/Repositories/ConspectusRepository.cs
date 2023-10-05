﻿using PSI_Project.Models;

namespace PSI_Project.Repositories;
public class ConspectusRepository : BaseRepository<Conspectus>
{
    protected override string DbFilePath => "..//PSI_Project//DB//conspectus.txt";
    
    public List<Conspectus> GetConspectusListByTopicName(string topicName)
    {
        return Items.Where(conspectus => conspectus.TopicName == topicName).ToList();
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
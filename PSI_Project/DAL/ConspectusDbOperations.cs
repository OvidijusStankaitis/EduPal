namespace PSI_Project.DAL;

public class ConspectusDbOperations : EntityDbOperations<Conspectus>
{
    protected override string DbFilePath => "..//PSI_Project//DB//conspectus.txt";
    
    protected override string ItemToDbString(Conspectus item)
    {
        return $"{item.Id};{item.TopicId};{item.Path}";
    }
    
    protected override Conspectus StringToItem(string dbString)
    {
        String[] fields = dbString.Split(";");
        return new Conspectus(fields[0], fields[1], fields[2]);
    }

    public List<Conspectus> GetConspectusListByTopicId(string topicId)
    {
        List<Conspectus> conspectusList = new List<Conspectus>();
        
        using (StreamReader sr = File.OpenText(DbFilePath))
        {
            string? line;
            while ((line = sr.ReadLine()) != null)
            {
                Conspectus conspectus = StringToItem(line);
                if (conspectus.TopicId == topicId)
                    conspectusList.Add(conspectus);
            }
        }

        return conspectusList;
    }
}
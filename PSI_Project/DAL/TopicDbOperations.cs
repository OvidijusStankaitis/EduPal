namespace PSI_Project.DAL;

public class TopicDbOperations : EntityDbOperations<Topic>
{
    protected override string DbFilePath => "..//PSI_Project//DB//topic.txt";

    protected override string ItemToDbString(Topic item)
    {
        return $"{item.Id};{item.SubjectId};{item.Name}";
    }

    protected override Topic StringToItem(string dbString)
    {
        String[] topicFields = dbString.Split(";");
        return new Topic(topicFields[0], topicFields[1], topicFields[2]);
    }
    
    public List<Topic> GetTopicListBySubjectId(string subjectId)
    {
        List<Topic> topicList = new List<Topic>();
        
        using (StreamReader sr = File.OpenText(DbFilePath))
        {
            string? line;
            while ((line = sr.ReadLine()) != null)
            {
                Topic topic = StringToItem(line);
                if (topic.SubjectId == subjectId)
                    topicList.Add(topic);
            }
        }

        return topicList;
    }
}
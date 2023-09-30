namespace PSI_Project.DAL;

public class TopicDbOperations : EntityDbOperations<Topic>
{
    protected override string DbFilePath => "..//PSI_Project//DB//topic.txt";

    protected override string ItemToDbString(Topic item)
    {
        return $"{item.Id};{item.Name};{item.Description};{item.SubjectName}";
    }

    protected override Topic StringToItem(string dbString)
    {
        String[] topicFields = dbString.Split(new char[] { ';' });
        return new Topic(topicFields[0], topicFields[1], topicFields[2], topicFields[3]);
    }
}
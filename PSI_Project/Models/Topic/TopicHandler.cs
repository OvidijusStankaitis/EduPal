using PSI_Project;

public class TopicHandler : BaseHandler<Topic>
{
    public TopicHandler()
    {
        ReadAllItemsFromDB();
    }

    protected override string DbFilePath => "DB//TopicInformation.txt";
    protected override string TempDbFilePath => "DB//TopicInformation_temp.txt";


    protected override string ItemToDbString(Topic item)
    {
        return $"{item.Name} {item.Description} {item.SubjectName}";
    }

    protected override Topic StringToItem(string dbString)
    {
        String[] topicFields = dbString.Split(new char[] { ' ' });
        return new Topic(topicFields[0], topicFields[1], topicFields[2]);
    }
}
using PSI_Project.Models;

namespace PSI_Project.Repositories;
public class TopicRepository : BaseRepository<Topic>
{
    protected override string DbFilePath => "..//PSI_Project//DB//topic.txt";
    
    public List<Topic> GetTopicsBySubjectName(string subjectName)
    {
        return Items.Where(topic => topic.SubjectName.Equals(subjectName)).ToList();
    }
    
    protected override string ItemToDbString(Topic item)
    {
        return $"{item.Id};{item.SubjectName};{item.Name};";
    }

    protected override Topic StringToItem(string dbString)
    {
        String[] topicFields = dbString.Split(";");
        Topic newTopic = new Topic(topicFields[2], topicFields[1]); // use only name to construct Subject
        newTopic.Id = topicFields[0];
        return newTopic;
    }
}
using System.Text.Json;
using PSI_Project.Models;

namespace PSI_Project.Repositories;
public class TopicRepository : BaseRepository<Topic>
{
    protected override string DbFilePath => "..//PSI_Project//DB//topic.txt";
    
    public List<Topic> GetTopicsBySubjectName(string subjectName)
    {
        return Items.Where(topic => topic.SubjectName.Equals(subjectName)).ToList();
    }

    public List<Topic>? CreateTopic(JsonElement request)
    {
        if (request.TryGetProperty("topicName", out var topicNameProperty) &&
            request.TryGetProperty("subjectName", out var subjectNameProperty))
        {
            string? topicName = topicNameProperty.GetString();
            string? subjectName = subjectNameProperty.GetString();

            if (subjectName == null || topicName == null)
                return null;

            InsertItem(new Topic(topicName, subjectName));
            return Items;
        }
        return null;
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
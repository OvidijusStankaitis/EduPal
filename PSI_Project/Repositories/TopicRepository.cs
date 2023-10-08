using System.Text.Json;
using PSI_Project.Models;

namespace PSI_Project.Repositories;
public class TopicRepository : BaseRepository<Topic>
{
    protected override string DbFilePath => "..//PSI_Project//DB//topic.txt";
    
    public List<Topic> GetTopicsBySubjectId(string subjectId)
    {
        return Items.Where(topic => topic.SubjectId.Equals(subjectId)).ToList();
    }

    public Topic? CreateTopic(JsonElement request)
    {
        if (request.TryGetProperty("topicName", out var topicNameProperty) &&
            request.TryGetProperty("subjectId", out var subjectNameProperty))
        {
            string? topicName = topicNameProperty.GetString();
            string? subjectId = subjectNameProperty.GetString();
            
            if (subjectId != null && topicName != null)
            {
                Topic newTopic = new Topic(topicName, subjectId);
                if(InsertItem(newTopic));
                    return newTopic;
            }
        }

        return null;
    }

    protected override string ItemToDbString(Topic item)
    {
        return $"{item.Id};{item.SubjectId};{item.Name};";
    }

    protected override Topic StringToItem(string dbString)
    {
        String[] topicFields = dbString.Split(";");
        Topic newTopic = new Topic(topicFields[2], topicFields[1])
        {
            Id = topicFields[0]
        };
        return newTopic;
    }
}
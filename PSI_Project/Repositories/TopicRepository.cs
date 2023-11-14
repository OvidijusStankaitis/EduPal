using System.Text.Json;
using PSI_Project.Data;
using PSI_Project.Models;

namespace PSI_Project.Repositories;
public class TopicRepository : Repository<Topic>
{
    public EduPalDatabaseContext EduPalContext => Context as EduPalDatabaseContext;
    
    public TopicRepository(EduPalDatabaseContext context) : base(context)
    {
    }
    
    public IEnumerable<Topic> GetTopicsListBySubjectId(string subjectId)
    {
        return Find(topic => topic.Subject.Id == subjectId);
    }

    public Topic? Create(JsonElement request)
    {
        if (!request.TryGetProperty("topicName", out var topicNameProperty) ||
            !request.TryGetProperty("subjectId", out var subjectNameProperty))
            return null;
            
        string? topicName = topicNameProperty.GetString();
        string? subjectId = subjectNameProperty.GetString();
        if (subjectId is null || topicName is null)
            return null;

        Subject subject = EduPalContext.Subjects.Find(subjectId);
        Topic newTopic = new Topic(topicName, subject);
        Add(newTopic);
        int changes = EduPalContext.SaveChanges();
        
        return changes > 0 ? newTopic : null;
    }
    
    public bool UpdateKnowledgeLevel(string topicId, KnowledgeLevel knowledgeLevel)
    {
        Topic? topic = Get(topicId);

        if (topic == null)
        {
            return false; // Topic not found
        }

        topic.KnowledgeRating = knowledgeLevel;
        
        EduPalContext.SaveChanges();

        return true;
    }
    
    public bool Remove(string topicId)
    {
        Topic topic = Get(topicId);
        Remove(topic);
        int changes = EduPalContext.SaveChanges();

        return changes > 0;
    } 
}
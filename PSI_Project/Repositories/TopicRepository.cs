using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PSI_Project.Data;
using PSI_Project.Models;

namespace PSI_Project.Repositories;
public class TopicRepository : Repository<Topic>
{
    public EduPalDatabaseContext EduPalContext => Context as EduPalDatabaseContext;
    
    public TopicRepository(EduPalDatabaseContext context) : base(context)
    {
    }
    
    public async Task<Topic?> GetAsync(string topicId)
    {
        return await EduPalContext.Topics.FindAsync(topicId);
    }
    
    public async Task<IEnumerable<Topic>> GetTopicsListBySubjectIdAsync(string subjectId)
    {
        return await EduPalContext.Topics.Where(topic => topic.Subject.Id == subjectId).ToListAsync();
    }

    public async Task<Topic?> CreateAsync(JsonElement request)
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
        
        int changes = await EduPalContext.SaveChangesAsync();
        return changes > 0 ? newTopic : null;
    }
    
    public async Task<bool> RemoveAsync(string topicId)
    {
        Topic? topic = Get(topicId);
        if (topic is null)
            return false;
        Remove(topic);
        
        int changes = await EduPalContext.SaveChangesAsync();
        return changes > 0;
    } 
}
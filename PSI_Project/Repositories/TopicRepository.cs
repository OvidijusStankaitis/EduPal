using Microsoft.EntityFrameworkCore;
using PSI_Project.Data;
using PSI_Project.Models;

namespace PSI_Project.Repositories
{
    public class TopicRepository : Repository<Topic>
    {
        public EduPalDatabaseContext EduPalContext => Context as EduPalDatabaseContext;

        public TopicRepository(EduPalDatabaseContext context) : base(context)
        {
        }
        
        public async Task<List<Topic>> GetTopicsListBySubjectIdAsync(string subjectId)
        {
            // return await EduPalContext.Topics
            //     .Where(topic => topic.Subject.Id == subjectId)
            //     .ToListAsync();
            var topics = await FindAsync(topic => topic.Subject.Id == subjectId);
            return topics.ToList();
        }

        public async Task<Topic?> CreateAsync(string topicName, string subjectId)
        {
            Subject subject = await EduPalContext.Subjects.FindAsync(subjectId);
            Topic newTopic = new Topic(topicName, subject);
            int changes = Add(newTopic);
            //int changes = await EduPalContext.SaveChangesAsync();
            
            return changes > 0 ? newTopic : null;
        }


        public async Task<bool> UpdateKnowledgeLevelAsync(string topicId, KnowledgeLevel knowledgeLevel)
        {
            Topic? topic = await GetAsync(topicId);

            if (topic == null)
            {
                return false; // Topic not found
            }

            topic.KnowledgeRating = knowledgeLevel;

            await EduPalContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveAsync(string topicId)
        {
            Topic topic = await GetAsync(topicId);
            int changes = Remove(topic);
            //int changes = await EduPalContext.SaveChangesAsync();

            return changes > 0;
        }
    }
}
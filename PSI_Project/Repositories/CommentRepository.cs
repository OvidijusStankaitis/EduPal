using Microsoft.EntityFrameworkCore;
using PSI_Project.Data;
using PSI_Project.Models;

namespace PSI_Project.Repositories
{
    public class CommentRepository : Repository<Comment>
    {
        public EduPalDatabaseContext EduPalContext => Context as EduPalDatabaseContext;

        public CommentRepository(EduPalDatabaseContext context) : base(context)
        {
        }
        
        public async Task<List<Comment>> GetAllCommentsOfTopicAsync(string topicId)
        {
            var comments = await FindAsync(comment => comment.TopicId == topicId);
            return comments.ToList();
        }
        
        public async Task<Comment?> GetItemByIdAsync(string itemId)  
        {
            var comments = await FindAsync(comment => comment.Id == itemId);
            return comments.FirstOrDefault();
        }
        
        public async Task<bool> RemoveAsync(string commentId) 
        {
            Comment comment = await GetAsync(commentId);
            if (comment == null)
                return false; // Comment not found

            int changes = Remove(comment);
            return changes > 0;
        }
    }
}
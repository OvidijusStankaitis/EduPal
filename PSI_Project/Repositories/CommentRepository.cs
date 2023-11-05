using System.Text.Json;
using PSI_Project.Data;
using PSI_Project.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace PSI_Project.Repositories;
public class CommentRepository : Repository<Comment>
{
    public EduPalDatabaseContext EduPalContext => Context as EduPalDatabaseContext;

    public CommentRepository(EduPalDatabaseContext context) : base(context)
    {
    }
    
    public async Task<Comment?> GetAsync(string commentId)
    {
        return await EduPalContext.Comments.FindAsync(commentId);
    }
    
    public async Task<List<Comment>> GetAllCommentsOfTopicAsync(string topicId)
    {
        return await EduPalContext.Comments.Where(comment => comment.Topic.Id.Equals(topicId)).ToListAsync();
    }
    
    public async Task<Comment?> GetItemByIdAsync(string itemId)
    {
        return await EduPalContext.Comments.FirstOrDefaultAsync(comment => comment.Id.Equals(itemId));
    }
    
    public async Task<Comment?> CreateCommentAsync(JsonElement request)
    {
        if (request.TryGetProperty("commentText", out var commentTextProperty) &&
            request.TryGetProperty("topicId", out var topicIdProperty))
        {
            string? commentText = commentTextProperty.GetString();
            string? topicId = topicIdProperty.GetString();
            
            if (topicId != null && commentText != null)
            {
                Topic topic = await EduPalContext.Topics.FindAsync(topicId);
                Comment newComment = new Comment(topic, commentText);
                Add(newComment);
                int changes = await EduPalContext.SaveChangesAsync();
                
                return changes > 0 ? newComment : null;
            }
        }
        return null;
    }

    public bool Remove(string commentId)
    {
        Comment? comment = Get(commentId);
        if (comment is null)
            return false;

        Remove(comment);
        int changes = EduPalContext.SaveChanges();

        return changes > 0;
    }
}
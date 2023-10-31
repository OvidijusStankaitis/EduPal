using System.Text.Json;
using PSI_Project.Data;
using PSI_Project.Models;

namespace PSI_Project.Repositories;

public class CommentRepository : Repository<Comment>
{
    public EduPalDatabaseContext EduPalContext => Context as EduPalDatabaseContext;

    public CommentRepository(EduPalDatabaseContext context) : base(context)
    {
    }
    
    public List<Comment> GetAllCommentsOfTopic(string topicId)
    {
        return EduPalContext.Comments.Select(comment => comment).Where(comment => comment.TopicId.Equals(topicId)).ToList();
    }
    
    public Comment? GetItemById(string itemId)  
    {
        return EduPalContext.Comments.FirstOrDefault(comment => comment.Id.Equals(itemId));
    }
    
    public Comment? CreateComment(JsonElement request)
    {
        if (request.TryGetProperty("senderId", out var senderProperty) &&
            request.TryGetProperty("commentText", out var commentTextProperty) &&
            request.TryGetProperty("topicId", out var topicIdProperty))
        {
            string? senderId = senderProperty.GetString();
            string? commentText = commentTextProperty.GetString();
            string? topicId = topicIdProperty.GetString();
            
            if (senderId != null && topicId != null && commentText != null)
            {
                // Topic topic = EduPalContext.Topics.Find(topicId); TODO: check if such topic exists
                Comment newComment = new Comment(senderId, topicId, commentText);
                Add(newComment);
                int changes = EduPalContext.SaveChanges();
                
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
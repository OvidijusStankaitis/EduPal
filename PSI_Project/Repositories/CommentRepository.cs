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
        return EduPalContext.Comments.Select(comment => comment).Where(comment => comment.Topic.Id.Equals(topicId)).ToList();
    }
    
    public Comment? GetItemById(string itemId)  
    {
        return EduPalContext.Comments.FirstOrDefault(comment => comment.Id.Equals(itemId));
    }
    
    public Comment? CreateComment(JsonElement request)
    {
        if (request.TryGetProperty("commentText", out var commentTextProperty) &&
            request.TryGetProperty("topicId", out var topicIdProperty))
        {
            string? commentText = commentTextProperty.GetString();
            string? topicId = topicIdProperty.GetString();
            
            if (topicId != null && commentText != null)
            {
                Topic topic = EduPalContext.Topics.Find(topicId);
                Comment newComment = new Comment(topic, commentText);
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
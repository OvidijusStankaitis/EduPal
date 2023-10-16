using System.Text.Json;
using PSI_Project.Models;

namespace PSI_Project.Repositories;

public class CommentRepository : BaseRepository<Comment>
{
    protected override string DbFilePath => "..//PSI_Project//DB//comment.txt";

    public List<Comment> GetAllCommentsOfTopic(string topicId)
    {
        return Items.Where(comment => comment.TopicId.Equals(topicId)).ToList();
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
                Comment newComment = new Comment(topicId:topicId, commentText:commentText);
                if(InsertItem(newComment));
                return newComment;
            }
        }
        return null;
    }
    
    protected override string ItemToDbString(Comment item)
    {
        return $"{item.Id};{item.TopicId};{item.CommentText};";
    }

    protected override Comment StringToItem(string dbString)
    {
        String[] commentFields = dbString.Split(";");
        Comment newComment = new Comment(topicId: commentFields[1], commentText: commentFields[2])
        {
            Id = commentFields[0]
        };
        return newComment;
    }
}
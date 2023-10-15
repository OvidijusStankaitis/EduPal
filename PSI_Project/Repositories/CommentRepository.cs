using System.Text.Json;
using PSI_Project.Models;

namespace PSI_Project.Repositories;

public class CommentRepository : BaseRepository<Comment>
{
    protected override string DbFilePath => "..//PSI_Project//DB//comment.txt";

    public List<Comment> GetAllCommentsOfConspectus(string conspectusId)
    {
        return Items.Where(comment => comment.ConspectusId.Equals(conspectusId)).ToList();
    }
    
    public Comment? CreateComment(JsonElement request) //needs to be tested
    {
        if (request.TryGetProperty("commentText", out var commentTextProperty) &&
            request.TryGetProperty("conspectusId", out var conspectusIdProperty))
        {
            string? commentText = commentTextProperty.GetString();
            string? conspectusId = conspectusIdProperty.GetString();
            
            if (conspectusId != null && commentText != null)
            {
                Comment newComment = new Comment(conspectusId:conspectusId, commentText:commentText);
                if(InsertItem(newComment));
                return newComment;
            }
        }
        return null;
    }
    
    protected override string ItemToDbString(Comment item)
    {
        return $"{item.Id};{item.ConspectusId};{item.CommentText};";
    }

    protected override Comment StringToItem(string dbString)
    {
        String[] commentFields = dbString.Split(";");
        Comment newComment = new Comment(conspectusId: commentFields[1], commentText: commentFields[2])
        {
            Id = commentFields[0]
        };
        return newComment;
    }
}
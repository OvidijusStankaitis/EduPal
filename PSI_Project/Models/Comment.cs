namespace PSI_Project.Models;

public class Comment : BaseEntity 
{
    public string ConspectusId { get; init; }
    public string CommentText { get; init; } 

    public Comment(string conspectusId, string commentText) : base()
    {
        ConspectusId = conspectusId;
        CommentText = commentText;
    }
}
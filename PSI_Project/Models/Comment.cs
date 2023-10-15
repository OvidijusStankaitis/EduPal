namespace PSI_Project.Models;

public class Comment : BaseEntity //name is not needed for Comment. Is ID needed? (inherited BaseEntity because it is required so CommentRepository could inherit BaseRepository)
{
    public string ConspectusId { get; init; }
    public string CommentText { get; init; } // As we discussed, comment cannot me modified

    public Comment(string conspectusId, string commentText) : base("")
    {
        ConspectusId = conspectusId;
        CommentText = commentText;
    }
}
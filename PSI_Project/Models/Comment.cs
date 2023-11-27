using System.ComponentModel.DataAnnotations.Schema;

namespace PSI_Project.Models;

public class Comment : BaseEntity
{
    [ForeignKey("TopicId")]
    public string TopicId { get; set; }
    public Topic Topic { get; init; }
    [ForeignKey("UserId")]
    public string UserId { get; set; }
    public User User { get; init; }
    public string CommentText { get; init; }
    
    public Comment(string userId, string topicId, string commentText)
    {
        UserId = userId;
        TopicId = topicId;
        CommentText = commentText;
    }
}
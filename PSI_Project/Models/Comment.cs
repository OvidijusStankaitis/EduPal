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
    public string Content { get; init; }
    public DateTime Timestamp { get; init; }

    public Comment(string userId, string topicId, string content)
    {
        UserId = userId;
        TopicId = topicId;
        Content = content;
        Timestamp = DateTime.UtcNow;
    }
}
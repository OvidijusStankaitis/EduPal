using System.ComponentModel.DataAnnotations.Schema;

namespace PSI_Project.Models;

public class Comment : BaseEntity
{
    [ForeignKey("Topic")]
    public string TopicId { get; set; }
    public Topic Topic { get; init; }
    public User User { get; init; }
    public string CommentText { get; init; }

    private Comment()   // used by EF
    {
    }
    
    public Comment(Topic topic, string commentText) // TODO: should be deleted, comment must have a user
    {
        Topic = topic;
        CommentText = commentText;
    }

    public Comment(User user, Topic topic, string commentText)
    {
        User = user;
        Topic = topic;
        CommentText = commentText;
    }
}
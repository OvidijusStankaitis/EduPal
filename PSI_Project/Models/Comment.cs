namespace PSI_Project.Models;

public class Comment : BaseEntity 
{
    //public string TopicId { get; init; }
    public Topic Topic { get; init; }
    public string CommentText { get; init; }

    public Comment()
    {
    }

    public Comment(Topic topic, string commentText)
    {
        Topic = topic;
        CommentText = commentText;
    }
}
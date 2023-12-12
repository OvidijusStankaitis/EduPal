using PSI_Project.DTO;
using PSI_Project.Models;
using PSI_Project.Repositories;

namespace PSI_Project.Services;

public class ChatService
{
    private readonly CommentRepository _commentRepository;
    private readonly ILogger<ChatService> _logger;

    public ChatService(CommentRepository commentRepository, ILogger<ChatService> logger)
    {
        _logger = logger;
        _commentRepository = commentRepository;
    }

    public ChatService() // for tests
    {
    }

    public virtual IEnumerable<CommentDTO> GetMessagesForUser(User user, string topicId)
    {
        IEnumerable<Comment> comments = _commentRepository.GetAll();
        IEnumerable<CommentDTO> commentDtoList = new List<CommentDTO>();

        foreach (Comment comment in comments)
        {
            if (comment.TopicId == topicId)
            {
                CommentDTO commentDto = new CommentDTO(comment.Id, comment.Content, comment.Timestamp,
                    comment.UserId == user.Id);
                commentDtoList = commentDtoList.Append(commentDto);
            }
        }

        return commentDtoList.OrderBy(comment => comment.TimeStamp);
    }

    public Comment SaveSentMessage(string userId, string topicId, string message)
    {
        Comment newComment = new Comment(userId, topicId, message);
        _commentRepository.Add(newComment);

        return newComment;
    }

    public async Task<Comment> DeleteMessage(string messageId)
    {
        Comment message = await _commentRepository.GetAsync(messageId);
        _commentRepository.Remove(message);

        return message;
    }
}
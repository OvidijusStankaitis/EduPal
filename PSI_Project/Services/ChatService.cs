using Microsoft.AspNetCore.SignalR;
using PSI_Project.Hubs;
using PSI_Project.Models;
using PSI_Project.Repositories;

namespace PSI_Project.Services;

public class ChatService
{
    private readonly CommentRepository _commentRepository;

    public ChatService(CommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }
    
    public Comment SaveSentMessage(string userId, string topicId, string message)
    {
        Comment newComment = new Comment(userId, topicId, message);
        _commentRepository.Add(newComment);
        _commentRepository.EduPalContext.SaveChanges();
        
        return newComment;
    }

    public Comment DeleteMessage(string messageId)
    {
        Comment message = _commentRepository.Get(messageId);
        _commentRepository.Remove(message);
        _commentRepository.EduPalContext.SaveChanges();
        
        return message;
    }
}
using Microsoft.AspNetCore.SignalR;
using PSI_Project.Hubs;
using PSI_Project.Models;
using PSI_Project.Repositories;

namespace PSI_Project.Services;

public class ChatService
{
    private readonly CommentRepository _commentRepository;
    private readonly UserRepository _userRepository;
    private readonly TopicRepository _topicRepository;

    public ChatService(CommentRepository commentRepository, UserRepository userRepository, TopicRepository topicRepository)
    {
        _commentRepository = commentRepository;
        _userRepository = userRepository;
        _topicRepository = topicRepository;
    }
    
    public void SaveSentMessage(string userId, string topicId, string message)
    {
        User? sender = _userRepository.Get(userId);
        Topic? currentTopic = _topicRepository.Get(topicId);
        
        if (sender is null || currentTopic is null)
        {
            return;
        }
        
        Comment newComment = new Comment(sender, currentTopic, message);
        _commentRepository.Add(newComment);
    }
}